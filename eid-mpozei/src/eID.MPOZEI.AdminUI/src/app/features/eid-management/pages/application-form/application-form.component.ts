import { Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import { retry, Subject, Subscription, switchMap, timer } from 'rxjs';
import { IBreadCrumbItems } from '../../interfaces/breadcrumb.interfaces';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import {
    ApplicationStatus,
    ApplicationType,
    DeviceType,
    LevelOfAssurance,
    NomenclatureNameWithAdditionalReasonField,
    PermittedUser,
    PersonalIdTypes,
    ReasonNamesByAction,
    SubmissionType,
} from '../../enums/eid-management.enum';
import { PivrClientService } from '../../services/pivr-client.service';
import { MpozeiClientService } from '../../services/mpozei-client.service';
import { ToastService } from '../../../../shared/services/toast.service';
import {
    IApplication,
    ICardCommunicationForm,
    ICheckUidRestrictionsResponseData,
    IFindEidentityByNumberAndTypeResponseData,
    IForeignIdentity,
    IGetCertificateInformationResponseData,
    INomenclature,
    IPersonalIdentity,
    IReason,
    ISignApplicationRequestData,
    IStep,
    LAWFUL_AGE,
} from '../../interfaces/eid-management.interfaces';
import { bulgarianPhoneNumberValidator } from '../../../../shared/validators/phoneNumber';
import { emailValidator } from '../../../../shared/validators/email';
import { dateMoreThanValidate } from '../../../../shared/validators/date';
import * as moment from 'moment';
import { GuardiansFormComponent } from '../../components/guardians-form/guardians-form.component';
import { SmartCardCommunicationClientService } from '../../services/smart-card-communication-client.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { takeUntil } from 'rxjs/operators';
import { NomenclatureService } from '../../services/nomenclature.service';
import { AppConfigService } from '../../../../core/services/config.service';
import { EidDeviceService } from '../../services/eid-device.service';
import { ConfirmationDialogService } from '../../../../shared/services/confirmation-dialog.service';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
    selector: 'app-application-form',
    templateUrl: './application-form.component.html',
    styleUrls: ['./application-form.component.scss'],
})
export class ApplicationFormComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private pivrClientService: PivrClientService,
        private mpozeiClientService: MpozeiClientService,
        private toastService: ToastService,
        private smartCardCommunicationClientService: SmartCardCommunicationClientService,
        private translocoLocaleService: TranslocoLocaleService,
        private nomenclatureService: NomenclatureService,
        private appConfigService: AppConfigService,
        private eidDeviceService: EidDeviceService,
        private confirmationDialogService: ConfirmationDialogService,
        private oAuthService: OAuthService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtSearchSubtitle'),
                    onClick: this.navigateToSearch.bind(this),
                },
                {
                    label: this.translateService.translate('modules.eidManagement.txtSearchResults'),
                    onClick: this.navigateToSearchResults.bind(this),
                },
                { label: this.translateService.translate('modules.eidManagement.txtNewApplication') },
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
            this.generateSteps();
            this.buildReasonsDropdownData();
            this.holderTypes = this.eidDeviceService.devices.map(device => {
                return {
                    id: device.id,
                    name:
                        device.translations.find(
                            (translation: any) => translation.language === this.translateService.getActiveLang()
                        )?.name || '',
                    type: device.type,
                };
            });

            const isChipCardAvailable = this.holderTypes.find(holder => holder.type === DeviceType.CHIP_CARD);
            if (this.holderTypes.length === 1) {
                this.form.controls.holderType.patchValue(this.holderTypes[0].id);
            } else if (isChipCardAvailable) {
                this.form.controls.holderType.patchValue(isChipCardAvailable.id);
            }
        });

        this.personalDocumentNumber = sessionStorage.getItem('personalDocumentNumber') || null;
        const navigation = this.router.getCurrentNavigation();
        if (navigation || window.history.state) {
            const state = navigation ? (navigation.extras.state as any) : window.history.state;
            if (state) {
                this.personalId = state.personalId;
                this.personalIdType = state.personalIdType;
                this.currentEid = state.currentEid;
                this.application = state.application || null;
                this.restrictionsStatus = state.restrictionsStatus;
                this.personalDocumentNumber = state.personalDocumentNumber;
                sessionStorage.setItem('personalDocumentNumber', state.personalDocumentNumber);
                if (state.personalDocumentNumber === null || state.personalIdType === PersonalIdTypes.LNCH) {
                    sessionStorage.removeItem('personalDocumentNumber');
                }
            }
        }
    }

    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    personalId;
    personalIdType;
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
    holderTypes: { id: string; name: string; type: DeviceType }[] = [];
    cyrillicPattern = /^[аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ]+$/;
    latinPattern = /^[a-zA-Z]*$/;
    form = new FormGroup({
        name: new FormControl<string | null>(null, [Validators.required, Validators.pattern(this.cyrillicPattern)]),
        surname: new FormControl<string | null>(null, [Validators.required, Validators.pattern(this.cyrillicPattern)]),
        familyName: new FormControl<string | null>(null, [
            Validators.required,
            Validators.pattern(this.cyrillicPattern),
        ]),
        firstNameLatin: new FormControl<string | null>(null, [
            Validators.required,
            Validators.pattern(this.latinPattern),
        ]),
        secondNameLatin: new FormControl<string | null>(null, [
            Validators.required,
            Validators.pattern(this.latinPattern),
        ]),
        lastNameLatin: new FormControl<string | null>(null, [
            Validators.required,
            Validators.pattern(this.latinPattern),
        ]),
        email: new FormControl<string>(''),
        phoneNumber: new FormControl<string>(''),
        citizenship: new FormControl<string | null>(null),
        identityIssuer: new FormControl<string | null>(null, Validators.required),
        identityType: new FormControl<string | null>(null, Validators.required),
        identityNumber: new FormControl<string | null>(null, Validators.required),
        identityIssueDate: new FormControl<string | null>(null, Validators.required),
        identityValidityToDate: new FormControl<string | null>(null, [
            Validators.required,
            dateMoreThanValidate('identityIssueDate'),
        ]),
        holderType: new FormControl<string | null>(null, Validators.required),
    });
    signForm = new FormGroup({
        email: new FormControl<string>('', emailValidator()),
        phoneNumber: new FormControl<string>('', bulgarianPhoneNumberValidator()),
    });
    steps: IStep[] = [];
    currentStep = 0;
    submitted = false;
    requestInProgress = true;
    INTERVAL = 10000;
    retryCountIfServerError = 3;
    loadRegixDataSubscription: Subscription = new Subscription();
    applicationId = '';
    applicationNumber = '';
    applicationFee: number | null = null;
    applicationCurrency: string | null = null;
    applicationSecondaryFee: number | null = null;
    applicationSecondaryCurrency: string | null = null;
    application: IApplication | null = null;
    currentEid: IFindEidentityByNumberAndTypeResponseData | null = null;
    loadingRegixData!: boolean;
    restrictionsStatus: ICheckUidRestrictionsResponseData = {
        response: { isProhibited: false, isDead: false, hasRevokedParentalRights: false },
        hasFailed: false,
        error: '',
    };
    isRegixAvailable = true;
    @ViewChild('guardiansForm') guardiansForm: GuardiansFormComponent | undefined;
    guardiansFormVisibleToggle = false;
    cardReaders: string[] = [];
    certificateForm = new FormGroup<ICardCommunicationForm>({
        readerName: new FormControl(
            { value: '', disabled: true },
            { validators: Validators.required, nonNullable: true }
        ),
        can: new FormControl('', { validators: Validators.required, nonNullable: true }),
    });
    isSmartCardServiceAvailable = false;
    closeTimer$ = new Subject<void>();
    retryCountsLeft = this.retryCountIfServerError;
    personalDocumentNumber: string | null = null;
    showDenyDialog = false;
    originalReasons: IReason[] = [];
    reasons: INomenclature[] = [];
    customReasonNames = Object.keys(NomenclatureNameWithAdditionalReasonField);
    showCustomReasonField = false;
    denyForm: FormGroup<{ reason: FormControl<INomenclature | null>; customReason: FormControl<string | null> }> =
        new FormGroup({
            reason: new FormControl<INomenclature | null>(null, Validators.required),
            customReason: new FormControl<string | null>(null, Validators.required),
        });
    loadingSmartCardService = false;
    certificate: IGetCertificateInformationResponseData = {
        certificateInfo: '',
        isPinActivated: false,
        isGenuine: false,
        cardNumber: '',
    };
    unsavedFormDataExists = true;
    exportedConfirmationAtLeastOnce = false;
    @HostListener('window:beforeunload', ['$event'])
    beforeUnloadHandler() {
        // returning true will navigate without confirmation
        // returning false will show a confirm dialog before navigating away
        return !this.unsavedFormDataExists;
    }

    ngOnInit() {
        this.form.disable();
        this.form.controls.holderType.enable();
        this.loadRegixData();
        this.breadcrumbItems = [
            {
                label: this.translateService.translate('modules.eidManagement.txtSearchSubtitle'),
                onClick: this.navigateToSearch.bind(this),
            },
            {
                label: this.translateService.translate('modules.eidManagement.txtSearchResults'),
                onClick: this.navigateToSearchResults.bind(this),
            },
            { label: this.translateService.translate('modules.eidManagement.txtNewApplication') },
        ];
        this.generateSteps();
        if (this.currentEid && (this.currentEid.email || this.currentEid.phoneNumber)) {
            this.signForm.setValue({ email: this.currentEid.email, phoneNumber: this.currentEid.phoneNumber });
            this.form.controls.email.setValidators([emailValidator()]);
            this.form.controls.email.updateValueAndValidity();
            this.form.controls.phoneNumber.setValidators([bulgarianPhoneNumberValidator()]);
            this.form.controls.phoneNumber.updateValueAndValidity();
        }
        if (this.PUKFieldIsVisible) {
            this.certificateForm.addControl(
                'puk',
                new FormControl('', { validators: Validators.required, nonNullable: true })
            );
        }
        if (this.application) {
            this.applicationId = this.application.id;
            this.applicationNumber = this.application.applicationNumber;
            this.applicationFee = this.application.fee;
            this.applicationCurrency = this.application.feeCurrency;
            this.applicationSecondaryFee = this.application.secondaryFee;
            this.applicationSecondaryCurrency = this.application.secondaryFeeCurrency;
            switch (this.application.status) {
                case ApplicationStatus.SIGNED:
                case ApplicationStatus.PENDING_PAYMENT:
                    this.currentStep = 2;
                    break;
                case ApplicationStatus.PAID:
                    if (
                        this.application.submissionType === SubmissionType.EID ||
                        this.application.submissionType === SubmissionType.BASE_PROFILE
                    ) {
                        this.currentStep = 1;
                    } else {
                        this.currentStep = 3;
                        if (this.isApplicationDeviceTypeMobileApp()) {
                            this.checkApplicationStatus();
                        }
                    }
                    break;
                case ApplicationStatus.GENERATED_CERTIFICATE:
                    this.currentStep = 3;
                    this.requestInProgress = true;
                    if (!this.isApplicationDeviceTypeMobileApp()) {
                        this.confirmCertificateStoreError();
                    } else {
                        this.checkApplicationStatus();
                    }
                    break;
                case ApplicationStatus.CERTIFICATE_STORED:
                    this.currentStep = 4;
                    break;
                default:
                    this.currentStep = 1;
                    break;
            }
        }
        this.form.controls.identityIssueDate.valueChanges.subscribe({
            next: () => {
                this.form.controls.identityValidityToDate.updateValueAndValidity();
            },
        });
        this.guardiansFormVisibleToggle = this.isDateBelowLawfulAge || this.restrictionsStatus.response.isProhibited;

        if (!this.isApplicationDeviceTypeMobileApp()) {
            this.initSmartCardService();
        }

        this.nomenclatureService.getReasons().subscribe({
            next: response => {
                this.originalReasons = response;
                this.buildReasonsDropdownData();
            },
        });
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
        this.loadRegixDataSubscription.unsubscribe();
        this.closeTimer$.next();
    }

    get guardiansFormVisible() {
        return this.isDateBelowLawfulAge || this.restrictionsStatus.response.isProhibited;
    }

    formatDate(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
    }

    navigateToSearch() {
        this.router.navigate(['/eid-management']);
    }

    navigateToSearchResults() {
        this.router.navigate(['/eid-management/search/results'], {
            state: {
                personalId: this.personalId,
                personalIdType: this.personalIdType,
                personalDocumentNumber: this.personalDocumentNumber,
            },
        });
    }

    onPersonalDataSubmit() {
        this.submitted = true;
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
            const foundDevice = this.eidDeviceService.getDeviceById(this.form.controls.holderType.value as string);
            if (
                this.form.controls.identityValidityToDate.value &&
                moment(new Date(this.form.controls.identityValidityToDate.value)).isBefore(moment()) &&
                foundDevice?.type === DeviceType.CHIP_CARD
            ) {
                this.showErrorToast(
                    this.translateService.translate('modules.eidManagement.txtIdentityDocumentExpiredError')
                );
            } else {
                this.requestInProgress = true;
                this.mpozeiClientService
                    .createEidApplication(
                        {
                            eidentityId: this.currentEid?.eidentityId as any,
                            firstName: this.form.controls.name.value as string,
                            secondName: this.form.controls.surname.value as string,
                            lastName: this.form.controls.familyName.value as string,
                            firstNameLatin: this.form.controls.firstNameLatin.value as string,
                            secondNameLatin: this.form.controls.secondNameLatin.value as string,
                            lastNameLatin: this.form.controls.lastNameLatin.value as string,
                            citizenIdentifierNumber: this.personalId as string,
                            citizenIdentifierType: this.personalIdType,
                            applicationType: ApplicationType.ISSUE_EID,
                            applicationSubmissionType: SubmissionType.DESK,
                            deviceId: this.form.controls.holderType.value as string,
                            levelOfAssurance: LevelOfAssurance.LOW,
                            citizenship: this.form.controls.citizenship.value as string,
                            personalIdentityDocument: {
                                identityIssuer: this.form.controls.identityIssuer.value as string,
                                identityType: this.form.controls.identityType.value as string,
                                identityIssueDate: this.formatDate(this.form.controls.identityIssueDate.value),
                                identityNumber: this.form.controls.identityNumber.value as string,
                                identityValidityToDate: this.formatDate(
                                    this.form.controls.identityValidityToDate.value
                                ),
                            },
                        },
                        this.isRegixAvailable
                    )
                    .subscribe({
                        next: response => {
                            this.applicationId = response.id;
                            this.applicationNumber = response.applicationNumber;
                            this.applicationFee = response.fee;
                            this.applicationCurrency = response.feeCurrency;
                            this.applicationSecondaryFee = response.secondaryFee;
                            this.applicationSecondaryCurrency = response.secondaryFeeCurrency;
                            this.requestInProgress = false;
                            this.goToNextStep();
                        },
                        error: error => {
                            this.requestInProgress = false;
                            switch (error.status) {
                                case 400:
                                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                    break;
                                default:
                                    this.showErrorToast(
                                        this.translateService.translate('global.txtCreateApplicationError')
                                    );
                                    break;
                            }
                        },
                    });
            }
        }
    }

    onPrintForSignatureSubmit() {
        this.requestInProgress = true;
        this.mpozeiClientService
            .exportEidApplication({
                applicationId: this.applicationId,
            })
            .subscribe({
                next: response => {
                    this.requestInProgress = false;
                    this.printPDFBlob(response);
                    if (
                        this.application?.submissionType === SubmissionType.EID ||
                        this.application?.submissionType === SubmissionType.BASE_PROFILE
                    ) {
                        this.goToNextStep();
                    } else {
                        this.goToNextInnerStep();
                    }
                    if (this.isApplicationDeviceTypeMobileApp()) {
                        this.steps[3].innerSteps = [];
                        this.checkApplicationStatus();
                    }
                },
                error: error => {
                    this.requestInProgress = false;
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        default:
                            this.showErrorToast(this.translateService.translate('global.txtPrintApplicationError'));
                            break;
                    }
                },
            });
    }

    onSignedSubmit(isConfirmationNeeded = false) {
        let guardiansFormIsValid = true;
        if (this.guardiansForm) {
            this.guardiansForm.form.markAllAsTouched();
            guardiansFormIsValid = this.guardiansForm.form.valid;
            this.markAllControlsAsDirty(this.guardiansForm.form.controls.array);
        }
        this.signForm.markAllAsTouched();
        this.markAllControlsAsDirty(this.signForm);
        if (this.signForm.valid && guardiansFormIsValid) {
            this.requestInProgress = true;
            const applicationSignObject: ISignApplicationRequestData = {
                id: this.applicationId,
            };
            if (this.form.controls.email.value || this.signForm.controls.email.value) {
                applicationSignObject.email = this.form.controls.email.value
                    ? this.form.controls.email.value
                    : (this.signForm.controls.email.value as string);
            }
            if (this.form.controls.phoneNumber.value || this.signForm.controls.phoneNumber.value) {
                applicationSignObject.phoneNumber = this.form.controls.phoneNumber.value
                    ? this.form.controls.phoneNumber.value
                    : (this.signForm.controls.phoneNumber.value as string);
            }
            if ((this.isDateBelowLawfulAge || this.restrictionsStatus.response.isProhibited) && this.guardiansForm) {
                this.guardiansForm.form.controls.array.value.forEach(formGroup => {
                    formGroup.personalIdentityDocument.identityIssueDate = this.formatDate(
                        formGroup.personalIdentityDocument.identityIssueDate
                    );
                    formGroup.personalIdentityDocument.identityValidityToDate = this.formatDate(
                        formGroup.personalIdentityDocument.identityValidityToDate
                    );
                });
                applicationSignObject.guardians = this.guardiansForm.form.controls.array.value as any;
            }

            this.mpozeiClientService.signEidApplication(applicationSignObject).subscribe({
                next: response => {
                    if (isConfirmationNeeded) {
                        this.mpozeiClientService
                            .exportEidApplicationConfirmation({ applicationId: this.applicationId })
                            .subscribe({
                                next: response => {
                                    this.requestInProgress = false;
                                    this.printPDFBlob(response);
                                    this.exportedConfirmationAtLeastOnce = true;
                                },
                                error: error => {
                                    this.requestInProgress = false;
                                    switch (error.status) {
                                        case 400:
                                            this.showErrorToast(
                                                this.translateService.translate('global.txtInvalidDataError')
                                            );
                                            break;
                                        default:
                                            this.showErrorToast(
                                                this.translateService.translate('global.txtPrintApplicationError')
                                            );
                                            break;
                                    }
                                },
                            });
                    } else {
                        this.requestInProgress = false;
                        this.goToNextStep();
                    }
                },
                error: error => {
                    this.requestInProgress = false;
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        default:
                            this.showErrorToast(this.translateService.translate('global.txtSignApplicationError'));
                            break;
                    }
                },
            });
        }
    }

    onConfirmSignatureAndPaymentSubmit() {
        this.confirmationDialogService.showConfirmation({
            rejectButtonStyleClass: 'p-button-danger',
            message: this.translateService.translate(
                'modules.eidManagement.txtContinueAfterConfirmSignatureAndPayment'
            ),
            header: this.translateService.translate('global.txtConfirmation'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.mpozeiClientService
                    .updateEidApplication({ applicationId: this.applicationId, status: ApplicationStatus.PAID })
                    .subscribe({
                        next: () => {
                            this.requestInProgress = false;
                            this.goToNextStep();
                        },
                        error: error => {
                            this.requestInProgress = false;
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
            },
        });
    }

    onFinishSubmit() {
        this.requestInProgress = true;
        this.mpozeiClientService.completeApplication(this.applicationId).subscribe({
            next: () => {
                this.requestInProgress = false;
                this.showSuccessToast(
                    this.translateService.translate('modules.eidManagement.txtApplicationCreationSuccess')
                );
                this.unsavedFormDataExists = false;
                this.confirmationDialogService.showConfirmation({
                    rejectButtonStyleClass: 'p-button-danger',
                    message: this.translateService.translate('modules.eidManagement.txtContinueWithSameCitizen'),
                    header: this.translateService.translate('global.txtConfirmation'),
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        this.navigateToSearchResults();
                    },
                    reject: () => {
                        this.router.navigate(['/eid-management']);
                    },
                });
            },
            error: error => {
                this.requestInProgress = false;
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

    onReaderSelected() {
        this.certificateForm.markAllAsTouched();
        Object.values(this.certificateForm.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    innerControl.markAsDirty();
                });
            } else {
                control.markAsDirty();
            }
        });
        if (this.certificateForm.valid) {
            this.requestInProgress = true;
            this.getCertificateInfoFromReader(true);
        }
    }

    getCertificateInfoFromReader(goToNextInnerStep = false) {
        this.requestInProgress = true;
        this.smartCardCommunicationClientService
            .getCertificateInformation({
                can: this.certificateForm.controls.can.value as string,
                readerName: this.certificateForm.controls.readerName.value as string,
            })
            .subscribe({
                next: response => {
                    this.requestInProgress = false;
                    this.certificate = response;
                    if (goToNextInnerStep) {
                        this.goToNextInnerStep();
                    }
                },
                error: (error: any) => {
                    this.requestInProgress = false;
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        case 404:
                            this.showErrorToast(
                                this.translateService.translate('modules.eidManagement.txtSmartCardNotDetectedError')
                            );
                            break;
                        default:
                            if (error.detail) {
                                this.parseSmartCardErrorType(error);
                            } else {
                                this.showErrorToast(
                                    this.translateService.translate(
                                        'modules.eidManagement.txtSmartCardErrorType.CIAInfoIsNotInitializedProperly'
                                    )
                                );
                            }
                            break;
                    }
                },
            });
    }

    saveCertificateToDevice() {
        this.requestInProgress = true;
        this.mpozeiClientService
            .findEidentityByNumberAndType({
                type: this.personalIdType,
                number: this.personalId,
            })
            .subscribe({
                next: response => {
                    this.currentEid = response;

                    this.oAuthService.refreshToken().then(tokenResponse => {
                        this.smartCardCommunicationClientService
                            .saveCertificate({
                                commonName: `${this.personalIdentity?.personNames?.firstName} ${
                                    this.personalIdentity?.personNames?.surname || ''
                                } ${this.personalIdentity?.personNames?.familyName || ''}`.replace(/ {2,}/g, ' '),
                                countryCode: 'BG',
                                readerName: this.certificateForm.controls.readerName.value as string,
                                can: this.certificateForm.controls.can.value as string,
                                certificateAuthorityName:
                                    this.appConfigService.config.certificate.certificateAuthorityName,
                                token: this.oAuthService.getAccessToken(),
                                applicationId: this.applicationId,
                                puk: (this.certificateForm.controls.puk?.value as string) || '',
                                givenName: this.form.controls.firstNameLatin.value as string,
                                surname: this.form.controls.lastNameLatin.value as string,
                                serialNumber: `${this.appConfigService.config.certificate.certificateSerialNumberPrefix}${this.currentEid?.eidentityId}`,
                                cardNumber: this.certificate.cardNumber,
                            })
                            .subscribe({
                                next: () => {
                                    this.mpozeiClientService
                                        .confirmCertificate({ applicationId: this.applicationId, status: 'OK' })
                                        .subscribe({
                                            next: () => {
                                                this.getCertificateInfoFromReader(true);
                                            },
                                            error: error => {
                                                this.requestInProgress = false;
                                                switch (error.status) {
                                                    case 400:
                                                        this.showErrorToast(
                                                            this.translateService.translate(
                                                                'global.txtInvalidDataError'
                                                            )
                                                        );
                                                        break;
                                                    default:
                                                        this.showErrorToast(
                                                            this.translateService.translate(
                                                                'modules.eidManagement.txtErrorConfirmCertificateStore'
                                                            )
                                                        );
                                                        break;
                                                }
                                            },
                                        });
                                },
                                error: error => {
                                    this.requestInProgress = false;
                                    switch (error.status) {
                                        case 400:
                                            this.showErrorToast(
                                                this.translateService.translate('global.txtInvalidDataError')
                                            );
                                            break;
                                        case 404:
                                            this.showErrorToast(
                                                this.translateService.translate(
                                                    'modules.eidManagement.txtSmartCardNotDetectedError'
                                                )
                                            );
                                            break;
                                        default:
                                            if (error.detail) {
                                                this.parseSmartCardErrorType(error);
                                                // The enroll process was not successful so we need to notify mpozei to update the pipeline
                                                if (error.detail.includes('SendCSRToMpozeiFailed')) {
                                                    this.confirmCertificateStoreError();
                                                }
                                            } else {
                                                this.showErrorToast(
                                                    this.translateService.translate('global.txtEnrollCertificateError')
                                                );
                                            }
                                            break;
                                    }
                                },
                            });
                    });
                },
                error: (error: any) => {
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        default:
                            this.showErrorToast(this.translateService.translate('global.txtNoConnectionEid'));
                            break;
                    }
                },
            });
    }

    goToNextStep() {
        this.currentStep = this.currentStep + 1;
    }

    goToPreviousStep() {
        this.currentStep = this.currentStep - 1;
    }

    goToNextInnerStep() {
        if (this.steps[this.currentStep] && this.steps[this.currentStep].currentInnerStep !== undefined) {
            this.steps[this.currentStep].currentInnerStep! += 1;
        }
    }

    goToPreviousInnerStep() {
        if (this.steps[this.currentStep] && this.steps[this.currentStep].currentInnerStep !== undefined) {
            this.steps[this.currentStep].currentInnerStep! -= 1;
        }
    }

    get translatedPersonalIdType() {
        return this.personalIdTypes.find(type => {
            return type.id === this.personalIdType;
        });
    }

    loadRegixData() {
        this.loadingRegixData = true;
        this.requestInProgress = true;

        if (this.personalIdType === PersonalIdTypes.EGN && this.personalDocumentNumber) {
            this.loadRegixDataSubscription = this.pivrClientService
                .getPersonalIdentityV2({
                    EGN: this.personalId,
                    IdentityDocumentNumber: this.personalDocumentNumber,
                })
                .pipe(retry({ count: this.retryCountIfServerError, delay: this.INTERVAL }))
                .subscribe({
                    next: response => {
                        this.requestInProgress = false;
                        this.loadingRegixData = false;
                        this.isRegixAvailable = true;
                        this.personalIdentity = response.response.PersonalIdentityInfoResponse;
                        this.form.patchValue({
                            name: this.personalIdentity.personNames?.firstName,
                            surname: this.personalIdentity.personNames?.surname,
                            familyName: this.personalIdentity.personNames?.familyName,
                            firstNameLatin: this.personalIdentity.personNames?.firstNameLatin,
                            secondNameLatin: this.personalIdentity.personNames?.surnameLatin,
                            lastNameLatin: this.personalIdentity.personNames?.lastNameLatin,
                            citizenship: this.personalIdentity.nationalityList[0].nationalityName,
                            phoneNumber: this.currentEid?.phoneNumber,
                            email: this.currentEid?.email,
                            identityType: this.personalIdentity.documentType,
                            identityValidityToDate: this.personalIdentity.validDate
                                ? this.formatDate(this.personalIdentity.validDate)
                                : null,
                            identityIssuer: this.personalIdentity.issuerName,
                            identityIssueDate: this.personalIdentity.issueDate
                                ? this.formatDate(this.personalIdentity.issueDate)
                                : null,
                            identityNumber: this.personalIdentity.identityDocumentNumber,
                        });
                    },
                    error: () => {
                        this.confirmationDialogService.showConfirmation({
                            rejectButtonStyleClass: 'p-button-danger',
                            message: this.translateService.translate('modules.eidManagement.txtFailedLoadingRegixData'),
                            header: this.translateService.translate('global.txtConfirmation'),
                            icon: 'pi pi-exclamation-triangle',
                            accept: () => {
                                this.requestInProgress = false;
                                this.loadingRegixData = false;
                                this.isRegixAvailable = false;
                                this.form.enable();
                                this.form.controls.email.disable();
                                this.form.controls.phoneNumber.disable();
                                this.form.patchValue({
                                    phoneNumber: this.currentEid?.phoneNumber,
                                    email: this.currentEid?.email,
                                });
                            },
                            reject: () => {
                                this.router.navigate(['/eid-management']);
                            },
                        });
                    },
                });
        } else {
            this.loadRegixDataSubscription = this.pivrClientService
                .getForeignIdentityV2({
                    Identifier: this.personalId,
                    IdentifierType: 'LNCh',
                })
                .pipe(retry({ count: this.retryCountIfServerError, delay: this.INTERVAL }))
                .subscribe({
                    next: response => {
                        this.requestInProgress = false;
                        this.loadingRegixData = false;
                        this.isRegixAvailable = true;
                        this.personalIdentity = response.response.ForeignIdentityInfoResponse;
                        this.form.patchValue({
                            name: this.personalIdentity.personNames?.firstName,
                            surname: this.personalIdentity.personNames?.surname,
                            familyName: this.personalIdentity.personNames?.familyName,
                            firstNameLatin: this.personalIdentity.personNames?.firstNameLatin,
                            secondNameLatin: this.personalIdentity.personNames?.surnameLatin,
                            lastNameLatin: this.personalIdentity.personNames?.lastNameLatin,
                            citizenship: this.personalIdentity.nationalityList[0].nationalityName,
                            phoneNumber: this.currentEid?.phoneNumber,
                            email: this.currentEid?.email,
                            identityType: this.personalIdentity.documentType,
                            identityValidityToDate: this.personalIdentity?.validDate
                                ? this.formatDate(this.personalIdentity?.validDate)
                                : null,
                            identityIssuer: this.personalIdentity?.issuerName,
                            identityIssueDate: this.personalIdentity?.issueDate
                                ? this.formatDate(this.personalIdentity?.issueDate)
                                : null,
                            identityNumber: this.personalIdentity?.identityDocumentNumber,
                        });
                    },
                    error: () => {
                        this.confirmationDialogService.showConfirmation({
                            rejectButtonStyleClass: 'p-button-danger',
                            message: this.translateService.translate('modules.eidManagement.txtFailedLoadingRegixData'),
                            header: this.translateService.translate('global.txtConfirmation'),
                            icon: 'pi pi-exclamation-triangle',
                            accept: () => {
                                this.requestInProgress = false;
                                this.loadingRegixData = false;
                                this.isRegixAvailable = false;
                                this.form.enable();
                                this.form.controls.email.disable();
                                this.form.controls.phoneNumber.disable();
                                this.form.patchValue({
                                    phoneNumber: this.currentEid?.phoneNumber,
                                    email: this.currentEid?.email,
                                });
                            },
                            reject: () => {
                                this.router.navigate(['/eid-management']);
                            },
                        });
                    },
                });
        }
    }

    printPDFBlob(data: Blob) {
        const blob = new Blob([data], { type: 'application/pdf' });
        const blobURL = URL.createObjectURL(blob);
        let iframe = document.getElementById('printFrame') as HTMLIFrameElement;
        if (!iframe) {
            iframe = document.createElement('iframe');
            iframe.setAttribute('id', 'printFrame');
            iframe.style.display = 'none';
            document.body.appendChild(iframe);
        }

        iframe.src = blobURL;
        iframe.onload = function () {
            setTimeout(function () {
                iframe.focus();
                iframe.contentWindow?.print();
            }, 1);
        };
    }

    applicationTypeTranslation() {
        let deviceId = this.form.controls.holderType.value as string;
        if (this.application) {
            deviceId = this.application.deviceId;
        }
        return this.eidDeviceService.getDeviceTranslation(deviceId);
    }

    get isDateBelowLawfulAge() {
        return moment().diff(moment(this.personalIdentity?.birthDate), 'years') < LAWFUL_AGE;
    }

    checkApplicationStatus() {
        timer(0, this.INTERVAL)
            .pipe(
                switchMap(() => this.mpozeiClientService.getApplicationById({ id: this.applicationId })),
                takeUntil(this.closeTimer$) // close the subscription when `closeTimer$` emits
            )
            .subscribe({
                next: response => {
                    if (
                        (response.status === ApplicationStatus.SIGNED && this.currentStep === 1) ||
                        (response.status === ApplicationStatus.GENERATED_CERTIFICATE && this.currentStep === 1)
                    ) {
                        this.closeTimer$.next();
                        this.currentStep = 2;
                        this.checkApplicationStatus();
                    } else if (
                        response.status === ApplicationStatus.CERTIFICATE_STORED &&
                        (this.currentStep === 2 || this.currentStep === 3)
                    ) {
                        this.closeTimer$.next();
                        this.currentStep = 4;
                    }
                },
                error: () => {
                    if (this.retryCountsLeft > 0) {
                        this.retryCountsLeft -= 1;
                        this.checkApplicationStatus();
                    } else {
                        this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                    }
                },
            });
    }

    isApplicationDeviceTypeMobileApp() {
        let deviceId = this.form.controls.holderType.value || '';
        if (this.application) {
            deviceId = this.application.deviceId;
        }

        const foundDevice = this.eidDeviceService.getDeviceById(deviceId);
        return foundDevice ? foundDevice.type === DeviceType.MOBILE : false;
    }

    generateSteps() {
        this.steps = [
            {
                id: 1,
                ref: 'personalData',
                name: this.translateService.translate('modules.eidManagement.txtPersonalDataStep'),
            },
            {
                id: 2,
                ref: 'verifyAndSign',
                name: this.translateService.translate('modules.eidManagement.txtVerifyAndSignStep'),
                currentInnerStep: 0,
                innerSteps: [
                    { id: 1, name: this.translateService.translate('modules.eidManagement.txtPrintPersonalDataStep') },
                    {
                        id: 2,
                        name: this.translateService.translate('modules.eidManagement.txtAdditionalPersonalDataStep'),
                    },
                ],
            },
            {
                id: 3,
                ref: 'confirmation',
                name: this.translateService.translate('modules.eidManagement.txtConfirmationStep'),
            },
            {
                id: 4,
                ref: 'generate',
                name: this.translateService.translate('modules.eidManagement.txtGenerateStep'),
                currentInnerStep: 0,
                innerSteps: [
                    { id: 1, name: this.translateService.translate('modules.eidManagement.txtSelectReader') },
                    { id: 2, name: this.translateService.translate('modules.eidManagement.txtCertificateInfo') },
                    { id: 3, name: this.translateService.translate('modules.eidManagement.txtSaveToDevice') },
                    { id: 4, name: this.translateService.translate('modules.eidManagement.txtCertificateInfo') },
                ],
            },
            {
                id: 5,
                ref: 'delivery',
                name: this.translateService.translate('modules.eidManagement.txtDeliveryStep'),
            },
        ];
        if (this.isApplicationDeviceTypeMobileApp()) {
            this.steps[3].innerSteps = [];
        }
        if (
            this.application?.submissionType === SubmissionType.EID ||
            this.application?.submissionType === SubmissionType.BASE_PROFILE
        ) {
            this.steps[1].innerSteps?.splice(1, 1);
            this.steps.splice(2, 1);
        }
    }

    get currentStepRef() {
        return this.steps[this.currentStep].ref;
    }

    confirmCertificateStoreError() {
        this.mpozeiClientService.confirmCertificate({ applicationId: this.applicationId, status: 'ERROR' }).subscribe({
            next: () => {
                this.requestInProgress = false;
            },
            error: error => {
                this.requestInProgress = false;
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(
                            this.translateService.translate('modules.eidManagement.txtErrorConfirmCertificateStore')
                        );
                        break;
                }
            },
        });
    }

    denyApplication() {
        this.showDenyDialog = true;
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

    onDataSubmit() {
        this.denyForm.markAllAsTouched();
        Object.values(this.denyForm.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    innerControl.markAsDirty();
                });
            } else {
                control.markAsDirty();
            }
        });
        if (this.denyForm.valid) {
            this.requestInProgress = true;
            const denyData: { applicationId: string; reason: { reasonId: string; reasonText?: string } } = {
                applicationId: this.applicationId,
                reason: {
                    reasonId: this.denyForm.controls.reason.value?.id as string,
                },
            };
            if (this.denyForm.controls.customReason.value) {
                denyData.reason['reasonText'] = this.denyForm.controls.customReason.value;
            }
            this.mpozeiClientService.denyApplication(denyData).subscribe({
                next: () => {
                    this.showDenyDialog = false;
                    this.requestInProgress = false;
                    this.showSuccessToast(
                        this.translateService.translate('modules.eidManagement.txtApplicationDenySuccess')
                    );
                    this.navigateToSearchResults();
                },
                error: (error: any) => {
                    this.requestInProgress = false;
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

    initSmartCardService() {
        this.loadingSmartCardService = true;
        this.requestInProgress = true;
        this.smartCardCommunicationClientService.getReaders().subscribe({
            next: response => {
                this.loadingSmartCardService = false;
                this.requestInProgress = false;
                this.isSmartCardServiceAvailable = true;
                this.cardReaders = response;
                if (this.appConfigService.config.uiFeatures?.defaultReaderName) {
                    const defaultReaderAvailable = this.cardReaders.find(reader =>
                        reader.includes(this.appConfigService.config.uiFeatures?.defaultReaderName as string)
                    );
                    if (defaultReaderAvailable) {
                        this.certificateForm.controls.readerName.patchValue(defaultReaderAvailable);
                    }
                }
                this.certificateForm.controls.readerName.enable();
            },
            error: error => {
                this.loadingSmartCardService = false;
                this.requestInProgress = false;
                this.isSmartCardServiceAvailable = false;
                this.certificateForm.controls.readerName.enable();
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtNoConnectionReaders'));
                        break;
                }
            },
        });
    }

    parseTextToJSON(inputText: string) {
        const lines = inputText.split('\n');
        const parsedData: { [index: string]: any } = {};

        for (let i = 0; i < lines.length; i++) {
            const line = lines[i].trim();
            if (line.startsWith('[')) {
                const key = line.slice(1, -1);
                parsedData[key] = lines[i + 1].trim();
                if (key === 'Subject') {
                    parsedData[key] = parsedData[key].split(', ').reverse().join(', ');
                }
            }
        }

        return parsedData;
    }

    parseSmartCardErrorType(error: any) {
        const errors = error.detail.split(',').map((item: string) => {
            return item.trim();
        });
        let translationForThisErrorType = null;
        let translationKey = null;

        for (let i = 0; i < errors.length; i++) {
            const key = errors[i];
            if (
                this.translateService.getTranslation(this.translateService.getActiveLang())[
                    `modules.eidManagement.txtSmartCardErrorType.${key}`
                ]
            ) {
                translationForThisErrorType = this.translateService.getTranslation(
                    this.translateService.getActiveLang()
                )[`modules.eidManagement.txtSmartCardErrorType.${key}`];
                translationKey = `modules.eidManagement.txtSmartCardErrorType.${key}`;
                break;
            }
        }

        if (translationForThisErrorType && translationKey) {
            this.toastService.showErrorDetailsToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate(translationKey),
                errors
            );
        } else {
            this.showErrorToast(this.translateService.translate('global.txtEnrollCertificateError'));
        }
    }

    get areIdCardNumbersIdentical() {
        return this.certificate.cardNumber === this.form.controls.identityNumber.value;
    }

    get PUKFieldIsVisible() {
        return this.appConfigService.config.uiFeatures?.showPUKField || false;
    }

    get isConfirmationNeeded() {
        if (!this.currentEid || (this.currentEid && !this.currentEid.email)) {
            if (this.signForm.controls.email.value || this.signForm.controls.phoneNumber.value) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    get isAdditionalDataNeeded() {
        return (
            this.isDateBelowLawfulAge ||
            this.restrictionsStatus.response.isProhibited ||
            !this.currentEid ||
            (this.currentEid && !this.currentEid.email && !this.isApplicationDeviceTypeMobileApp())
        );
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    protected readonly ApplicationStatus = ApplicationStatus;

    markAllControlsAsDirty(control: FormGroup | FormArray | FormControl): void {
        if (control instanceof FormGroup) {
            Object.keys(control.controls).forEach(key => {
                this.markAllControlsAsDirty(control.controls[key] as any);
            });
        } else if (control instanceof FormArray) {
            control.controls.forEach(ctrl => {
                this.markAllControlsAsDirty(ctrl as any);
            });
        } else if (control instanceof FormControl) {
            control.markAsDirty();
        }
    }
}
