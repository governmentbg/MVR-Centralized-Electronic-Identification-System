import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { EidManagementService } from '../../services/eid-management.service';
import { Subscription, first } from 'rxjs';
import { MenuItem, LazyLoadEvent, ConfirmationService } from 'primeng/api';
import { ApplicationStatus, ApplicationTypes, CertificateStatus } from '../../enums/eid-management';
import { Table } from 'primeng/table';
import { CertificatesFilterComponent } from '../../components/certificates-filter/certificates-filter.component';
import { ToastService } from 'src/app/shared/services/toast.service';
import {
    IDevices,
    IEidAdministrator,
    IHideCertificatePreviewEventEmitter,
} from '../../interfaces/eid-management.interface';
import { SortOrder } from '../../enums/eid-management';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { RaeiceiClientService } from '../../services/raeicei-client.service';

@Component({
    selector: 'app-certificates',
    templateUrl: './certificates.component.html',
    styleUrls: ['./certificates.component.scss'],
    providers: [DatePipe, ConfirmationService, CertificatesFilterComponent],
})
export class CertificatesComponent implements OnInit, OnDestroy {
    @ViewChild('dt') table!: Table;
    certificates: any[] = [];
    breadcrumbItems: MenuItem[] = [];
    totalRecords = 0;
    pageSize = 10;
    loading!: boolean;
    lazyLoadEventState: LazyLoadEvent = {};
    certificatesFilterState: any = {};
    subscriptions: Subscription[] = [];
    selectedCertificate: any;
    showPreview = false;
    statusChangeRequest!: boolean;
    administratorsList!: IEidAdministrator[];
    requestInProgress!: boolean;
    devicesList!: IDevices[];
    CertificateStatus = CertificateStatus;
    action = ApplicationTypes;
    constructor(
        public translateService: TranslocoService,
        private eidManagementService: EidManagementService,
        private toastService: ToastService,
        private translocoLocaleService: TranslocoLocaleService,
        private raeiceiService: RaeiceiClientService
    ) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtTitle'),
                    routerLink: '/eid-management',
                },
                { label: this.translateService.translate('modules.eidManagement.txtCertificates') },
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
            { label: this.translateService.translate('modules.eidManagement.txtCertificates') },
        ];
        this.loading = true;
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    showRevokeAttemptMessage() {
        this.toastService.showErrorToast(
            this.translateService.translate('global.txtAttention'),
            this.translateService.translate('modules.eidManagement.errors.txtRevokeAttempt')
        );
    }

    loadCertificates(event: LazyLoadEvent, certificatesFilterData?: any) {
        this.loading = true;
        this.lazyLoadEventState = event;
        let payload: any = {};
        payload.sort = `${event.sortField},${event.sortOrder !== undefined && SortOrder[event.sortOrder]}`;
        payload.page = 0;
        if (certificatesFilterData !== null && certificatesFilterData !== undefined) {
            payload = { ...payload, ...certificatesFilterData };
        } else if (this.certificatesFilterState !== null && Object.keys(this.certificatesFilterState).length > 0) {
            payload = { ...payload, ...this.certificatesFilterState };
        }

        if (event && event.first && event.rows) {
            payload.page = event.first / event.rows;
        }

        payload.size = event.rows || this.pageSize;

        this.eidManagementService
            .getCertificatesList(payload)
            .pipe(first())
            .subscribe({
                next: (certificates: any) => {
                    this.certificates = certificates.content;
                    this.totalRecords = certificates.totalElements;
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

    rowIsNotActive(certificate: any): boolean {
        return (
            certificate.status === ApplicationStatus.DENIED ||
            certificate.status === ApplicationStatus.PENDING_SIGNATURE ||
            (certificate.status === ApplicationStatus.CERTIFICATE_STORED &&
                this.ifDateIsExpired(certificate.validityUntil))
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

    ifUpcoming(certificate: any): boolean {
        let result = false;
        certificate.status.find((sH: any) => {
            if (sH === ApplicationStatus.CERTIFICATE_STORED) {
                result = new Date().getTime() < new Date(certificate.createDate).getTime();
            }
        });
        return result;
    }

    getAdministratorName(adminId: string) {
        const administrator = this.administratorsList?.find((admin: any) => admin.id === adminId);
        return administrator?.name;
    }

    onFilteredDataChange(filteredData: any) {
        this.certificatesFilterState = filteredData;
        // On filter change we always set pagination to first page
        if (this.table.first > 0) {
            this.table.first = 0;
            this.lazyLoadEventState.first = 0;
        }
        this.loadCertificates(this.lazyLoadEventState, filteredData);
    }

    onHideDetails(eventData: IHideCertificatePreviewEventEmitter) {
        if (eventData.refreshTable) {
            this.loadCertificates(this.lazyLoadEventState, this.certificatesFilterState);
        }
        this.showPreview = eventData.showPreview;
        this.statusChangeRequest = eventData.statusChangeRequest;
    }

    showDetails(certificate: any, statusChangeRequest: boolean, action?: string) {
        this.selectedCertificate = certificate;
        if (statusChangeRequest) {
            this.statusChangeRequest = true;
            this.selectedCertificate.action = action;
        }
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

    translateStatus(status: CertificateStatus | null) {
        switch (status) {
            case CertificateStatus.ACTIVE:
                return this.translateService.translate('modules.eidManagement.certificateStatus.txtActive');
            case CertificateStatus.REVOKED:
                return this.translateService.translate('modules.eidManagement.certificateStatus.txtRevoked');
            case CertificateStatus.STOPPED:
                return this.translateService.translate('modules.eidManagement.certificateStatus.txtStopped');
            default:
                return status;
        }
    }

    computedStatus(status: any) {
        return this.translateService.translate('modules.eidManagement.certificateStatus.txt' + status);
    }
}
