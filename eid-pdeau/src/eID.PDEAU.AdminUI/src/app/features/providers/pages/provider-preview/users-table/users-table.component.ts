import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { UserType } from '@app/features/providers/provider.dto';
import { ProviderService } from '@app/features/providers/provider.service';
import { Table } from 'primeng/table';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { TranslocoService } from '@ngneat/transloco';
import { CreateUserType, GetUserType } from '@app/shared/interfaces/users.dto';
import { UserFormComponent } from '@app/shared/components/user-form/user-form.component';

@Component({
    selector: 'app-users-table',
    templateUrl: './users-table.component.html',
    styleUrls: ['./users-table.component.scss'],
})
export class UsersTableComponent {
    constructor(
        private providerService: ProviderService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}
    @ViewChild('usersTable') table!: Table;
    @ViewChild('filterInput') filterInput!: ElementRef<HTMLInputElement>;
    @ViewChild('userForm') userForm!: UserFormComponent;
    @Input() users: UserType[] = [];
    @Input() providerId: string | null = null;
    @Input() allowUsersModification = false;
    @Output() refreshUsers = new EventEmitter();

    userToEdit: GetUserType | null = null;
    showDialog = false;
    showAdministratorActionsDialog = false;
    userToPreview: GetUserType | null = null;
    addUserQuery = new RequestHandler({
        requestFunction: this.providerService.addUser,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.users.userRegister.toasts.successfulRegistration')
            );
            this.showDialog = false;
            this.refreshUsers.emit();
        },
        onError: errorResponse => {
            // Default error message
            let errorMessage = this.translateService.translate('modules.users.userRegister.toasts.failedRegistration');
            switch (errorResponse.status) {
                case 400: {
                    errorMessage = this.translateService.translate('global.txtInvalidDataError');
                    break;
                }
                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Uid') {
                        errorMessage = this.translateService.translate('modules.users.userRegister.toasts.Uid');
                    }
                }
            }

            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
        },
    });
    editUserQuery = new RequestHandler({
        requestFunction: this.providerService.updateUser,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.users.userEdit.toasts.successfulEdit')
            );
            this.showDialog = false;
            this.refreshUsers.emit();
        },
        onError: errorResponse => {
            let errorMessage = this.translateService.translate('modules.users.userEdit.toasts.failedEdit');
            switch (errorResponse.status) {
                case 400: {
                    errorMessage = this.translateService.translate('global.txtInvalidDataError');
                    break;
                }
                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Id') {
                        errorMessage = this.translateService.translate('modules.users.userEdit.toasts.Id');
                    }
                }
            }
            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
        },
    });
    fetchUserHistoryQuery = new RequestHandler({
        requestFunction: this.providerService.fetchAdministratorActions,
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.users.userEdit.toasts.failedEdit')
            );
        },
    });

    globalFilter(event: Event) {
        this.table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
    }

    clearGlobalFilter() {
        this.table.clear();
        this.filterInput.nativeElement.value = '';
    }

    previewAdministratorAction(userData: any) {
        this.userToPreview = userData;
        this.fetchUserHistoryQuery.data = [];
        this.showAdministratorActionsDialog = true;
        this.fetchUserHistoryQuery.execute(this.providerId as string, userData.id);
    }

    addUser() {
        this.userToEdit = null;
        this.showDialog = true;
    }

    editUser(userData: any) {
        this.userToEdit = userData;
        this.showDialog = true;
    }

    onUserFormSubmit(data: CreateUserType) {
        if (this.providerId) {
            if (this.userToEdit) {
                this.editUserQuery.execute(this.providerId, {
                    isAdministrator: data.isAdministrator,
                    id: this.userToEdit.id,
                    comment: data.comment,
                });
            } else {
                this.addUserQuery.execute(this.providerId, data);
            }
        }
        this.userToEdit = null;
    }
}
