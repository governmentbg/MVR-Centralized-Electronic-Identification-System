import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from '../../../../shared/services/toast.service';
import { SmartCardCommunicationClientService } from '../../services/smart-card-communication-client.service';
import {
    ICardCommunicationForm,
    IForeignIdentity,
    IGetCertificateInformationResponseData,
    IPersonalIdentity,
    IPINRequestData,
    IStep,
} from '../../interfaces/eid-management.interfaces';
import { Subscription } from 'rxjs';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AppConfigService } from '../../../../core/services/config.service';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
    selector: 'app-pin-change-form',
    templateUrl: './pin-change-form.component.html',
    styleUrls: ['./pin-change-form.component.scss'],
})
export class PinChangeFormComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private toastService: ToastService,
        private smartCardCommunicationClientService: SmartCardCommunicationClientService,
        private appConfigService: AppConfigService,
        private oAuthService: OAuthService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.generateSteps();
        });
    }

    @Input() personalIdentity!: IPersonalIdentity | IForeignIdentity | null;
    @Output() hide: EventEmitter<any> = new EventEmitter<any>();
    languageChangeSubscription: Subscription = new Subscription();
    steps: IStep[] = [];
    currentStep = 0;
    requestInProgress = false;
    loadingSmartCardService = false;
    isSmartCardServiceAvailable = false;
    cardReaders: string[] = [];
    certificateForm = new FormGroup<ICardCommunicationForm>({
        readerName: new FormControl(
            { value: '', disabled: true },
            { validators: Validators.required, nonNullable: true }
        ),
        can: new FormControl('', { validators: Validators.required, nonNullable: true }),
    });
    certificate: IGetCertificateInformationResponseData = {
        certificateInfo: '',
        isPinActivated: false,
        isGenuine: false,
        cardNumber: '',
    };

    ngOnInit() {
        this.initSmartCardService();
        if (this.PUKFieldIsVisible) {
            this.certificateForm.addControl(
                'puk',
                new FormControl('', { validators: Validators.required, nonNullable: true })
            );
        }
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    generateSteps() {
        this.steps = [
            {
                id: 1,
                name: this.translateService.translate('modules.eidManagement.txtSelectReader'),
                ref: 'selectReader',
            },
            {
                id: 2,
                name: this.translateService.translate('modules.eidManagement.txtCertificateInfo'),
                ref: 'certificateInfo',
            },
        ];
    }

    goToNextStep() {
        this.currentStep = this.currentStep + 1;
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
            this.smartCardCommunicationClientService
                .getCertificateInformation({
                    can: this.certificateForm.controls.can.value as string,
                    readerName: this.certificateForm.controls.readerName.value as string,
                })
                .subscribe({
                    next: response => {
                        this.goToNextStep();
                        this.requestInProgress = false;
                        this.certificate = response;
                    },
                    error: (error: any) => {
                        this.requestInProgress = false;
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
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
    }

    setPin() {
        this.requestInProgress = true;

        // We refresh the token so it does not expire while we set the PIN
        this.oAuthService.refreshToken().then(tokenResponse => {
            const requestData: IPINRequestData = {
                can: this.certificateForm.controls.can.value as string,
                puk: (this.certificateForm.controls.puk?.value as string) || '',
                readerName: this.certificateForm.controls.readerName.value as string,
                token: this.oAuthService.getAccessToken(),
            };

            this.smartCardCommunicationClientService.changePIN(requestData).subscribe({
                next: response => {
                    this.requestInProgress = false;
                    this.showSuccessToast(
                        this.translateService.translate('modules.eidManagement.txtPINChangedSuccess')
                    );
                    this.hide.emit();
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
                                    this.translateService.translate('modules.eidManagement.txtPINChangedError')
                                );
                            }
                            break;
                    }
                },
            });
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
        return this.certificate.cardNumber === this.personalIdentity?.identityDocumentNumber;
    }

    get PUKFieldIsVisible() {
        return this.appConfigService.config.uiFeatures?.showPUKField || false;
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }
}
