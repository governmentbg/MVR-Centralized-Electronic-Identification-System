import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ActivatedRoute, Router } from '@angular/router';
import { MpozeiClientService } from '../../services/mpozei-client.service';
import {
    ICarrier,
    ICertificate,
    ICheckUidRestrictionsResponseData,
    IFindEidentityByNumberAndTypeResponseData,
    IForeignIdentity,
    IHidePreviewEventEmitter,
    IPersonalIdentity,
} from '../../interfaces/eid-management.interfaces';
import { retry, Subscription } from 'rxjs';
import { IBreadCrumbItems } from '../../interfaces/breadcrumb.interfaces';
import { ApplicationType, CertificateStatus, PersonalIdTypes } from '../../enums/eid-management.enum';
import { ToastService } from '../../../../shared/services/toast.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { EidDeviceService } from '../../services/eid-device.service';
import { ConfirmationDialogService } from '../../../../shared/services/confirmation-dialog.service';
import * as moment from 'moment/moment';
import { PivrClientService } from '../../services/pivr-client.service';
import { PunClientService } from '../../services/pun-client.service';

@Component({
    selector: 'app-certificate-details',
    templateUrl: './certificate-details.component.html',
    styleUrls: ['./certificate-details.component.scss'],
})
export class CertificateDetailsComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private mpozeiClientService: MpozeiClientService,
        private toastService: ToastService,
        private pivrClientService: PivrClientService,
        private route: ActivatedRoute,
        private translocoLocaleService: TranslocoLocaleService,
        private eidDeviceService: EidDeviceService,
        private confirmationDialogService: ConfirmationDialogService,
        private punClientService: PunClientService
    ) {
        this.personalDocumentNumber = sessionStorage.getItem('personalDocumentNumber') || null;
        const navigation = this.router.getCurrentNavigation();

        if (navigation) {
            const state = navigation.extras.state as any;
            if (state) {
                this.personalId = state.personalId;
                this.personalIdType = state.personalIdType;
                this.applicationId = state.applicationId;
                this.restrictionsStatus = state.restrictionsStatus;
                this.action = state.action;
                this.personalDocumentNumber = state.personalDocumentNumber;
                sessionStorage.setItem('personalDocumentNumber', state.personalDocumentNumber);
                if (state.personalDocumentNumber === null || state.personalIdType === PersonalIdTypes.LNCH) {
                    sessionStorage.removeItem('personalDocumentNumber');
                }
            }
        }
    }

    @Output() hide: EventEmitter<IHidePreviewEventEmitter> = new EventEmitter<IHidePreviewEventEmitter>();
    @Input() certificateId!: string;
    @Input() personalId!: string;
    @Input() personalIdType!: PersonalIdTypes;
    @Input() personalIdentity!: IPersonalIdentity | IForeignIdentity | null;
    @Input() action: ApplicationType | null = null;
    @Input() currentEid!: IFindEidentityByNumberAndTypeResponseData | null;
    @Input() restrictionsStatus!: ICheckUidRestrictionsResponseData;
    @Input() applicationId: string | null = null;

    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    certificate!: ICertificate;
    loading = false;
    form = new FormGroup({
        reason: new FormControl<string | null>(null, Validators.required),
    });
    isCertificateHistoryVisible = false;
    routeSubscription!: Subscription;
    INTERVAL = 10000;
    retryCountIfServerError = 3;
    loadRegixDataSubscription: Subscription = new Subscription();
    loadingRegixData!: boolean;
    shouldLoadRegixData = false;
    personalDocumentNumber: string | null = null;
    CertificateStatus = CertificateStatus;
    carrierLoading = false;
    carrier: ICarrier | null = null;

    ngOnInit() {
        this.loading = true;
        this.carrierLoading = true;

        this.languageChangeSubscription = this.translateService.langChanges$.subscribe(() => {
            let breadcrumbs: IBreadCrumbItems[] = [];
            if (this.router.url === '/eid-management/search/results') {
                breadcrumbs = [
                    {
                        label: this.translateService.translate('modules.eidManagement.txtSearchSubtitle'),
                        onClick: this.navigateToSearch.bind(this),
                    },
                    {
                        label: this.translateService.translate('modules.eidManagement.txtSearchResults'),
                        onClick: this.navigateToSearchResults.bind(this),
                    },
                ];
            } else if (this.router.url === '/eid-management/applications-list') {
                breadcrumbs = [
                    {
                        label: this.translateService.translate('modules.eidManagement.txtApplicationsListTitle'),
                        onClick: this.closePreview.bind(this),
                    },
                ];
            }
            this.breadcrumbItems = [
                ...breadcrumbs,
                {
                    label: this.translateService.translate('modules.eidManagement.details.txtPreviewCertificate'),
                },
            ];
        });

        this.routeSubscription = this.route.params.subscribe(params => {
            if (params['id']) {
                this.certificateId = params['id'];
                this.shouldLoadRegixData = true;
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.eidManagement.details.txtPreviewCertificate'),
                    },
                ];
            }
        });
        this.mpozeiClientService.getCertificateById({ id: this.certificateId }).subscribe({
            next: response => {
                this.certificate = response;
                if (this.shouldLoadRegixData) {
                    this.loading = true;
                    this.mpozeiClientService.getEidentityById(this.certificate.eidentityId).subscribe({
                        next: response => {
                            this.currentEid = response;
                            this.personalIdType = response.citizenIdentifierType as PersonalIdTypes;
                            this.personalId = response.citizenIdentifierNumber as string;
                            this.getPersonalIdentity();
                        },
                        error: (error: any) => {
                            this.loading = false;
                            switch (error.status) {
                                case 400:
                                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                    break;
                                default:
                                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                                    break;
                            }
                        },
                    });
                } else {
                    this.loading = false;
                }
            },
            error: () => {
                this.loading = false;
                this.showErrorToast(this.translateService.translate('global.txtCertificateLoadError'));
                this.navigateToSearch();
            },
        });
        this.punClientService.getCarriers({ certificateId: this.certificateId }).subscribe({
            next: response => {
                this.carrierLoading = false;
                this.carrier = response[0];
            },
            error: (error: any) => {
                this.carrierLoading = false;
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                        break;
                }
            },
        });
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
        this.routeSubscription.unsubscribe();
    }

    formatDate(date: string | null): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
            timeStyle: 'medium',
        });
    }

    deviceTypeTranslation(deviceId: string) {
        return this.eidDeviceService.getDeviceTranslation(deviceId);
    }

    certificateStatusTranslation(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.ACTIVE:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Active');
            case CertificateStatus.STOPPED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Stopped');
            case CertificateStatus.REVOKED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Revoked');
            case CertificateStatus.CREATED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Created');
            case CertificateStatus.EXPIRED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Expired');
            case CertificateStatus.FAILED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Failed');
        }
    }

    personalIdTypesTranslation(type: PersonalIdTypes) {
        switch (type) {
            case PersonalIdTypes.EGN:
                return this.translateService.translate('modules.eidManagement.txtEGN');
            case PersonalIdTypes.LNCH:
                return this.translateService.translate('modules.eidManagement.txtLNCH');
        }
    }

    navigateToSearch() {
        if (this.action) {
            this.confirmationDialogService.showConfirmation({
                accept: () => {
                    this.router.navigate(['/eid-management']);
                },
            });
        } else {
            this.router.navigate(['/eid-management']);
        }
    }

    navigateToSearchResults() {
        if (this.action) {
            this.confirmationDialogService.showConfirmation({
                accept: () => {
                    this.closePreview();
                },
            });
        } else {
            this.closePreview();
        }
    }

    closePreview() {
        if (this.shouldLoadRegixData) {
            this.router.navigate(['/eid-management/']);
        } else {
            this.hide.emit({ showPreview: false, refreshTable: true });
        }
    }

    closePreviewAndRefresh() {
        if (this.shouldLoadRegixData) {
            this.navigateToSearch();
        } else {
            this.hide.emit({ showPreview: false, refreshTable: true });
        }
    }

    closeCertificateHistory(eventData: IHidePreviewEventEmitter) {
        if (eventData.refreshTable) {
            // We don't really want to refresh the table, we only use the refreshTable prop
            // to know when the breadcrumb inside the history component is clicked
            this.closePreview();
        } else {
            this.isCertificateHistoryVisible = false;
        }
    }

    certificateTitleText() {
        switch (this.action) {
            case ApplicationType.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.details.txtApplicationForTermination');
            case ApplicationType.STOP_EID:
                return this.translateService.translate('modules.eidManagement.details.txtApplicationForStopping');
            case ApplicationType.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.details.txtApplicationForStarting');
            default:
                return this.translateService.translate('modules.eidManagement.details.txtPreviewCertificate');
        }
    }

    showCertificateHistory() {
        this.isCertificateHistoryVisible = true;
    }

    getPersonalIdentity() {
        this.loadingRegixData = true;
        if (this.personalIdType === PersonalIdTypes.EGN && this.personalDocumentNumber) {
            this.loadRegixDataSubscription = this.pivrClientService
                .getPersonalIdentityV2({ EGN: this.personalId, IdentityDocumentNumber: this.personalDocumentNumber })
                .pipe(
                    retry({
                        count: this.retryCountIfServerError,
                        delay: this.INTERVAL,
                    })
                )
                .subscribe({
                    next: response => {
                        this.loading = false;
                        this.loadingRegixData = false;
                        this.personalIdentity = response.response.PersonalIdentityInfoResponse;
                    },
                    error: (error: any) => {
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            default:
                                this.showErrorToast(this.translateService.translate('global.txtNoConnectionWithRegix'));
                                break;
                        }
                    },
                });
        } else {
            this.loadRegixDataSubscription = this.pivrClientService
                .getForeignIdentityV2({ Identifier: this.personalId, IdentifierType: 'LNCh' })
                .pipe(
                    retry({
                        count: this.retryCountIfServerError,
                        delay: this.INTERVAL,
                    })
                )
                .subscribe({
                    next: response => {
                        this.loading = false;
                        this.loadingRegixData = false;
                        this.personalIdentity = response.response.ForeignIdentityInfoResponse;
                    },
                    error: (error: any) => {
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            default:
                                this.showErrorToast(this.translateService.translate('global.txtNoConnectionWithRegix'));
                                break;
                        }
                    },
                });
        }
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    isCertificateExpired(certificate: ICertificate) {
        const date = moment.utc(certificate.validityUntil);
        const now = moment.utc();

        return now > date;
    }

    printCertificate() {
        window.print();
    }
}
