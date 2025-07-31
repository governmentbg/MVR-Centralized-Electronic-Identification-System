import { Component, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { ProviderService } from '../../provider.service';
import { Table } from 'primeng/table';
import { LazyLoadEvent } from 'primeng/api';
import { ProviderFilter } from './components/applications-filter/providers-filter.component';
import { ProviderServicesService } from '@app/features/providers/provider-services.service';
import { ServicesFilterT } from '@app/features/providers/pages/provider-services/components/services-filter/services-filter.component';
import { ProviderServiceType } from '@app/features/providers/services.dto';
import { RoleType } from '@app/core/enums/auth.enum';
type PreviewType = 'view' | 'edit';
@Component({
    selector: 'app-providers',
    templateUrl: './providers.component.html',
    styleUrls: ['./providers.component.scss'],
})
export class ProvidersComponent {
    private subscriptions = new Subscription();

    constructor(
        private translateService: TranslocoService,
        private providerService: ProviderService,
        private toastService: ToastService,
        private providerServicesService: ProviderServicesService
    ) {}

    @ViewChild('applicationsTable') table!: Table;

    applicationsQuery = new RequestHandler({
        requestFunction: this.providerService.fetchProviderApplications,
    });

    servicesQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchProviderServices,
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.services.services-table.table.errors.general')
            );
        },
    });

    providersLazyLoadEvent: LazyLoadEvent = {};
    servicesLazyLoadEvent: LazyLoadEvent = {};
    filters: ProviderFilter = {};
    servicesFilters: ServicesFilterT = {
        includeInactive: true,
    };
    selectedService: ProviderServiceType | null = null;
    selectedProvider: string | null = null;
    previewMode: PreviewType = 'view';

    onFilterDataChange(data: ProviderFilter) {
        this.filters = data;
        // always start from the first page when filters are changed
        this.providersLazyLoadEvent.first = 0;
        if (this.table) {
            this.table.first = 0;
            this.loadApplications();
        }
    }

    onLazyLoad(event: LazyLoadEvent) {
        this.providersLazyLoadEvent = event;
        this.loadApplications();
    }

    loadApplications() {
        const sortBy = this.providersLazyLoadEvent?.sortField || 'createdOn';
        const sortDirection = this.providersLazyLoadEvent?.sortOrder === 1 ? 'asc' : 'desc';
        const first = this.providersLazyLoadEvent?.first || 0;
        const pageSize = this.providersLazyLoadEvent?.rows || 10;
        this.applicationsQuery.execute({
            pageIndex: first / pageSize + 1,
            pageSize,
            sortBy,
            sortDirection,
            ...this.filters,
        });
    }

    selectProvider(application: string, mode: PreviewType) {
        this.previewMode = mode;
        this.selectedProvider = application;
    }

    closePreview() {
        this.selectedProvider = null;
        this.loadApplications();
    }

    onServicesFilterDataChange(data: ServicesFilterT) {
        this.servicesFilters = data;
        // always start from the first page when filters are changed
        this.servicesLazyLoadEvent.first = 0;
        if (this.table) {
            this.table.first = 0;
            this.loadServices();
        }
    }

    onServicesLazyLoad(event: LazyLoadEvent) {
        this.servicesLazyLoadEvent = event;
        this.loadServices();
    }

    loadServices() {
        const sortBy = this.servicesLazyLoadEvent?.sortField || 'serviceNumber';
        const sortDirection = this.servicesLazyLoadEvent?.sortOrder === 1 ? 'asc' : 'desc';
        const first = this.servicesLazyLoadEvent?.first || 0;
        const pageSize = this.servicesLazyLoadEvent?.rows || 10;
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
            ...this.servicesFilters,
        });
    }

    selectService(application: ProviderServiceType, mode: PreviewType) {
        this.previewMode = mode;
        this.selectedService = application;
    }

    closeServicesPreview() {
        this.selectedService = null;
        this.loadServices();
    }

    refreshTable(event: any) {
        if (event.index === 1) {
            this.loadServices();
        } else {
            this.loadApplications();
        }
    }

    protected readonly RoleType = RoleType;
}
