import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { EidManagementService } from '../../services/eid-management.service';
import { Subscription, first } from 'rxjs';
import { MenuItem, LazyLoadEvent, ConfirmationService } from 'primeng/api';
import { SortOrder, SubmissionTypes } from '../../enums/eid-management';
import { ApplicationStatus, ApplicationTypes } from '../../enums/eid-management';
import { ApplicationsFilterComponent } from '../../components/applications-filter/applications-filter.component';
import { Table } from 'primeng/table';
import { IHidePreviewEventEmitter } from 'src/app/features/authorization-register/interfaces/authorization-register.interfaces';
import { ApplicationPreviewComponent } from '../../components/application-preview/application-preview.component';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { ToastService } from 'src/app/shared/services/toast.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { IEidAdministrator, IDevices, Application } from '../../interfaces/eid-management.interface';
import { RaeiceiClientService } from '../../services/raeicei-client.service';
import { AppConfigService } from 'src/app/core/services/config.service';
@Component({
    selector: 'app-applications',
    templateUrl: './applications.component.html',
    styleUrls: ['./applications.component.scss'],
    providers: [DatePipe, ConfirmationService, ApplicationsFilterComponent, ApplicationPreviewComponent],
})
export class ApplicationsComponent implements OnInit, OnDestroy {
    @ViewChild('dt') table!: Table;
    applications: any[] = [];
    breadcrumbItems: MenuItem[] = [];
    totalRecords = 0;
    pageSize = 10;
    loading!: boolean;
    lazyLoadEventState: LazyLoadEvent = {};
    applicationsFilterState: any = {};
    subscriptions: Subscription[] = [];
    selectedApplication: any;
    showPreview = false;
    paymentUrl = this.appConfigService.config.externalLinks.paymentByAccessCodeUrl;
    administratorsList!: IEidAdministrator[];
    devicesList!: IDevices[];
    constructor(
        public translateService: TranslocoService,
        private eidManagementService: EidManagementService,
        private toastService: ToastService,
        private raeiceiService: RaeiceiClientService,
        private translocoLocaleService: TranslocoLocaleService,
        private appConfigService: AppConfigService
    ) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtTitle'),
                    routerLink: '/eid-management',
                },
                { label: this.translateService.translate('modules.eidManagement.txtApplications') },
            ];
        });
        this.subscriptions.push(languageChangeSubscription);
    }
    ngOnInit(): void {
        const administrators = this.raeiceiService
            .fetchAdministrators()
            .subscribe((administrators: IEidAdministrator[]) => {
                this.administratorsList = administrators;
            });
        this.subscriptions.push(administrators);
        const devices = this.raeiceiService.fetchDevices().subscribe((devices: IDevices[]) => {
            this.devicesList = devices;
        });
        this.subscriptions.push(devices);
        this.breadcrumbItems = [
            {
                label: this.translateService.translate('modules.eidManagement.txtTitle'),
                routerLink: '/eid-management',
            },
            { label: this.translateService.translate('modules.eidManagement.txtApplications') },
        ];
        this.loading = true;
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    loadApplications(event: LazyLoadEvent, applicationsFilterData?: any) {
        this.loading = true;
        this.lazyLoadEventState = event;
        let payload: any = {};
        payload.sort = `${event.sortField},${event.sortOrder !== undefined && SortOrder[event.sortOrder]}`;
        payload.page = 0;
        if (applicationsFilterData !== null && applicationsFilterData !== undefined) {
            payload = { ...payload, ...applicationsFilterData };
        } else if (this.applicationsFilterState !== null && Object.keys(this.applicationsFilterState).length > 0) {
            payload = { ...payload, ...this.applicationsFilterState };
        }

        if (event && event.first && event.rows) {
            payload.page = event.first / event.rows;
        }
        payload.size = event.rows || this.pageSize;

        this.eidManagementService
            .getApplicationsList(payload)
            .pipe(first())
            .subscribe({
                next: (applications: any) => {
                    this.applications = applications.content;
                    this.totalRecords = applications.totalElements;
                    this.loading = false;
                },
                error: err => {
                    let showDefaultError = true;
                    err.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.toastService.showErrorToast(
                                this.translateService.translate('global.txtErrorTitle'),
                                this.translateService.translate('errors.' + formatError(el))
                            );
                            showDefaultError = false;
                        }
                    });
                    if (showDefaultError) {
                        this.toastService.showErrorToast(
                            this.translateService.translate('global.txtErrorTitle'),
                            this.translateService.translate('global.txtUnexpectedError')
                        );
                    }
                    this.loading = false;
                },
            });
    }

    rowIsNotActive(application: any): boolean {
        return (
            application.status === ApplicationStatus.DENIED ||
            application.status === ApplicationStatus.PENDING_SIGNATURE ||
            (application.status === ApplicationStatus.CERTIFICATE_STORED &&
                this.ifDateIsExpired(application.expiryDate))
        );
    }

    ifDateIsExpired(date: Date): boolean {
        if (!date) {
            return false;
        }
        const todayTimestamp = new Date().getTime();
        const expireDateUtc = new Date(date).getTime();
        return expireDateUtc < todayTimestamp;
    }

    ifUpcoming(application: any): boolean {
        let result = false;
        application.status.find((sH: any) => {
            if (sH === ApplicationStatus.CERTIFICATE_STORED) {
                result = new Date().getTime() < new Date(application.createDate).getTime();
            }
        });
        return result;
    }

    onFilteredDataChange(filteredData: any) {
        this.applicationsFilterState = filteredData;
        // On filter change we always set pagination to first page
        if (this.table.first > 0) {
            this.table.first = 0;
            this.lazyLoadEventState.first = 0;
        }
        this.loadApplications(this.lazyLoadEventState, filteredData);
    }

    onHideDetails(eventData: IHidePreviewEventEmitter) {
        if (eventData.refreshTable) {
            this.loadApplications(this.lazyLoadEventState, this.applicationsFilterState);
        }
        this.showPreview = eventData.showPreview;
    }

    showDetails(application: any) {
        this.selectedApplication = application;
        this.showPreview = true;
    }

    translateApplicationType(appType: ApplicationTypes | null) {
        switch (appType) {
            case ApplicationTypes.ISSUE_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtIssueEID');
            case ApplicationTypes.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtResumeEID');
            case ApplicationTypes.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtRevokeEID');
            case ApplicationTypes.STOP_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtStopEID');
            default:
                return appType;
        }
    }

    translateDeviceType(id: string) {
        const device = this.devicesList?.find((device: any) => device.id === id);
        const language = this.translocoLocaleService.getLocale();
        if (language === 'bg-BG') {
            return device?.name;
        } else {
            return device?.description;
        }
    }

    translateStatus(status: ApplicationStatus | null) {
        switch (status) {
            case ApplicationStatus.CERTIFICATE_STORED:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtSigned');
            case ApplicationStatus.COMPLETED:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtCompleted');
            case ApplicationStatus.DENIED:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtDenied');
            case ApplicationStatus.GENERATED_CERTIFICATE:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtSigned');
            case ApplicationStatus.PAID:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtPaid');
            case ApplicationStatus.PENDING_PAYMENT:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtPendingPayment');
            case ApplicationStatus.SIGNED:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtSigned');
            case ApplicationStatus.SUBMITTED:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtSubmitted');
            case ApplicationStatus.APPROVED:
                return this.translateService.translate('modules.eidManagement.applicationStatus.txtApproved');
            default:
                return status;
        }
    }

    computedStatus(status: any) {
        return this.translateService.translate('modules.eidManagement.applicationStatus.txt' + status);
    }

    redirectToPayment(accessCode: string): void {
        window.open(`${this.paymentUrl}${accessCode}`, '_blank');
    }

    showPaymentButton(application: Application) {
        return (
            application.status === ApplicationStatus.PENDING_PAYMENT &&
            application.submissionType !== SubmissionTypes.DESK
        );
    }
}
