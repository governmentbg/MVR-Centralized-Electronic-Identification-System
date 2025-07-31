import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ActivatedRoute, Router } from '@angular/router';
import { MpozeiClientService } from '../../services/mpozei-client.service';
import { retry, Subscription } from 'rxjs';
import { IBreadCrumbItems } from '../../interfaces/breadcrumb.interfaces';
import {
    IApplication,
    ICheckUidRestrictionsResponseData,
    IFindEidentityByNumberAndTypeResponseData,
    IForeignIdentity,
    IHidePreviewEventEmitter,
    INomenclature,
    IPersonalIdentity,
    IReason,
} from '../../interfaces/eid-management.interfaces';
import {
    ApplicationStatus,
    ApplicationType,
    DeviceType,
    NomenclatureNameWithAdditionalReasonField,
    PermittedUser,
    PersonalIdTypes,
    ReasonNamesByAction,
    SubmissionType,
} from '../../enums/eid-management.enum';
import { ToastService } from '../../../../shared/services/toast.service';
import { PivrClientService } from '../../services/pivr-client.service';
import { NomenclatureService } from '../../services/nomenclature.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { EidDeviceService } from '../../services/eid-device.service';
import { ConfirmationDialogService } from '../../../../shared/services/confirmation-dialog.service';
import { RoleType } from '../../../../core/enums/auth.enum';

@Component({
    selector: 'app-application-details',
    templateUrl: './application-details.component.html',
    styleUrls: ['./application-details.component.scss'],
})
export class ApplicationDetailsComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private mpozeiClientService: MpozeiClientService,
        private toastService: ToastService,
        private pivrClientService: PivrClientService,
        private route: ActivatedRoute,
        private nomenclatureService: NomenclatureService,
        private translocoLocaleService: TranslocoLocaleService,
        private eidDeviceService: EidDeviceService,
        private confirmationDialogService: ConfirmationDialogService
    ) {
        this.personalDocumentNumber = sessionStorage.getItem('personalDocumentNumber') || null;
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
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
        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            const state = navigation.extras.state as any;
            if (state) {
                this.personalId = state.personalId;
                this.personalIdType = state.personalIdType;
                this.personalDocumentNumber = state.personalDocumentNumber;
                sessionStorage.setItem('personalDocumentNumber', state.personalDocumentNumber);
                if (state.personalDocumentNumber === null || state.personalIdType === PersonalIdTypes.LNCH) {
                    sessionStorage.removeItem('personalDocumentNumber');
                }
            }
        }
    }

    @Output() hide: EventEmitter<IHidePreviewEventEmitter> = new EventEmitter<IHidePreviewEventEmitter>();
    @Input() applicationId!: string;
    @Input() personalId!: string;
    @Input() personalIdType!: PersonalIdTypes;
    @Input() personalIdentity: IPersonalIdentity | IForeignIdentity | null = null;
    @Input() currentEid: IFindEidentityByNumberAndTypeResponseData | null = null;
    @Input() restrictionsStatus!: ICheckUidRestrictionsResponseData;
    @Input() hideActionButtons = false;

    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    application!: IApplication;
    loading = false;
    routeSubscription!: Subscription;
    INTERVAL = 10000;
    retryCountIfServerError = 3;
    loadRegixDataSubscription: Subscription = new Subscription();
    loadingRegixData = false;
    shouldLoadRegixData = false;
    showDenyDialog = false;
    originalReasons: IReason[] = [];
    reasons: INomenclature[] = [];
    customReasonNames = Object.keys(NomenclatureNameWithAdditionalReasonField);
    showCustomReasonField = false;
    form: FormGroup<{ reason: FormControl<INomenclature | null>; customReason: FormControl<string | null> }> =
        new FormGroup({
            reason: new FormControl<INomenclature | null>(null, Validators.required),
            customReason: new FormControl<string | null>(null, Validators.required),
        });
    ApplicationStatus = ApplicationStatus;
    originalReasonName: string | undefined;
    SubmissionType = SubmissionType;
    personalDocumentNumber: string | null = null;
    submissionTypes: { id: SubmissionType; name: string }[] = [];

    ngOnInit() {
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
                    label: this.translateService.translate('modules.eidManagement.details.txtPreviewApplication'),
                },
            ];

            this.buildReasonsDropdownData();
        });

        this.routeSubscription = this.route.params.subscribe(params => {
            if (params['id']) {
                this.applicationId = params['id'];
                this.shouldLoadRegixData = true;
            }
        });

        this.loading = true;
        this.mpozeiClientService.getApplicationById({ id: this.applicationId }).subscribe({
            next: response => {
                this.application = response;
                this.nomenclatureService.getReasons().subscribe({
                    next: response => {
                        this.originalReasons = response;
                        this.buildReasonsDropdownData();

                        const nomenclatures = this.originalReasons.flatMap(({ nomenclatures }) => nomenclatures);
                        this.originalReasonName = nomenclatures.find(nomenclature => {
                            return nomenclature.id === this.application.reasonId;
                        })?.name;
                    },
                });

                if (this.shouldLoadRegixData) {
                    this.mpozeiClientService.getEidentityById(this.application.eidentityId).subscribe({
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
                }
                this.loading = false;
            },
            error: () => {
                this.loading = false;
                this.showErrorToast(this.translateService.translate('global.txtCertificateLoadError'));
                this.navigateToSearch();
            },
        });
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
        this.routeSubscription.unsubscribe();
    }

    formatDate(date: string | null | undefined, includeHourAndMinutes = true): string {
        if (!date) {
            return '';
        }

        if (includeHourAndMinutes) {
            return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
                timeStyle: 'medium',
            });
        } else {
            return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
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

    applicationDeviceTypeTranslation(deviceId: string) {
        return this.eidDeviceService.getDeviceTranslation(deviceId);
    }

    get permanentAddress() {
        if (this.personalIdentity && this.personalIdentity.permanentAddress) {
            const locationName = this.personalIdentity.permanentAddress.locationName;
            const buildingNumber = this.personalIdentity.permanentAddress.buildingNumber;
            const entrance = this.personalIdentity.permanentAddress.entrance;
            const floor = this.personalIdentity.permanentAddress.floor;
            const apartment = this.personalIdentity.permanentAddress.apartment;
            const settlementCode = this.personalIdentity.permanentAddress.settlementCode;
            const districtName = this.personalIdentity.permanentAddress.districtName;
            const settlementName = this.personalIdentity.permanentAddress.settlementName;
            const municipalityName = this.personalIdentity.permanentAddress.municipalityName;

            const addressLines = [];

            if (locationName && buildingNumber) {
                addressLines.push(locationName + ' №' + buildingNumber + ', ');
            } else if (locationName) {
                addressLines.push(locationName + ', ');
            }

            const additionalInfo = [];
            if (entrance) {
                additionalInfo.push(
                    `${this.translateService.translate('modules.eidManagement.txtBuilding')} ` + entrance
                );
            }
            if (floor) {
                additionalInfo.push(`${this.translateService.translate('modules.eidManagement.txtFloor')} ` + floor);
            }

            if (apartment) {
                additionalInfo.push(
                    `${this.translateService.translate('modules.eidManagement.txtApartment')} ` + apartment
                );
            }

            if (additionalInfo.length > 0) {
                addressLines.push(additionalInfo.join(', ') + ', ');
            }

            if (settlementCode && settlementName) {
                addressLines.push(settlementCode + ' ' + settlementName + ', ');
            } else if (settlementName) {
                addressLines.push(settlementName + ', ');
            }

            const locationInfo = [];
            if (districtName) {
                locationInfo.push(districtName);
            }
            if (municipalityName) {
                locationInfo.push(municipalityName);
            }
            if (locationInfo.length > 0) {
                addressLines.push(locationInfo.join(', '));
            }

            return addressLines.join('');
        } else {
            return this.translateService.translate('modules.eidManagement.txtNoData');
        }
    }

    navigateToSearch() {
        this.router.navigate(['/eid-management']);
    }

    navigateToSearchResults() {
        this.closePreview();
    }

    closePreview() {
        if (this.shouldLoadRegixData) {
            this.router.navigate(['/eid-management/']);
        } else {
            this.hide.emit({ showPreview: false, refreshTable: true });
        }
    }

    goToApplicationAction() {
        switch (this.application.applicationType) {
            case ApplicationType.ISSUE_EID:
                this.router.navigate(['/eid-management/search/new-application'], {
                    state: {
                        personalId: this.personalId,
                        personalIdType: this.personalIdType,
                        applicationId: this.application.id,
                        currentEid: this.currentEid,
                        restrictionsStatus: this.restrictionsStatus,
                        application: this.application,
                        personalDocumentNumber: this.personalDocumentNumber,
                    },
                });
                break;
            case ApplicationType.STOP_EID:
            case ApplicationType.REVOKE_EID:
            case ApplicationType.RESUME_EID:
                if (this.shouldLoadRegixData) {
                    this.router.navigate([`/eid-management/certificate/${this.application.certificateId}`], {
                        state: {
                            personalId: this.personalId,
                            personalIdType: this.personalIdType,
                            currentEid: this.currentEid,
                            applicationId: this.application.id,
                            restrictionsStatus: this.restrictionsStatus,
                            action: this.application.applicationType,
                            personalDocumentNumber: this.personalDocumentNumber,
                        },
                    });
                    break;
                } else {
                    this.hide.emit({
                        showPreview: false,
                        requestedAction: {
                            name: this.application.applicationType,
                            context: {
                                certificateId: this.application.certificateId,
                                applicationId: this.application.id,
                                personalDocumentNumber: this.personalDocumentNumber,
                            },
                        },
                    });
                    break;
                }
        }
    }

    denyApplication() {
        this.showDenyDialog = true;
    }

    openCertificate(id: string) {
        const url = this.router.serializeUrl(this.router.createUrlTree([`/eid-management/certificate/${id}`]));
        window.open(url, '_blank');
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

                        this.confirmationDialogService.showConfirmation({
                            message: this.translateService.translate(
                                'modules.eidManagement.txtFailedLoadingPersonalData'
                            ),
                            header: this.translateService.translate('global.txtConfirmation'),
                            icon: 'pi pi-exclamation-triangle',
                            rejectVisible: false,
                            acceptLabel: this.translateService.translate('global.txtOk'),
                            accept: () => {
                                this.loading = false;
                                this.loadingRegixData = false;
                            },
                        });
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

                        this.confirmationDialogService.showConfirmation({
                            message: this.translateService.translate(
                                'modules.eidManagement.txtFailedLoadingPersonalData'
                            ),
                            header: this.translateService.translate('global.txtConfirmation'),
                            icon: 'pi pi-exclamation-triangle',
                            rejectVisible: false,
                            acceptLabel: this.translateService.translate('global.txtOk'),
                            accept: () => {
                                this.loading = false;
                                this.loadingRegixData = false;
                            },
                        });
                    },
                });
        }
    }

    onDataSubmit() {
        this.form.markAllAsTouched();
        Object.values(this.form.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    innerControl.markAsDirty();
                });
            } else {
                control.markAsDirty();
            }
        });
        if (this.form.valid) {
            this.loading = true;
            const denyData: { applicationId: string; reason: { reasonId: string; reasonText?: string } } = {
                applicationId: this.application.id,
                reason: {
                    reasonId: this.form.controls.reason.value?.id as string,
                },
            };
            if (this.form.controls.customReason.value) {
                denyData.reason['reasonText'] = this.form.controls.customReason.value;
            }
            this.mpozeiClientService.denyApplication(denyData).subscribe({
                next: () => {
                    this.showDenyDialog = false;
                    this.loading = false;
                    this.showSuccessToast(
                        this.translateService.translate('modules.eidManagement.txtApplicationDenySuccess')
                    );
                    this.hide.emit({ showPreview: false, refreshTable: true });
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
        }
    }

    buildReasonsDropdownData(): void {
        const foundReason = this.originalReasons.find(reason => reason.name === ReasonNamesByAction.DENY_APPLICATION);
        if (foundReason) {
            this.reasons = foundReason.nomenclatures.filter(nomenclature => {
                return (
                    nomenclature.language === this.translateService.getActiveLang() &&
                    nomenclature.permittedUser === PermittedUser.ADMIN
                );
            });
        }
    }

    onReasonChange(
        formName: FormGroup<{ reason: FormControl<INomenclature | null>; customReason: FormControl<string | null> }>
    ) {
        if (formName.controls['reason'].value) {
            const customReason = this.customReasonNames.indexOf(formName.controls['reason'].value?.name) > -1;
            if (customReason) {
                this.showCustomReasonField = true;
                formName.controls['customReason'].setValidators([Validators.required]);
            } else {
                this.showCustomReasonField = false;
                formName.controls['customReason'].reset();
                formName.controls['customReason'].setValidators(null);
            }
            formName.controls['customReason'].updateValueAndValidity();
        }
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    getReasonTranslation(reasonId: string | null, reasonText?: string | null) {
        if (reasonId) {
            const foundReason = this.reasons.find(reason => reason.id === reasonId);

            if (foundReason) {
                return reasonText || foundReason.description;
            } else {
                const nomenclatures = this.originalReasons.flatMap(({ nomenclatures }) => nomenclatures);
                const foundReasonByName = nomenclatures.find(
                    nom =>
                        nom.name === this.originalReasonName && nom.language === this.translateService.getActiveLang()
                );
                return reasonText || foundReasonByName?.description;
            }
        } else {
            return this.translateService.translate('modules.eidManagement.txtNoData');
        }
    }

    getIssuerName() {
        if (this.personalIdentity) {
            if ('issuerPlace' in this.personalIdentity) {
                return `${this.personalIdentity.issuerName}-${this.personalIdentity.issuerPlace}`;
            } else if ('issuePlace' in this.personalIdentity) {
                return `${this.personalIdentity.issuerName}-${this.personalIdentity.issuePlace}`;
            }
        }
        return this.translateService.translate('modules.eidManagement.txtNoData');
    }

    reasonLabelText() {
        switch (this.application.applicationType) {
            case ApplicationType.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.details.txtReasonForTermination');
            case ApplicationType.STOP_EID:
                return this.translateService.translate('modules.eidManagement.details.txtReasonForStopping');
            case ApplicationType.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.details.txtReasonForStarting');
            default:
                return this.translateService.translate('modules.eidManagement.details.txtReason');
        }
    }

    continueApplicationProcessText() {
        const foundDevice = this.eidDeviceService.getDeviceById(this.application.deviceId);
        switch (this.application.applicationType) {
            case ApplicationType.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.txtContinueApplicationResume');
            case ApplicationType.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.txtContinueApplicationRevoke');
            case ApplicationType.STOP_EID:
                return this.translateService.translate('modules.eidManagement.txtContinueApplicationStop');
            case ApplicationType.ISSUE_EID:
                if (
                    this.application.status === ApplicationStatus.GENERATED_CERTIFICATE &&
                    foundDevice?.type === DeviceType.MOBILE
                ) {
                    return this.translateService.translate('modules.eidManagement.txtRestartCertificateGeneration');
                } else {
                    return this.translateService.translate('modules.eidManagement.txtContinueApplicationCreation');
                }
        }
    }

    getApplicationTypeText(type: ApplicationType) {
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

    isApplicationDeviceTypeMobileApp() {
        const foundDevice = this.eidDeviceService.getDeviceById(this.application.deviceId);
        return foundDevice ? foundDevice.type === DeviceType.MOBILE : false;
    }

    isContinueApplicationProcessAvailable() {
        return (
            this.application.submissionType === SubmissionType.DESK ||
            this.application.submissionType === SubmissionType.BASE_PROFILE ||
            (this.application.submissionType === SubmissionType.EID &&
                !this.isApplicationDeviceTypeMobileApp() &&
                this.application.status !== ApplicationStatus.PENDING_PAYMENT)
        );
    }

    submissionTypeTranslation(submissionType: SubmissionType) {
        return this.submissionTypes.find(type => type.id === submissionType)?.name || '';
    }

    protected readonly RoleType = RoleType;
}
