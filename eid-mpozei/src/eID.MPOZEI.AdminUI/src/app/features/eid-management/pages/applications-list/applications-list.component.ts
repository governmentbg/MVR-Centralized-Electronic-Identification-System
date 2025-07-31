import { Component, OnInit, ViewChild } from '@angular/core';
import { ApplicationStatus, ApplicationType, PersonalIdTypes, SubmissionType } from '../../enums/eid-management.enum';
import { IBreadCrumbItems } from '../../interfaces/breadcrumb.interfaces';
import {
    IApplication,
    ICheckUidRestrictionsResponseData,
    IFindEidentityByNumberAndTypeResponseData,
    IForeignIdentity,
    IHidePreviewEventEmitter,
    IPersonalIdentity,
} from '../../interfaces/eid-management.interfaces';
import { LazyLoadEvent } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { MpozeiClientService } from '../../services/mpozei-client.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { EidDeviceService } from '../../services/eid-device.service';
import { Subscription } from 'rxjs';
import { Table } from 'primeng/table';
import { EidAdministratorService } from '../../services/eid-administrator.service';

@Component({
    selector: 'app-applications-list',
    templateUrl: './applications-list.component.html',
    styleUrls: ['./applications-list.component.scss'],
})
export class ApplicationsListComponent implements OnInit {
    constructor(
        public translateService: TranslocoService,
        private mpozeiClientService: MpozeiClientService,
        private toastService: ToastService,
        private translocoLocaleService: TranslocoLocaleService,
        private eidDeviceService: EidDeviceService,
        private eidAdministratorService: EidAdministratorService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtApplicationsListTitle'),
                },
            ];
            this.personalIdTypes = [
                {
                    name: this.translateService.translate('modules.eidManagement.txtEGN'),
                    id: PersonalIdTypes.EGN,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.txtLNCH'),
                    id: PersonalIdTypes.LNCH,
                },
            ];
            this.submissionTypes = [
                {
                    id: SubmissionType.DESK,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.DESK}`
                    ),
                },
                {
                    id: SubmissionType.EID,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.EID}`
                    ),
                },
                {
                    id: SubmissionType.BASE_PROFILE,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.BASE_PROFILE}`
                    ),
                },
                {
                    id: SubmissionType.PERSO_CENTRE,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.PERSO_CENTRE}`
                    ),
                },
            ];
        });
    }

    @ViewChild('dt') table!: Table;
    languageChangeSubscription: Subscription = new Subscription();
    protected readonly ApplicationStatus = ApplicationStatus;
    breadcrumbItems: IBreadCrumbItems[] = [];
    applications: IApplication[] = [];
    applicationsTotalRecords = 0;
    applicationsPageSize = 10;
    loadingApplications!: boolean;
    lazyLoadApplicationsEventState: LazyLoadEvent = {};
    applicationsFilterState: any = {};
    showApplicationDetails = false;
    selectedApplication!: IApplication;
    personalId: any;
    personalIdType!: PersonalIdTypes;
    restrictionsStatus: ICheckUidRestrictionsResponseData = {
        response: { isProhibited: false, isDead: false, hasRevokedParentalRights: false },
        hasFailed: false,
        error: '',
    };
    currentEid: IFindEidentityByNumberAndTypeResponseData | null = null;
    personalIdentity: IPersonalIdentity | IForeignIdentity | null = null;
    personalIdTypes = [
        {
            name: this.translateService.translate('modules.eidManagement.txtEGN'),
            id: PersonalIdTypes.EGN,
        },
        {
            name: this.translateService.translate('modules.eidManagement.txtLNCH'),
            id: PersonalIdTypes.LNCH,
        },
    ];
    submissionTypes: { id: SubmissionType; name: string }[] = [];

    ngOnInit() {
        this.loadingApplications = true;
    }

    loadApplications(event: LazyLoadEvent, applicationsFilterData?: any): void {
        this.loadingApplications = true;
        this.lazyLoadApplicationsEventState = event;
        this.applicationsTotalRecords = 0;

        const sortField = event.sortField || 'createDate';
        const sortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
        let payload = {
            page: 0,
            size: event.rows || this.applicationsPageSize,
            sort: `${sortField},${sortOrder}`,
        };

        if (applicationsFilterData !== null && applicationsFilterData !== undefined) {
            payload = { ...payload, ...applicationsFilterData };
        } else if (this.applicationsFilterState !== null && Object.keys(this.applicationsFilterState).length > 0) {
            payload = { ...payload, ...this.applicationsFilterState };
        }

        if (event && event.first && event.rows) {
            payload.page = event.first / event.rows;
        }

        this.mpozeiClientService.getAllApplications(payload).subscribe({
            next: response => {
                this.applications = response.content;
                this.applicationsTotalRecords = response.totalElements;
                this.loadingApplications = false;
            },
            error: (error: any) => {
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtNoConnectionApplication'));
                        break;
                }
                this.loadingApplications = false;
            },
        });
    }

    showDetails(application: any) {
        this.selectedApplication = application;
        this.showApplicationDetails = true;
    }

    applicationDeviceTypeTranslation(deviceId: string) {
        return this.eidDeviceService.getDeviceTranslation(deviceId);
    }

    applicationStatusTranslation(status: ApplicationStatus) {
        switch (status) {
            case ApplicationStatus.COMPLETED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Completed');
            case ApplicationStatus.SUBMITTED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Submitted');
            case ApplicationStatus.SIGNED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Signed');
            case ApplicationStatus.APPROVED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Approved');
            case ApplicationStatus.DENIED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Denied');
            case ApplicationStatus.GENERATED_CERTIFICATE:
                return this.translateService.translate(
                    'modules.eidManagement.tableFilter.statuses.GeneratedCertificate'
                );
            case ApplicationStatus.PAID:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Paid');
            case ApplicationStatus.PENDING_PAYMENT:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.PendingPayment');
            case ApplicationStatus.PENDING_SIGNATURE:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.PendingSignature');
            case ApplicationStatus.PROCESSING:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Processing');
            case ApplicationStatus.CERTIFICATE_STORED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.CertificateStored');
            case ApplicationStatus.PAYMENT_EXPIRED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.PaymentExpired');
        }
    }

    applicationTypeText(type: ApplicationType) {
        switch (type) {
            case ApplicationType.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.txtApplicationTypeRevoke');
            case ApplicationType.STOP_EID:
                return this.translateService.translate('modules.eidManagement.txtApplicationTypeStop');
            case ApplicationType.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.txtApplicationTypeResume');
            case ApplicationType.ISSUE_EID:
                return this.translateService.translate('modules.eidManagement.txtApplicationTypeIssue');
        }
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    formatDate(date: string | undefined): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
            timeStyle: 'medium',
        });
    }

    onHideApplicationDetails(eventData: IHidePreviewEventEmitter) {
        if (eventData.refreshTable) {
            this.loadApplications(this.lazyLoadApplicationsEventState, this.applicationsFilterState);
        }
        this.showApplicationDetails = eventData.showPreview;
    }

    onFilteredApplicationsDataChange(filteredData: any) {
        this.applicationsFilterState = filteredData;
        // On filter change we always set pagination to first page
        if (this.table) {
            if (this.table.first > 0) {
                this.table.first = 0;
                this.lazyLoadApplicationsEventState.first = 0;
            }
            this.loadApplications(this.lazyLoadApplicationsEventState, filteredData);
        }
    }

    onExport(filteredData: any) {
        this.loadingApplications = true;
        this.mpozeiClientService.exportAdminApplications(filteredData).subscribe({
            next: response => {
                const BOM = '\uFEFF';
                const blob = new Blob([BOM + response], { type: 'text/csv;charset=utf-8' });
                const downloadUrl = window.URL.createObjectURL(blob);
                const anchor = document.createElement('a');
                anchor.href = downloadUrl;
                anchor.download = `export.csv`;
                anchor.click();
                this.loadingApplications = false;
            },
            error: (error: any) => {
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                        break;
                }
                this.loadingApplications = false;
            },
        });
    }

    getOfficeName(officeId: string) {
        return this.eidAdministratorService.getOfficeNameById(officeId);
    }

    submissionTypeTranslation(submissionType: SubmissionType) {
        return this.submissionTypes.find(type => type.id === submissionType)?.name || '';
    }
}
