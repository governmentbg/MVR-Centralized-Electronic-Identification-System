import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { Table } from 'primeng/table';
import { LazyLoadEvent } from 'primeng/api';
import { ServicesFilterT } from './components/services-filter/services-filter.component';
import { ProviderServiceType, providerServiceStatusEnum } from '../../services.dto';
import { ProviderServicesService } from '../../provider-services.service';
import { CurrentProviderService } from '@app/core/services/current-provider.service';

type PreviewType = 'view' | 'edit';

@Component({
    selector: 'app-provider-services',
    templateUrl: './provider-services.component.html',
    styleUrls: ['./provider-services.component.scss'],
})
export class ProviderServicesComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private translateService: TranslocoService,
        private providerServicesService: ProviderServicesService,
        private toastService: ToastService,
        private currentProviderService: CurrentProviderService
    ) {}

    @ViewChild('servicesTable') table!: Table;

    servicesQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchProviderServices,
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.services.services-table.table.errors.general')
            );
        },
    });

    lazyLoadEvent: LazyLoadEvent = {};
    filters: ServicesFilterT = {};
    selectedService: ProviderServiceType | null = null;
    previewMode: PreviewType = 'view';
    showTable = false;
    sortField = 'serviceNumber';
    providerServiceStatusEnum = providerServiceStatusEnum;

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                return;
            })
        );
        this.showTable = true;
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    onFilterDataChange(data: ServicesFilterT) {
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
        const sortBy = this.lazyLoadEvent?.sortField || 'serviceNumber';
        const sortDirection = this.lazyLoadEvent?.sortOrder === 1 ? 'asc' : 'desc';
        const first = this.lazyLoadEvent?.first || 0;
        const pageSize = this.lazyLoadEvent?.rows || 10;
        this.servicesQuery.execute({
            pageIndex: first / pageSize + 1,
            pageSize,
            sortBy,
            sortDirection,
            includeEmpowermentOnly: false,
            includeDeleted: false,
            includeWithoutScope: true,
            includeInactive: true,
            includeApprovedOnly: false,
            ...this.filters,
        });
    }

    selectService(application: ProviderServiceType, mode: PreviewType) {
        this.previewMode = mode;
        this.selectedService = application;
    }

    closePreview() {
        this.selectedService = null;
        this.loadApplications();
    }

    get isProviderPrivateSubjectType() {
        return this.currentProviderService.isOfType('PrivateLawSubject');
    }
}
