import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { Table } from 'primeng/table';
import { ConfirmationService, LazyLoadEvent } from 'primeng/api';
import { UsersService } from '../../users.service';
import { UserFilterType } from './components/users-filter/users-filter.component';
import { GetUserType } from '../../users.dto';
import { Router } from '@angular/router';

@Component({
    selector: 'app-users-table',
    templateUrl: './users-table.component.html',
    styleUrls: ['./users-table.component.scss'],
    providers: [ConfirmationService],
})
export class UsersTableComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private translateService: TranslocoService,
        private usersService: UsersService,
        private toastService: ToastService,
        private router: Router,
        private confirmationService: ConfirmationService
    ) {}

    @ViewChild('usersTable') table!: Table;

    usersQuery = new RequestHandler({
        requestFunction: this.usersService.fetchUsers,
    });
    deleteUserQuery = new RequestHandler({
        requestFunction: this.usersService.deleteUser,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.users.userDelete.toasts.successfulDelete')
            );
            this.loadUsers();
        },
        onError: errorResponse => {
            let errorMessage = this.translateService.translate('modules.users.userDelete.toasts.failedDelete');
            switch (errorResponse.status) {
                case 400: {
                    errorMessage = this.translateService.translate('global.txtInvalidDataError');
                    break;
                }
                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Id') {
                        errorMessage = this.translateService.translate('modules.users.userDelete.toasts.Id');
                    }
                }
            }
            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
        },
    });

    lazyLoadEvent: LazyLoadEvent = {};
    filters: UserFilterType = {};
    userToEdit: GetUserType | null = null;
    showTable = false;
    sortField = 'createdOn';
    sessionStorageKey = 'users-table';

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                return;
            })
        );
        this.sortField = sessionStorage.getItem(this.sessionStorageKey)
            ? JSON.parse(sessionStorage[this.sessionStorageKey])['sortField']
            : 'createdOn';
        this.showTable = true;
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    onFilterDataChange(data: UserFilterType) {
        this.filters = data;
        // always start from the first page when filters are changed
        this.lazyLoadEvent.first = 0;
        if (this.table) {
            this.table.first = 0;
            this.loadUsers();
        }
    }

    onLazyLoad(event: LazyLoadEvent) {
        this.lazyLoadEvent = event;
        this.loadUsers();
    }

    loadUsers() {
        const sortBy = this.lazyLoadEvent?.sortField;
        const sortDirection = this.lazyLoadEvent?.sortOrder === 1 ? 'asc' : 'desc';
        const first = this.lazyLoadEvent?.first || 0;
        const pageSize = this.lazyLoadEvent?.rows || 10;
        this.usersQuery.execute({
            pageIndex: first / pageSize + 1,
            pageSize,
            sortBy,
            sortDirection,
            ...this.filters,
        });
    }

    previewUser(userId: string) {
        this.router.navigate([`/users/${userId}`]);
    }

    selectUser(userData: GetUserType) {
        this.userToEdit = userData;
    }

    deleteUser(userData: GetUserType) {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            message: this.translateService.translate('modules.users.usersTable.deleteUserConfirmation'),
            header: this.translateService.translate('global.txtConfirmation'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.deleteUserQuery.execute(userData.id);
            },
        });
    }

    closeEdit() {
        this.userToEdit = null;
    }
}
