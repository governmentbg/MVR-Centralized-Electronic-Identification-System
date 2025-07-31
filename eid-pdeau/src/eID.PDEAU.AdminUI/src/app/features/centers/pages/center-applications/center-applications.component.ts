import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { UserService } from '@app/core/services/user.service';
import { Table } from 'primeng/table';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { LazyLoadEvent } from 'primeng/api';
import { CentersService } from '@app/features/centers/centers.service';
import { ApplicationFilter } from '@app/features/centers/pages/center-applications/components/applications-filter/applications-filter.component';
import { ApplicationType } from '@app/features/centers/enums/centers.enum';
import { eidManagerStatus } from '@app/features/administrators/enums/administrators.enum';

@Component({
    selector: 'app-center-applications',
    templateUrl: './center-applications.component.html',
    styleUrls: ['./center-applications.component.scss'],
})
export class CenterApplicationsComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private translateService: TranslocoService,
        private userService: UserService,
        private centersService: CentersService
    ) {}

    @ViewChild('applicationsTable') table!: Table;

    applicationsQuery = new RequestHandler({
        requestFunction: this.centersService.fetchApplications,
    });

    circumstanceChangesQuery = new RequestHandler({
        requestFunction: this.centersService.fetchCenterCircumstanceChanges,
    });

    lazyLoadEvent: LazyLoadEvent = {};
    filters: any = {};
    showTable = false;
    sortField = 'createDate';
    sessionStorageKey = 'center-applications-table';

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                return;
            })
        );
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
        return this.translateService.translate(`modules.centers.applications.types.${type}`);
    }

    loadCircumstanceChanges() {
        this.circumstanceChangesQuery.execute(eidManagerStatus.IN_REVIEW);
    }
}
