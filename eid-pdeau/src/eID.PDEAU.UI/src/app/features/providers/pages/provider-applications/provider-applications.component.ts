import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { ProviderService } from '../../provider.service';
import { Table } from 'primeng/table';
import { LazyLoadEvent } from 'primeng/api';
import { ApplicationFilter } from './components/applications-filter/applications-filter.component';
import { UserService } from '@app/core/services/user.service';
import { RoleType } from '@app/core/enums/auth.enum';

@Component({
    selector: 'app-provider-applications',
    templateUrl: './provider-applications.component.html',
    styleUrls: ['./provider-applications.component.scss'],
})
export class ProviderApplicationsComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private translateService: TranslocoService,
        private providerService: ProviderService,
        private toastService: ToastService,
        private userService: UserService
    ) {}

    @ViewChild('applicationsTable') table!: Table;

    applicationsQuery = new RequestHandler({
        requestFunction: this.providerService.fetchProviderApplications,
    });

    lazyLoadEvent: LazyLoadEvent = {};
    filters: ApplicationFilter = {};

    isEmployee = false;
    _userService = this.userService;
    showTable = false;
    sortField = 'createdOn';
    sortOrder = -1;
    sessionStorageKey = 'provider-applications-table';
    RoleType = RoleType;

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                return;
            })
        );
        this.isEmployee = this.userService.hasRole(RoleType.EMPLOYEE);
        this.sortField = sessionStorage.getItem(this.sessionStorageKey)
            ? JSON.parse(sessionStorage[this.sessionStorageKey])['sortField']
            : 'createdOn';
        this.sortOrder = sessionStorage.getItem(this.sessionStorageKey)
            ? JSON.parse(sessionStorage[this.sessionStorageKey])['sortOrder']
            : -1;
        this.showTable = true;
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    onFilterDataChange(data: ApplicationFilter) {
        this.filters = data;
        // always start from the first page when filters are changed
        this.lazyLoadEvent.first = 0;
        if (this.table) {
            this.table.first = 0;
            this.loadApplications();
        }
    }

    onLazyLoad(event: LazyLoadEvent) {
        this.lazyLoadEvent = event;
        this.loadApplications();
    }

    loadApplications() {
        const sortBy = this.lazyLoadEvent?.sortField || 'createdOn';
        const sortDirection = this.lazyLoadEvent?.sortOrder === 1 ? 'asc' : 'desc';
        const first = this.lazyLoadEvent?.first || 0;
        const pageSize = this.lazyLoadEvent?.rows || 10;

        this.applicationsQuery.execute({
            pageIndex: first / pageSize + 1,
            pageSize,
            sortBy,
            sortDirection,
            ...this.filters,
        });
    }
}
