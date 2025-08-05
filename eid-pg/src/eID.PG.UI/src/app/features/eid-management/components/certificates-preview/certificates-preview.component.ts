import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import {
    Application,
    ICertificate,
    IDevices,
    IEidAdministrator,
    IEidAdministratorOffice,
    IHideCertificatePreviewEventEmitter,
    INomenclature,
    IReason,
    Tariff,
} from '../../interfaces/eid-management.interface';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from 'src/app/shared/services/toast.service';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { Subject, Subscription, first, switchMap, takeUntil, timer } from 'rxjs';
import { EidManagementService } from '../../services/eid-management.service';
import { IdentifierType } from 'src/app/features/authorization-register/enums/authorization-register.enum';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { dateMoreThanOrEqualValidate, isDateBelowLawfulAge } from 'src/app/shared/validators/date';
import { EGNAdultValidator, EGNValidator, PinOrAdultEGNValidator, PinValidator } from 'src/app/shared/validators/egn';
import { DocumentType } from 'src/app/features/authorization-register/enums/authorization-register.enum';
import { requiredLatinNamesValidator } from 'src/app/shared/validators/names';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { ActivatedRoute, Router } from '@angular/router';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { RaeiceiClientService } from '../../services/raeicei-client.service';
import {
    ApplicationTypes,
    CertificateStatus,
    ILevelOfAssurances,
    PermittedUser,
    ReasonNamesByAction,
    SubmissionTypes,
} from '../../enums/eid-management';
import { NomenclatureService } from '../../services/nomenclature.service';
import { AppConfigService } from 'src/app/core/services/config.service';
import { AccessTypes } from 'src/app/shared/enums/access-types';
import {
    BoricaSignatureStatus,
    SignatureProvider,
    SignatureStatus,
} from 'src/app/features/authorization-register/enums/document-sign.enum';
import { SignClientService } from 'src/app/features/authorization-register/services/sign-client.service';
import { Base64 } from 'js-base64';
import {
    KEPDigestRequestData,
    KEPSignRequestData,
    BoricaSignRequestData,
    SignTransactions,
} from 'src/app/features/authorization-register/interfaces/document-sign.interfaces';
@Component({
    selector: 'app-certificates-preview',
    templateUrl: './certificates-preview.component.html',
    styleUrls: ['./certificates-preview.component.scss'],
})
export class CertificatesPreviewComponent implements OnChanges, OnDestroy, OnInit {
    breadcrumbItems: IBreadCrumbItems[] = [];
    subscriptions: Subscription[] = [];
    requestInProgress!: boolean;
    certificate!: ICertificate;
    submitted!: boolean;
    previewMode!: boolean;
    showHistory!: boolean;
    isDescriptionMandatory!: boolean;
    refreshTable = false;
    latinPatternFirstOrSecondName = /^(?=.{1,40}$)[a-zA-Z\s-']*$/;
    latinPatternLastName = /^(?=.{1,60}$)[a-zA-Z\s-']*$/;
    certificateId!: string | null;
    administratorOffices!: any[];
    showRevokeMessage!: boolean;
    isOpen!: boolean;
    isRevokeOpen!: boolean;
    tariff!: Tariff;
    CertificateStatus = CertificateStatus;
    originalReasonName: string | undefined;
    originalReasons: IReason[] = [];
    reasons: INomenclature[] = [];
    stopReasonName = 'STOPPED_CANCELED_BY_USER';
    paymentUrl = this.appConfigService.config.externalLinks.paymentByAccessCodeUrl;
    accessTypes = AccessTypes;
    isSignatureRequired = JSON.parse(sessionStorage.getItem('user') || '{}')?.acr === this.accessTypes.LOW;
    aliasControl = new FormControl<string>('', [Validators.minLength(1), Validators.maxLength(30)]);
    signatureForm = new FormGroup({
        xml: new FormControl<string | null>(null),
        signatureMethod: new FormControl<string | null>(null, [Validators.required]),
        signature: new FormControl<string | null>(null, [Validators.required]),
    });
    responseXml!: string;
    nexuDetected = false;
    userHasToPay!: boolean;
    loading = false;
    INTERVAL = 5000;
    closeTimer$ = new Subject<void>();
    documentFileName = 'ApplicationForm';
    retryCountIfServerError = 2;
    retryCountsLeft = this.retryCountIfServerError;
    selectedSignatureProvider: SignatureProvider | null = null;
    signTransactions: SignTransactions = {
        [SignatureProvider.Borica]: {
            code: '',
            data: {
                callbackId: '',
                validity: 0,
            },
            message: '',
            responseCode: '',
        },
        [SignatureProvider.Evrotrust]: {
            groupSigning: false,
            transactions: [],
            threadID: '',
        },
    };
    form = new FormGroup({
        firstName: new FormControl<string | null>(null),
        secondName: new FormControl<string | null>(null),
        lastName: new FormControl<string | null>(null),
        firstNameLatin: new FormControl<string | null>(null),
        secondNameLatin: new FormControl<string | null>(null),
        lastNameLatin: new FormControl<string | null>(null),
        applicationType: new FormControl<ApplicationTypes>(ApplicationTypes.ISSUE_EID, { nonNullable: true }),
        citizenship: new FormControl<string | null>(null, [Validators.required]),
        citizenIdentifierNumber: new FormControl<string | null>(null, [
            Validators.required,
            Validators.pattern('[0-9]+'),
            PinOrAdultEGNValidator(),
        ]),
        citizenIdentifierType: new FormControl<string | null>(null, [Validators.required]),
        personalIdentityDocument: new FormGroup({
            identityNumber: new FormControl<string | null>(null, [
                Validators.required,
                Validators.pattern('[A-Z0-9]+'),
            ]),
            identityType: new FormControl<DocumentType | null>(null, [Validators.required]),
            identityIssueDate: new FormControl<string | null>(null, [Validators.required]),
            identityValidityToDate: new FormControl<string | null>(null, [
                Validators.required,
                dateMoreThanOrEqualValidate('identityIssueDate'),
            ]),
            identityIssuer: new FormControl<DocumentType | null>(null, [Validators.required]),
        }),
        reasonId: new FormControl<string | null>(null),
        reasonText: new FormControl<string | null>(null),
        certificateId: new FormControl<string | null>(this.certificateId),
        dateOfBirth: new FormControl<string | null>(null, [Validators.required, isDateBelowLawfulAge('dateOfBirth')]),
    });
    identifierTypes = [
        {
            name: this.translateService.translate('modules.authorizationStatement.statementForm.EGN'),
            id: IdentifierType.EGN,
        },
        {
            name: this.translateService.translate('modules.authorizationStatement.statementForm.LNCh'),
            id: IdentifierType.LNCh,
        },
    ];

    documentTypes = [
        {
            name: this.translateService.translate('modules.eidManagement.documentTypeOptions.personalIdCard'),
            id: DocumentType.ID_CARD,
        },
        {
            name: this.translateService.translate('modules.eidManagement.documentTypeOptions.passport'),
            id: DocumentType.PASSPORT,
        },
        {
            name: this.translateService.translate('modules.eidManagement.documentTypeOptions.passport2'),
            id: DocumentType.PASSPORT2,
        },
    ];
    signatureOptions = [
        {
            name: this.translateService.translate(
                'modules.authorizationStatement.electronicSignature.txtLocalSignature'
            ),
            code: SignatureProvider.KEP,
        },
        {
            name: this.translateService.translate(
                'modules.authorizationStatement.electronicSignature.txtBoricaSignature'
            ),
            code: SignatureProvider.Borica,
        },
        {
            name: this.translateService.translate(
                'modules.authorizationStatement.electronicSignature.txtEvrotrustSignature'
            ),
            code: SignatureProvider.Evrotrust,
        },
    ];

    constructor(
        public translateService: TranslocoService,
        private eidManagementService: EidManagementService,
        private toastService: ToastService,
        private translocoLocaleService: TranslocoLocaleService,
        private route: ActivatedRoute,
        private router: Router,
        private raeiceiService: RaeiceiClientService,
        private nomenclatureService: NomenclatureService,
        private appConfigService: AppConfigService,
        private signService: SignClientService
    ) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.certificateId = this.route.snapshot.paramMap.get('id');
            this.updateBreadcrumbItems();
            this.identifierTypes = [
                {
                    name: this.translateService.translate('modules.authorizationStatement.statementForm.EGN'),
                    id: IdentifierType.EGN,
                },
                {
                    name: this.translateService.translate('modules.authorizationStatement.statementForm.LNCh'),
                    id: IdentifierType.LNCh,
                },
            ];
            this.documentTypes = [
                {
                    name: this.translateService.translate('modules.eidManagement.documentTypeOptions.personalIdCard'),
                    id: DocumentType.ID_CARD,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.documentTypeOptions.passport'),
                    id: DocumentType.PASSPORT,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.documentTypeOptions.passport2'),
                    id: DocumentType.PASSPORT2,
                },
            ];
            this.signatureOptions = [
                {
                    name: this.translateService.translate(
                        'modules.authorizationStatement.electronicSignature.txtLocalSignature'
                    ),
                    code: SignatureProvider.KEP,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationStatement.electronicSignature.txtBoricaSignature'
                    ),
                    code: SignatureProvider.Borica,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationStatement.electronicSignature.txtEvrotrustSignature'
                    ),
                    code: SignatureProvider.Evrotrust,
                },
            ];
        });
        this.subscriptions.push(languageChangeSubscription);
    }

    ngOnInit(): void {
        // this.certificateId --> used when we were navigated from a specific url /eid-management/certificates/:id
        // this.data --> used when we were navigated using preview button at certificates list /eid-management/certificates
        if (this.certificateId) {
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
            this.form.patchValue({ certificateId: this.certificateId });
            this.getCertificateData(this.certificateId);
        } else {
            this.form.patchValue({ certificateId: this.data.id });
            this.getCertificateData(this.data.id);
        }
        this.setupFormChangeListeners();
    }

    @Output() hide: EventEmitter<IHideCertificatePreviewEventEmitter> =
        new EventEmitter<IHideCertificatePreviewEventEmitter>();
    @Input() data: Application = {
        id: '',
        status: '',
        createDate: new Date(),
        eidentityId: '',
        eidAdministratorName: '',
        deviceId: '',
        applicationType: '',
        applicationNumber: '',
        serialNumber: '',
        reasonId: '',
        submissionType: SubmissionTypes.EID,
        action: ApplicationTypes.ISSUE_EID,
    };
    @Input() statusChangeRequest!: boolean;
    @Input() administratorsList!: IEidAdministrator[];
    @Input() devicesList!: IDevices[];
    ngOnChanges(changes: SimpleChanges) {
        this.requestInProgress = false;
        if (changes['data'] || changes['application']) {
            this.updateBreadcrumbItems();
        }
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    updateBreadcrumbItems() {
        if ((this.data && this.data.id) || (this.certificateId && this.certificate)) {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtTitle'),
                    routerLink: '/eid-management',
                },
                {
                    label: this.translateService.translate('modules.eidManagement.txtCertificates'),
                    onClick: this.hideChangeRequest.bind(this),
                },
                {
                    label: `${this.translateService.translate('modules.eidManagement.tableFilter.txtCertificate')} ${
                        this.data.serialNumber ? this.data.serialNumber : this.certificate?.serialNumber
                    }`,
                },
            ];
        }
    }

    getCertificateData(id: string) {
        this.eidManagementService
            .getCertificateById(id)
            .pipe(first())
            .subscribe({
                next: (certificate: any) => {
                    this.certificate = certificate;
                    this.updateBreadcrumbItems();
                    const offices = this.raeiceiService
                        .fetchOffices(this.certificate.eidAdministratorId)
                        .subscribe((office: IEidAdministratorOffice[]) => {
                            this.administratorOffices = office;
                        });
                    this.subscriptions.push(offices);
                    let newApplicationType;
                    if (this.statusChangeRequest) {
                        if (this.data.action === ApplicationTypes.REVOKE_EID) {
                            newApplicationType = ApplicationTypes.REVOKE_EID;
                        } else {
                            newApplicationType =
                                this.certificate.status === CertificateStatus.ACTIVE
                                    ? ApplicationTypes.STOP_EID
                                    : ApplicationTypes.RESUME_EID;
                        }
                        this.form.controls.applicationType.patchValue(newApplicationType);
                        this.form.controls.reasonId.setValidators(
                            newApplicationType === ApplicationTypes.STOP_EID ||
                                newApplicationType === ApplicationTypes.REVOKE_EID
                                ? Validators.required
                                : null
                        );
                        this.form.controls.reasonId.updateValueAndValidity();
                        this.getEidentity();
                    }
                    this.nomenclatureService.reasons().then(response => {
                        this.originalReasons = response;

                        const nomenclatures = this.originalReasons.flatMap(({ nomenclatures }) => nomenclatures);
                        this.originalReasonName = nomenclatures.find(nomenclature => {
                            return nomenclature.id === this.certificate.reasonId;
                        })?.name;
                        this.buildReasonsDropdownData();
                    });
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
                    this.router.navigate(['eid-management/certificates']);
                },
            });
    }

    findAdministratorOffice(id: string) {
        const office = this.administratorOffices.find((office: any) => id === office.id);
        return office?.name ? office.name : '';
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

    buildReasonsDropdownData(): void {
        const foundReason = this.originalReasons.find(
            reason => reason.name === ReasonNamesByAction[this.form.controls.applicationType.value]
        );
        if (foundReason) {
            this.reasons = foundReason.nomenclatures.filter(nomenclature => {
                return (
                    nomenclature.language === this.translateService.getActiveLang() &&
                    nomenclature.permittedUser === PermittedUser.PUBLIC
                );
            });
            if (this.form.get('applicationType')?.value === ApplicationTypes.STOP_EID) {
                const onlineStopReason = this.reasons.find(el => el.name === this.stopReasonName);
                this.form.get('reasonId')?.setValue(onlineStopReason ? onlineStopReason.id : '');
            }
        }
    }

    hideChangeRequest(): void {
        if (this.certificateId) {
            this.router.navigate(['/eid-management/certificates']);
        } else {
            this.statusChangeRequest = false;
            this.hide.emit({ showPreview: false, refreshTable: this.refreshTable, statusChangeRequest: false });
        }
    }

    computedStatus(status: any) {
        return this.translateService.translate('modules.eidManagement.certificateStatus.txt' + status);
    }

    statusRequestForm(revokeRequest: boolean) {
        this.statusChangeRequest = true;
        this.showHistory = false;
        let newApplicationType;
        if (revokeRequest) {
            newApplicationType = ApplicationTypes.REVOKE_EID;
        } else {
            newApplicationType =
                this.certificate.status === CertificateStatus.ACTIVE
                    ? ApplicationTypes.STOP_EID
                    : ApplicationTypes.RESUME_EID;
        }
        this.form.controls.applicationType.patchValue(newApplicationType);
        this.form.controls.reasonId.setValidators(
            newApplicationType === ApplicationTypes.STOP_EID || newApplicationType === ApplicationTypes.REVOKE_EID
                ? Validators.required
                : null
        );
        this.form.controls.reasonId.updateValueAndValidity();
        this.getEidentity();
        this.nomenclatureService.reasons().then(response => {
            this.originalReasons = response;

            const nomenclatures = this.originalReasons.flatMap(({ nomenclatures }) => nomenclatures);
            this.originalReasonName = nomenclatures.find(nomenclature => {
                return nomenclature.id === this.certificate.reasonId;
            })?.name;
            this.buildReasonsDropdownData();
        });
    }

    onIdentifierChange(event: any, formGroup: FormGroup) {
        if (event.value === IdentifierType.EGN) {
            formGroup.controls['citizenIdentifierNumber'].setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                EGNAdultValidator(),
                EGNValidator(),
            ]);
        } else if (event.value === IdentifierType.LNCh) {
            formGroup.controls['citizenIdentifierNumber'].setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                PinValidator(),
            ]);
        }
        formGroup.controls['citizenIdentifierNumber'].updateValueAndValidity();
    }

    setupFormChangeListeners() {
        const formChangeSubscription = this.form.valueChanges.subscribe(() => {
            requiredLatinNamesValidator(this.form);
            const identityValidityToDateControl = this.form.get('personalIdentityDocument.identityValidityToDate');
            const identityIssueDateValue = this.form.get('personalIdentityDocument.identityIssueDate')?.value;
            if (identityIssueDateValue) {
                identityValidityToDateControl?.setValidators([
                    Validators.required,
                    dateMoreThanOrEqualValidate('identityIssueDate'),
                ]);
            } else {
                identityValidityToDateControl?.clearValidators();
            }
            identityValidityToDateControl?.updateValueAndValidity({ emitEvent: false });
        });

        this.subscriptions.push(formChangeSubscription);
    }

    getEidentity() {
        this.eidManagementService
            .getEidentity()
            .pipe(first())
            .subscribe({
                next: (eidentity: any) => {
                    this.form.patchValue(eidentity);
                    const citizenIdentifierType = this.form.get('citizenIdentifierType')?.value;
                    const citizenIdentifierNumber = this.form.get('citizenIdentifierNumber')?.value;
                    if (citizenIdentifierType !== null && citizenIdentifierNumber !== null) {
                        // IF both controls have values, disable them
                        this.form.get('citizenIdentifierType')?.disable();
                        this.form.get('citizenIdentifierNumber')?.disable();
                    }
                },
                error: err => {
                    let showDefaultError = true;
                    err.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.toastService.showErrorToast(
                                this.translateService.translate('global.txtErrorTitle'),
                                this.translateService.translate('errors.' + formatError(el))
                            );
                            // if backend has an error/s, turn off the default message
                            showDefaultError = false;
                        }
                    });
                    // if not error was recognized by the storage, throw default message once
                    if (showDefaultError) {
                        this.toastService.showErrorToast(
                            this.translateService.translate('global.txtErrorTitle'),
                            this.translateService.translate('global.txtUnexpectedError')
                        );
                    }
                },
            });
    }

    closeCertificateHistory() {
        this.showHistory = false;
    }

    onCancelEdit(inplace: any) {
        inplace.deactivate();
        this.form.markAsPristine();
    }

    showEdit(inplace: any) {
        inplace.activate();
    }

    changeAlias(inplace: any, id: string) {
        if (this.aliasControl.valid && this.aliasControl.value && id) {
            this.requestInProgress = true;
            this.eidManagementService
                .updateAlias(this.aliasControl.value, id)
                .pipe(first())
                .subscribe({
                    next: () => {
                        this.toastService.showSuccessToast(
                            this.translateService.translate('global.txtSuccessTitle'),
                            this.translateService.translate('modules.eidManagement.txtSuccessfulAliasChange')
                        );
                        inplace.deactivate();
                        this.requestInProgress = false;
                        this.refreshTable = true;
                        if (this.certificateId) {
                            this.getCertificateData(this.certificateId);
                        } else {
                            this.getCertificateData(this.data.id);
                        }
                    },
                    error: err => {
                        inplace.deactivate();
                        this.requestInProgress = false;
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
                    },
                });
        }
    }

    handleAliasEdit() {
        this.aliasControl.setValue(this.certificate?.alias);
        this.aliasControl.updateValueAndValidity();
    }

    onSubmit() {
        this.closeRevokeModal();
        this.requestInProgress = true;
        // Extracting form value for better readability
        if (this.form.valid) {
            const { personalIdentityDocument } = this.form.controls;
            const originalIdentityIssueDate = personalIdentityDocument.controls.identityIssueDate.value as string;
            const originalIdentityValidityToDate = personalIdentityDocument.controls.identityValidityToDate
                .value as string;

            // Patching the form value with updated Date objects
            this.form.patchValue({
                personalIdentityDocument: {
                    identityIssueDate: this.formatDate(new Date(originalIdentityIssueDate)),
                    identityValidityToDate: this.formatDate(new Date(originalIdentityValidityToDate)),
                },
            });
            this.eidManagementService
                .certificateStatusChangePlain(this.form.value)
                .pipe(first())
                .subscribe({
                    next: (response: any) => {
                        if (response.fee > 0) {
                            this.getCertificateData(this.certificateId ? this.certificateId : this.data.id);
                            this.toastService.showSuccessToast(
                                this.translateService.translate('global.txtSuccessTitle'),
                                this.translateService.translate('modules.eidManagement.txtStatusChangeSubmitted')
                            );
                            this.userHasToPay = true;
                            this.tariff = {
                                fee: response.fee,
                                feeCurrency: response.feeCurrency,
                                secondaryFee: response.secondaryFee,
                                secondaryFeeCurrency: response.secondaryFeeCurrency,
                                paymentAccessCode: response.paymentAccessCode,
                                isPaymentRequired: response.isPaymentRequired,
                            };
                        } else {
                            this.showSuccessStatusChangeMessage();
                            this.finalizeStatusChange(response.id);
                        }
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
                        this.form.patchValue({
                            personalIdentityDocument: {
                                identityIssueDate: originalIdentityIssueDate,
                                identityValidityToDate: originalIdentityValidityToDate,
                            },
                        });
                        this.requestInProgress = false;
                        this.statusChangeRequest = true;
                        this.previewMode = false;
                        this.submitted = true;
                    },
                });
        }
    }

    markFormGroupTouched(formGroup: FormGroup) {
        Object.values(formGroup.controls).forEach(control => {
            if (control instanceof FormGroup) {
                this.markFormGroupTouched(control);
            } else {
                control.markAsTouched();
                control.markAsDirty();
            }
        });
    }

    finalizeStatusChange(id: string) {
        this.eidManagementService
            .completeCertificate(id)
            .pipe(first())
            .subscribe({
                next: () => {
                    this.getCertificateData(this.certificateId ? this.certificateId : this.data.id);
                    this.sendBackToDetails();
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
                },
            });
    }

    cancel() {
        this.statusChangeRequest = false;
        this.submitted = false;
        this.hide.emit({ showPreview: false, refreshTable: false, statusChangeRequest: false });
    }

    formatDateForUI(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
            timeStyle: 'medium',
        });
    }

    formatDate(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
    }

    showPreview() {
        if (this.form.valid) {
            this.previewMode = true;
            this.submitted = false;
            this.form.get('citizenIdentifierType')?.enable();
            this.form.get('citizenIdentifierNumber')?.enable();
        } else {
            this.submitted = true;
            this.markFormGroupTouched(this.form);
        }
    }

    handleReasonSelect(event: any) {
        const findReason = this.reasons.find((reason: any) => reason.id === event.value);
        if (findReason?.textRequired) {
            this.isDescriptionMandatory = true;
            this.form.patchValue({ reasonText: null });
            this.form.controls['reasonText'].setValidators([Validators.required]);
            this.form.controls['reasonText'].markAsPristine();
        } else {
            this.isDescriptionMandatory = false;
            this.form.patchValue({ reasonText: null });
            this.form.controls['reasonText'].clearValidators();
        }
        this.form.controls['reasonText'].updateValueAndValidity();
    }

    hidePreview() {
        this.form.get('citizenIdentifierType')?.disable();
        this.form.get('citizenIdentifierNumber')?.disable();
        this.previewMode = false;
        this.showHistory = false;
    }

    showCertificateHistory() {
        this.showHistory = true;
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

    translateLevelOfAssurance(level: ILevelOfAssurances) {
        switch (level) {
            case ILevelOfAssurances.HIGH:
                return this.translateService.translate('modules.eidManagement.txtHigh');
            case ILevelOfAssurances.SUBSTANTIAL:
                return this.translateService.translate('modules.eidManagement.txtSubstantial');
            default:
                return level;
        }
    }

    showSuccessStatusChangeMessage() {
        this.toastService.showSuccessToast(
            this.translateService.translate('global.txtSuccessTitle'),
            this.translateService.translate('modules.eidManagement.txtSuccessfulStatusChange')
        );
    }

    showErrorStatusChangeMessage() {
        this.toastService.showErrorToast(
            this.translateService.translate('global.txtErrorTitle'),
            this.translateService.translate('modules.eidManagement.errors.txtStatusChange')
        );
    }

    openModal() {
        this.isOpen = true;
    }

    closeModal() {
        this.isOpen = false;
    }

    openRevokeModal() {
        this.isRevokeOpen = true;
    }

    closeRevokeModal() {
        this.isRevokeOpen = false;
    }

    administratorOfficeInfoRequest(requestedProperty: string) {
        if (!this.administratorOffices) {
            return '';
        }
        const office = this.administratorOffices.find(
            (office: IEidAdministratorOffice) => this.certificate.eidAdministratorOfficeId === office.id
        );
        if (!office) {
            return '';
        }
        return office[`${requestedProperty}`] || '';
    }

    sendBackToDetails() {
        this.userHasToPay = false;
        this.statusChangeRequest = false;
        this.previewMode = false;
        this.requestInProgress = false;
        this.refreshTable = true;
        this.showRevokeMessage = false;
    }

    redirectToPayment(accessCode: string): void {
        window.open(`${this.paymentUrl}${accessCode}`, '_blank');
    }

    toUpperCase(event: Event): void {
        const input = event.target as HTMLInputElement;
        input.value = input.value.toUpperCase();
    }

    protected readonly ApplicationTypes = ApplicationTypes;

    generateXml(): void {
        this.submitted = true;
        this.requestInProgress = true;
        if (this.form.valid) {
            this.form.get('citizenIdentifierType')?.enable();
            this.form.get('citizenIdentifierNumber')?.enable();
            // Extracting form value for better readability
            const originalIdentityIssueDate = this.form.controls.personalIdentityDocument.controls.identityIssueDate
                .value as string;
            const originalIdentityValidityToDate = this.form.controls.personalIdentityDocument.controls
                .identityValidityToDate.value as string;
            const originalIDateOfBirth = this.form.controls.dateOfBirth.value as string;
            // Patching the form value with updated Date objects
            this.form.patchValue({
                personalIdentityDocument: {
                    identityIssueDate: this.formatDate(new Date(originalIdentityIssueDate)),
                    identityValidityToDate: this.formatDate(new Date(originalIdentityValidityToDate)),
                },
                dateOfBirth: this.formatDate(new Date(originalIDateOfBirth)),
            });
            const payload = {
                ...this.form.value,
                deviceId: this.certificate.deviceId,
                eidAdministratorOfficeId: this.certificate.eidAdministratorOfficeId,
                eidAdministratorId: this.certificate.eidAdministratorId,
            };
            this.eidManagementService
                .generateApplicationXml(payload)
                .pipe(first())
                .subscribe({
                    next: (responseXml: string) => {
                        const jsonObject = JSON.parse(responseXml);
                        this.responseXml = jsonObject.xml;
                        this.signatureForm.patchValue({ xml: this.responseXml });
                        this.requestInProgress = false;
                        this.onSignatureSubmit();
                    },
                    error: err => {
                        let showDefaultError = true;
                        JSON.parse(err).errors?.forEach((el: string) => {
                            if (errorDataStorage.has(el)) {
                                this.toastService.showErrorToast(
                                    this.translateService.translate('global.txtErrorTitle'),
                                    this.translateService.translate('errors.' + formatError(el))
                                );
                                // if backend has an error/s, turn off the default message
                                showDefaultError = false;
                            }
                        });
                        // if not error was recognized by the storage, throw default message once
                        if (showDefaultError) {
                            this.toastService.showErrorToast(
                                this.translateService.translate('global.txtErrorTitle'),
                                this.translateService.translate('global.txtUnexpectedError')
                            );
                        }
                        this.form.patchValue({
                            personalIdentityDocument: {
                                identityIssueDate: originalIdentityIssueDate,
                                identityValidityToDate: originalIdentityValidityToDate,
                            },
                        });
                        this.requestInProgress = false;
                    },
                });
        } else {
            // Mark invalid controls as touched
            this.markFormGroupTouched(this.form);
            this.requestInProgress = false;
        }
    }

    /////////////////////////// SIGNATURE METHODS BELOW //////////////////////////////
    get showNexuNotDetectedMessage(): boolean | null {
        return (
            this.submitted &&
            !this.loading &&
            !this.nexuDetected &&
            this.selectedSignatureProvider &&
            this.selectedSignatureProvider === SignatureProvider.KEP
        );
    }

    onSignatureOptionsChange(e: any): void {
        this.submitted = false;
        this.selectedSignatureProvider = e.value;
        this.retryCountsLeft = this.retryCountIfServerError;
    }

    onSignatureSubmit(): void {
        this.submitted = true;
        if (this.signatureForm.get('signatureMethod')?.valid) {
            switch (this.signatureForm.get('signatureMethod')?.value) {
                case SignatureProvider.KEP:
                    this.startPhysicalSignatureSign();
                    break;
                case SignatureProvider.Borica:
                    this.signWithBorica();
                    break;
                case SignatureProvider.Evrotrust:
                    this.signWithEvrotrust();
                    break;
            }
        } else {
            this.submitted = false;
            this.markFormGroupTouched(this.signatureForm);
        }
    }

    async isNexuIsRunning(): Promise<boolean> {
        return this.signService
            .getNexuJSVersion()
            .then((response: any) => {
                if (response.ok) {
                    return response.json();
                }
                throw new Error('Something went wrong');
            })
            .then(() => {
                return true;
            })
            .catch(() => {
                return false;
            });
    }

    async startPhysicalSignatureSign(): Promise<void> {
        this.showLoader();
        this.nexuDetected = await this.isNexuIsRunning();
        if (this.nexuDetected) {
            window.nexu_get_certificates(
                async (certificate: any) => {
                    if (certificate.response !== null && this.responseXml) {
                        certificate = certificate.response;
                        const digest = await this.getDigest(this.responseXml);
                        const payloadForDigest: KEPDigestRequestData = {
                            digestToSign: digest,
                            documentName: this.documentFileName,
                            signingCertificate: certificate.certificate,
                            certificateChain: certificate.certificateChain,
                            encryptionAlgorithm: certificate.encryptionAlgorithm,
                            signingDate: new Date(),
                        };

                        this.signService.digestData(payloadForDigest).subscribe({
                            next: response => {
                                window.nexu_sign_with_token_infos(
                                    certificate.tokenId.id,
                                    certificate.keyId,
                                    response.dataToSign,
                                    'SHA256',
                                    (signatureData: any) => {
                                        const payloadForSign: KEPSignRequestData = {
                                            ...payloadForDigest,
                                            ...{ signatureValue: signatureData.response.signatureValue },
                                        };
                                        this.signService.signData(payloadForSign).subscribe({
                                            next: data => {
                                                this.uploadSignedFile({
                                                    content: data.bytes,
                                                    contentType: data.mimeType['mimeTypeString'],
                                                    fileName: data.name,
                                                });
                                                this.hideLoader();
                                            },
                                            error: () => {
                                                this.hideLoader();
                                                this.showErrorToastAndEnableForm(
                                                    'modules.authorizationStatement.electronicSignature.txtSignatureErrorMessage'
                                                );
                                            },
                                        });
                                    },
                                    () => {
                                        this.hideLoader();
                                        this.showErrorToastAndEnableForm(
                                            'modules.authorizationStatement.electronicSignature.txtSignatureErrorMessage'
                                        );
                                    }
                                );
                            },
                            error: () => {
                                this.hideLoader();
                                this.showErrorToastAndEnableForm(
                                    'modules.authorizationStatement.electronicSignature.txtSignatureErrorMessage'
                                );
                            },
                        });
                    } else {
                        this.hideLoader();
                        this.showErrorToastAndEnableForm(
                            'modules.authorizationStatement.electronicSignature.txtSignatureErrorMessage'
                        );
                    }
                },
                () => {
                    this.hideLoader();
                    this.showErrorToastAndEnableForm(
                        'modules.authorizationStatement.electronicSignature.txtSignatureErrorMessage'
                    );
                }
            );
        } else {
            this.hideLoader();
        }
    }

    signWithBorica(): void {
        this.showLoader();
        this.signService.checkUserBorica(this.form.controls.citizenIdentifierNumber.value as string).subscribe({
            next: response => {
                if (response.responseCode === 'OK') {
                    const signBody: BoricaSignRequestData = {
                        contents: [
                            {
                                confirmText: `Confirm sign`,
                                contentFormat: 'BINARY_BASE64',
                                mediaType: 'text/xml',
                                data: Base64.encode(this.responseXml),
                                fileName: `${this.documentFileName}.xml`,
                                padesVisualSignature: true,
                                signaturePosition: {
                                    imageHeight: 20,
                                    imageWidth: 100,
                                    imageXAxis: 20,
                                    imageYAxis: 20,
                                    pageNumber: 1,
                                },
                            },
                        ],
                        uid: this.form.controls.citizenIdentifierNumber.value as string,
                    };

                    this.signService.signDataBorica(signBody).subscribe({
                        next: response => {
                            this.signTransactions[SignatureProvider.Borica] = response;
                            this.handleDocumentStatusCheck(SignatureProvider.Borica);
                        },
                        error: () => {
                            this.showErrorToastAndEnableForm(
                                'modules.authorizationStatement.electronicSignature.txtSignatureProviderErrorMessage'
                            );
                        },
                    });
                } else {
                    this.showErrorToastAndEnableForm(
                        'modules.authorizationStatement.electronicSignature.txtSignatureProviderErrorMessage'
                    );
                }
            },
            error: () => {
                this.showErrorToastAndEnableForm(
                    'modules.authorizationStatement.electronicSignature.txtSignatureProviderErrorMessage'
                );
            },
        });
    }

    signWithEvrotrust(): void {
        this.showLoader();
        this.signService.checkUserEvrotrust(this.form.controls.citizenIdentifierNumber.value as string).subscribe({
            next: response => {
                if (response['isReadyToSign']) {
                    const dateExpire = new Date();
                    dateExpire.setDate(dateExpire.getDate() + 1);

                    const signBody = {
                        dateExpire: dateExpire,
                        documents: [
                            {
                                content: Base64.encode(this.responseXml),
                                fileName: `${this.documentFileName}.xml`,
                                contentType: 'text/xml',
                            },
                        ],
                        uid: this.form.controls.citizenIdentifierNumber.value as string,
                    };

                    this.signService.signDataEvrotrust(signBody).subscribe({
                        next: response => {
                            this.signTransactions[SignatureProvider.Evrotrust] = response;
                            this.handleDocumentStatusCheck(SignatureProvider.Evrotrust);
                        },
                        error: () => {
                            this.showErrorToastAndEnableForm(
                                'modules.authorizationStatement.electronicSignature.txtSignatureProviderErrorMessage'
                            );
                        },
                    });
                } else {
                    this.showErrorToastAndEnableForm(
                        'modules.authorizationStatement.electronicSignature.txtSignatureProviderErrorMessage'
                    );
                }
            },
            error: () => {
                this.showErrorToastAndEnableForm(
                    'modules.authorizationStatement.electronicSignature.txtSignatureProviderErrorMessage'
                );
            },
        });
    }

    handleDocumentStatusCheck(type: SignatureProvider): void {
        const TIMEOUT_DURATION = 3 * 60 * 1000; // 3 minutes
        const startTime = Date.now();
        switch (type) {
            case SignatureProvider.Evrotrust:
                timer(0, this.INTERVAL)
                    .pipe(
                        switchMap(() => {
                            const elapsedTime = Date.now() - startTime;
                            if (elapsedTime > TIMEOUT_DURATION) {
                                this.closeTimer$.next();
                                this.hideLoader();
                                this.showErrorToastAndEnableForm(
                                    'modules.authorizationStatement.electronicSignature.txtErrorTimedOut'
                                );
                            }
                            return this.signService.checkSignedDataEvrotrust({
                                transactionId:
                                    this.signTransactions[SignatureProvider.Evrotrust].transactions[0].transactionID,
                                groupSigning: this.signTransactions[SignatureProvider.Evrotrust].groupSigning,
                            });
                        }),
                        takeUntil(this.closeTimer$) // close the subscription when `closeTimer$` emits
                    )
                    .subscribe({
                        next: response => {
                            if (response.status === SignatureStatus.Signed && !response.isProcessing) {
                                this.closeTimer$.next();
                                this.signService
                                    .getSignedDataEvrotrust({
                                        transactionId:
                                            this.signTransactions[SignatureProvider.Evrotrust].transactions[0]
                                                .transactionID,
                                        groupSigning: this.signTransactions[SignatureProvider.Evrotrust].groupSigning,
                                    })
                                    .subscribe(data => {
                                        this.uploadSignedFile(data[0]);
                                    });
                            } else if (response.status === SignatureStatus.Rejected) {
                                this.closeTimer$.next();
                                this.hideLoader();
                            }
                        },
                        error: () => {
                            if (this.retryCountsLeft > 0) {
                                this.retryCountsLeft -= 1;
                                this.handleDocumentStatusCheck(SignatureProvider.Evrotrust);
                            } else {
                                this.showErrorToastAndEnableForm(
                                    'modules.authorizationStatement.electronicSignature.txtSignatureErrorMessage'
                                );
                            }
                        },
                    });
                break;
            case SignatureProvider.Borica:
                timer(0, this.INTERVAL)
                    .pipe(
                        switchMap(() => {
                            const elapsedTime = Date.now() - startTime;
                            if (elapsedTime > TIMEOUT_DURATION) {
                                this.closeTimer$.next();
                                this.hideLoader();
                                this.showErrorToastAndEnableForm(
                                    'modules.authorizationStatement.electronicSignature.txtErrorTimedOut'
                                );
                            }
                            return this.signService.checkSignedDataBorica({
                                transactionId: this.signTransactions[SignatureProvider.Borica].data.callbackId,
                            });
                        }),
                        takeUntil(this.closeTimer$) // close the subscription when `closeTimer$` emits
                    )
                    .subscribe({
                        next: response => {
                            if (
                                response.responseCode === BoricaSignatureStatus.COMPLETED &&
                                response.data.signatures[0].status === BoricaSignatureStatus.SIGNED
                            ) {
                                this.closeTimer$.next();
                                this.signService
                                    .getSignedDataBorica({
                                        transactionId: response.data.signatures[0].signature as string,
                                    })
                                    .subscribe(data => {
                                        this.uploadSignedFile(data);
                                    });
                            } else if (
                                response.responseCode === BoricaSignatureStatus.COMPLETED &&
                                response.data.signatures[0].status === BoricaSignatureStatus.REJECTED
                            ) {
                                this.closeTimer$.next();
                                this.hideLoader();
                            }
                        },
                        error: () => {
                            if (this.retryCountsLeft > 0) {
                                this.retryCountsLeft -= 1;
                                this.handleDocumentStatusCheck(SignatureProvider.Borica);
                            } else {
                                this.showErrorToastAndEnableForm(
                                    'modules.authorizationStatement.electronicSignature.txtSignatureErrorMessage'
                                );
                            }
                        },
                    });
                break;
        }
    }

    uploadSignedFile(data: any): void {
        if (this.selectedSignatureProvider && this.responseXml) {
            this.eidManagementService
                .certificateStatusChangeSigned({
                    xml: Base64.encode(this.responseXml),
                    signatureProvider: this.selectedSignatureProvider,
                    signature: data.content,
                })
                .subscribe({
                    next: res => {
                        this.hideLoader();
                        this.showSuccessToast(
                            this.translateService.translate(
                                'modules.authorizationStatement.electronicSignature.txtSignatureSuccessMessage'
                            )
                        );
                        if (res.isPaymentRequired) {
                            this.getCertificateData(this.certificateId ? this.certificateId : this.data.id);
                            this.userHasToPay = true;
                            this.tariff = {
                                fee: res.fee,
                                feeCurrency: res.feeCurrency,
                                secondaryFee: res.secondaryFee,
                                secondaryFeeCurrency: res.secondaryFeeCurrency,
                                paymentAccessCode: res.paymentAccessCode,
                                isPaymentRequired: res.isPaymentRequired,
                            };
                        } else {
                            this.finalizeStatusChange(res.id);
                        }
                    },
                    error: error => {
                        this.hideLoader();
                        let showDefaultError = true;
                        switch (error.status) {
                            case 400:
                                error.errors?.forEach((el: string) => {
                                    if (errorDataStorage.has(el)) {
                                        this.toastService.showErrorToast(
                                            this.translateService.translate('global.txtErrorTitle'),
                                            this.translateService.translate('errors.' + formatError(el))
                                        );
                                        // if backend has an error/s, turn off the default message
                                        showDefaultError = false;
                                    }
                                });
                                // if not error was recognized by the storage, throw default message once
                                if (showDefaultError) {
                                    this.toastService.showErrorToast(
                                        this.translateService.translate('global.txtErrorTitle'),
                                        this.translateService.translate('global.txtUnexpectedError')
                                    );
                                }
                                break;
                            case 403:
                                this.showErrorToast(this.translateService.translate('global.txtNoPermissionsMessage'));
                                break;
                            case 409:
                                this.showErrorToast(
                                    this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtSignConflictErrorMessage'
                                    )
                                );
                                break;
                            default:
                                this.showErrorToast(
                                    this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtSignatureErrorMessage'
                                    )
                                );
                                break;
                        }
                    },
                });
        }
    }

    showLoader(): void {
        this.signatureForm.controls['signatureMethod'].disable();
        this.loading = true;
    }

    hideLoader(): void {
        this.signatureForm.controls['signatureMethod'].enable();
        this.loading = false;
    }

    showErrorToastAndEnableForm(errorMessageKey: string): void {
        this.showErrorToast(this.translateService.translate(errorMessageKey));
        this.hideLoader();
    }

    showErrorToast(message: string): void {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string): void {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    async getDigest(file: string): Promise<string> {
        //convert file to bytes and get digest
        const byteArray = new TextEncoder().encode(file);
        const hash = await crypto.subtle.digest('SHA-256', byteArray);
        //convert bytes back to base64
        const bytes = new Uint8Array(hash);
        const len = bytes.byteLength;
        let binary = '';
        for (let i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return btoa(binary);
    }
}
