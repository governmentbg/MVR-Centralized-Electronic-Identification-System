import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { UserService } from '@app/core/services/user.service';
import { Table } from 'primeng/table';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { LazyLoadEvent } from 'primeng/api';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { RoleType } from '@app/core/enums/auth.enum';
import { ApplicationFilter } from '@app/features/administrators/pages/administrator-applications/components/applications-filter/applications-filter.component';
import { ApplicationType } from '@app/features/administrators/enums/administrators.enum';

@Component({
    selector: 'app-administrator-applications',
    templateUrl: './administrator-applications.component.html',
    styleUrls: ['./administrator-applications.component.scss'],
})
export class AdministratorApplicationsComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private translateService: TranslocoService,
        private userService: UserService,
        private administratorService: AdministratorsService
    ) {}

    @ViewChild('applicationsTable') table!: Table;

    applicationsQuery = new RequestHandler({
        requestFunction: this.administratorService.fetchApplications,
    });
    lazyLoadEvent: LazyLoadEvent = {};
    filters: any = {};

    isEmployee = false;
    _userService = this.userService;
    showTable = false;
    sortField = 'createDate';
    sessionStorageKey = 'administrator-applications-table';
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
            : 'createDate';
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
        const sortOrder = this.lazyLoadEvent.sortOrder === 1 ? 'asc' : 'desc';
        let payload = {
            page: 0,
            size: this.lazyLoadEvent.rows,
            sort: `${this.lazyLoadEvent.sortField},${sortOrder}`,
        };

        if (this.filters !== null && this.filters !== undefined) {
            payload = { ...payload, ...this.filters };
        } else if (this.filters !== null && Object.keys(this.filters).length > 0) {
            payload = { ...payload, ...this.filters };
        }

        if (this.lazyLoadEvent && this.lazyLoadEvent.first && this.lazyLoadEvent.rows) {
            payload.page = this.lazyLoadEvent.first / this.lazyLoadEvent.rows;
        }

        this.applicationsQuery.execute({
            ...payload,
            ...this.filters,
        });
    }

    getApplicationTypeText(type: ApplicationType) {
        return this.translateService.translate(`modules.administrators.applications.types.${type}`);
    }
}
