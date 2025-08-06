import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { Subject, Subscription, first, switchMap, takeUntil, timer } from 'rxjs';
import { DeviceType, IdentifierType } from 'src/app/features/authorization-register/enums/authorization-register.enum';
import { DocumentType } from 'src/app/features/authorization-register/enums/authorization-register.enum';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { ToastService } from 'src/app/shared/services/toast.service';
import { EGNAdultValidator, EGNValidator, PinOrAdultEGNValidator, PinValidator } from 'src/app/shared/validators/egn';
import { EidManagementService } from '../../services/eid-management.service';
import { dateMoreThanOrEqualValidate, isDateBelowLawfulAge } from 'src/app/shared/validators/date';
import { DatePipe } from '@angular/common';
import { Base64 } from 'js-base64';
import {
    SignatureProvider,
    SignatureStatus,
    BoricaSignatureStatus,
} from 'src/app/features/authorization-register/enums/document-sign.enum';
import {
    KEPDigestRequestData,
    KEPSignRequestData,
    BoricaSignRequestData,
    SignTransactions,
} from 'src/app/features/authorization-register/interfaces/document-sign.interfaces';
import { SignClientService } from 'src/app/features/authorization-register/services/sign-client.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { requiredLatinNamesValidator } from 'src/app/shared/validators/names';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import {
    IEidAdministrator,
    IEidAdministratorOffice,
    IDevices,
    IStep,
    Tariff,
} from '../../interfaces/eid-management.interface';
import { RaeiceiClientService } from '../../services/raeicei-client.service';
import * as moment from 'moment';
import { AppConfigService } from 'src/app/core/services/config.service';
import { OfficeType } from '../../enums/eid-management';
import { Router } from '@angular/router';
import { AccessTypes } from 'src/app/shared/enums/access-types';

declare global {
    interface Window {
        nexu_get_certificates: any;
        nexu_sign_with_token_infos: any;
    }
}
@Component({
    selector: 'app-new-application-form',
    templateUrl: './new-application-form.component.html',
    styleUrls: ['./new-application-form.component.scss'],
    providers: [ConfirmationService, DatePipe],
})
export class NewApplicationFormComponent implements OnInit, OnDestroy {
    breadcrumbItems: IBreadCrumbItems[] = this._initialBreadcrumbs;
    subscriptions: Subscription[] = [];
    administratorsList!: IEidAdministrator[];
    devicesList!: IDevices[];
    devicesListView!: IDevices[];
    signatureForm = new FormGroup({
        xml: new FormControl<string | null>(null),
        signatureMethod: new FormControl<string | null>(null, [Validators.required]),
        signature: new FormControl<string | null>(null, [Validators.required]),
    });
    steps: IStep[] = [];
    currentStep = 0;
    submitted = false;
    requestInProgress!: boolean;
    officeList!: any[];
    officeListView!: any[];
    responseXml!: string;
    selectedAdministrator!: IEidAdministrator;
    latinPatternFirstOrSecondName = /^(?=.{1,40}$)[a-zA-Z\s-']*$/;
    latinPatternLastName = /^(?=.{1,60}$)[a-zA-Z\s-']*$/;
    form = new FormGroup({
        firstName: new FormControl<string | null>(null),
        secondName: new FormControl<string | null>(null),
        lastName: new FormControl<string | null>(null),
        firstNameLatin: new FormControl<string | null>(null),
        secondNameLatin: new FormControl<string | null>(null),
        lastNameLatin: new FormControl<string | null>(null),
        applicationType: new FormControl<string>('ISSUE_EID'),
        deviceId: new FormControl<string | null>({ value: null, disabled: true }, [Validators.required]),
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
        }),
        eidAdministratorId: new FormControl<string | null>(null, [Validators.required]),
        eidAdministratorOfficeId: new FormControl<string | null>({ value: null, disabled: true }, [
            Validators.required,
        ]),
        dateOfBirth: new FormControl<string | null>(null, [Validators.required, isDateBelowLawfulAge('dateOfBirth')]),
    });
    nexuDetected = false;
    isOpen!: boolean;
    tariff!: Tariff;
    userHasToPay!: boolean;
    accessTypes = AccessTypes;
    isSignatureRequired = JSON.parse(sessionStorage.getItem('user') || '{}')?.acr === this.accessTypes.LOW;
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
    loading = false;
    INTERVAL = 5000;
    closeTimer$ = new Subject<void>();
    documentFileName = 'ApplicationForm';
    retryCountIfServerError = 2;
    retryCountsLeft = this.retryCountIfServerError;
    paymentUrl = this.appConfigService.config.externalLinks.paymentByAccessCodeUrl;
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

    get _initialBreadcrumbs(): IBreadCrumbItems[] {
        return ([] as IBreadCrumbItems[]).concat([
            {
                label: this.translateService.translate('modules.eidManagement.txtTitle'),
                routerLink: '/eid-management',
            },
            { label: this.translateService.translate('modules.eidManagement.txtRequestCertificate') },
        ]);
    }

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

    constructor(
        public translateService: TranslocoService,
        private translocoLocaleService: TranslocoLocaleService,
        private eidManagementService: EidManagementService,
        private toastService: ToastService,
        private raeiceiService: RaeiceiClientService,
        private signService: SignClientService,
        private appConfigService: AppConfigService,
        private router: Router
    ) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtTitle'),
                    routerLink: '/eid-management',
                },
                { label: this.translateService.translate('modules.eidManagement.txtRequestCertificate') },
            ];

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
            this.generateSteps();
        });
        this.subscriptions.push(languageChangeSubscription);
    }

    ngOnInit() {
        this.breadcrumbItems = this._initialBreadcrumbs;
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
        this.setupFormChangeListeners();
        this.getEidentity();
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
        this.closeTimer$.next();
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

    getAdministratorsFrontOffices(id: string) {
        const offices = this.raeiceiService.fetchOffices(id).subscribe((office: IEidAdministratorOffice[]) => {
            this.officeList = office;
            this.officeListView = office;
        });
        this.subscriptions.push(offices);
    }

    navigateToOffices() {
        window.open(`home/administrators/${this.selectedAdministrator.id}`, '_blank');
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

    onSubmit() {
        if (!this.checkForeignerEligibility()) {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('global.txtForeignerError')
            );
            return;
        }
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
            this.eidManagementService
                .generateApplicationXml(this.form.value)
                .pipe(first())
                .subscribe({
                    next: (responseXml: string) => {
                        const jsonObject = JSON.parse(responseXml);
                        this.responseXml = jsonObject.xml;
                        this.signatureForm.patchValue({ xml: this.responseXml });
                        this.currentStep = this.currentStep + 1;
                        this.requestInProgress = false;
                    },
                    error: err => {
                        let showDefaultError = true;
                        JSON.parse(err).errors?.forEach((el: string) => {
                            if (errorDataStorage.has(el)) {
                                this.toastService.showErrorToast(
                                    this.translateService.translate('global.txtErrorTitle'),
                                    this.translateService.translate('errors.' + formatError(el))
                                );
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
                            dateOfBirth: originalIDateOfBirth,
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

    generateSteps() {
        this.steps = [
            {
                id: 1,
                name: this.translateService.translate('modules.eidManagement.steps.txtPersonalDataStep'),
            },
            {
                id: 2,
                name: this.isSignatureRequired
                    ? this.translateService.translate('modules.eidManagement.steps.txtPreviewAndSign')
                    : this.translateService.translate('modules.eidManagement.steps.txtPreview'),
            },
            {
                id: 3,
                name: this.translateService.translate('modules.eidManagement.steps.txtConfirmationStep'),
            },
        ];
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

    handleGoToPreviousStep() {
        this.requestInProgress = true;
        this.currentStep = this.currentStep - 1;
        this.requestInProgress = false;
        this.form.get('citizenIdentifierType')?.disable();
        this.form.get('citizenIdentifierNumber')?.disable();
        this.closeTimer$.next();
    }

    onAdministratorChange(event: any) {
        const selectedAdministrator = this.administratorsList.find(
            (administrator: any) => administrator.id === event.value
        );
        if (selectedAdministrator) {
            this.selectedAdministrator = selectedAdministrator;
            this.form.get('eidAdministratorOfficeId')?.setValue(null);
            this.form.get('eidAdministratorOfficeId')?.disable();
            this.getAdministratorsFrontOffices(selectedAdministrator.id);
            this.form.get('deviceId')?.enable();
            this.form.get('deviceId')?.setValue(null);
            this.form.get('deviceId')?.setValidators(Validators.required);
            this.form.get('deviceId')?.updateValueAndValidity();
            this.devicesListView = this.devicesList.filter(device =>
                selectedAdministrator.deviceIds.includes(device.id)
            );
        }
    }

    onDeviceChange(event: any) {
        const deviceType = this.devicesList.find((device: any) => device.id === event.value);
        if (deviceType?.type !== DeviceType.MOBILE && this.officeList) {
            this.officeListView = this.officeList.filter((el: any) => el.name !== OfficeType.ONLINE);
        } else {
            this.officeListView = this.officeList.filter((el: any) => el.name === OfficeType.ONLINE);
        }
        this.form.get('eidAdministratorOfficeId')?.enable();
        this.form.get('eidAdministratorOfficeId')?.setValue(null);
        this.form.get('eidAdministratorOfficeId')?.setValidators(Validators.required);
        this.form.get('eidAdministratorOfficeId')?.updateValueAndValidity();
    }

    translateDeviceName() {
        const language = this.translocoLocaleService.getLocale();
        if (language === 'bg-BG') {
            return 'name';
        } else {
            return 'description';
        }
    }

    formatDate(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
    }

    checkForeignerEligibility() {
        const identityType = this.form.controls.citizenIdentifierType.value as string;
        const eidAdministratorId = this.form.controls.eidAdministratorId.value as string;
        const administrator = this.administratorsList.find(administrator => administrator.id === eidAdministratorId);
        if (identityType === IdentifierType.LNCh && administrator?.name === 'МВР') {
            return false;
        }
        return true;
    }

    deviceTypeTranslation() {
        const deviceId = this.form.controls.deviceId.value as string;
        const device = this.devicesList.find((device: any) => device.id === deviceId);
        const language = this.translocoLocaleService.getLocale();
        if (language === 'bg-BG') {
            return device?.name;
        } else {
            return device?.description;
        }
    }

    identityTypeTranslation(type: DocumentType | null) {
        switch (type) {
            case DocumentType.ID_CARD:
                return this.translateService.translate('modules.eidManagement.documentTypeOptions.personalIdCard');
            case DocumentType.PASSPORT:
                return this.translateService.translate('modules.eidManagement.documentTypeOptions.passport');
            case DocumentType.PASSPORT2:
                return this.translateService.translate('modules.eidManagement.documentTypeOptions.passport2');
            default:
                return type;
        }
    }

    getAdministratorName() {
        if (!this.administratorsList) {
            return '';
        }
        const administrator = this.administratorsList.find(
            (administrator: any) => this.form.value.eidAdministratorId === administrator.id
        );
        return administrator?.name ? administrator.name : '';
    }

    administratorOfficeInfoRequest(requestedProperty: string) {
        if (!this.officeList) {
            return '';
        }
        const office = this.officeList.find(
            (office: IEidAdministratorOffice) => this.form.value.eidAdministratorOfficeId === office.id
        );
        if (!office) {
            return '';
        }
        return office[`${requestedProperty}`] || '';
    }

    openModal() {
        this.isOpen = true;
    }

    closeModal() {
        this.isOpen = false;
    }

    calculateUserAge() {
        const currentDate = moment();
        const birthDate = moment(this.form.value.dateOfBirth);
        let age = currentDate.diff(birthDate, 'years');

        if (currentDate.isBefore(birthDate.add(age, 'years'))) {
            age -= 1;
        }

        return age;
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
                .createApplication({
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
                        this.currentStep = this.currentStep + 1;
                        if (res.isPaymentRequired) {
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
                            this.finalizeApplicationProcess(res.id);
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

    uploadUnsignedFile(): void {
        this.showLoader();
        this.eidManagementService
            .createApplication({
                xml: Base64.encode(this.responseXml),
            })
            .subscribe({
                next: res => {
                    this.hideLoader();
                    this.currentStep = this.currentStep + 1;
                    if (res.isPaymentRequired) {
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
                        this.finalizeApplicationProcess(res.id);
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

    finalizeApplicationProcess(applicationId: string) {
        this.eidManagementService
            .completeCertificate(applicationId)
            .pipe(first())
            .subscribe({
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

    redirectToPayment(accessCode: string): void {
        window.open(`${this.paymentUrl}${accessCode}`, '_blank');
        this.router.navigate(['/eid-management/applications']);
    }

    showLoader(): void {
        this.signatureForm.controls['signatureMethod'].disable();
        this.loading = true;
        this.requestInProgress = true;
    }

    hideLoader(): void {
        this.signatureForm.controls['signatureMethod'].enable();
        this.loading = false;
        this.requestInProgress = false;
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

    toUpperCase(event: Event): void {
        const input = event.target as HTMLInputElement;
        input.value = input.value.toUpperCase();
    }
}
