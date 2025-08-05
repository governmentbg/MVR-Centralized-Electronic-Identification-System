import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from '../../../../shared/services/toast.service';
import { Router } from '@angular/router';
import { IBreadCrumbItems } from '../../../authorization-register/interfaces/IBreadCrumbItems';
import { Subscription } from 'rxjs';
import { SmartCardCommunicationClientService } from '../../services/smart-card-communication-client.service';
import { MustMatch } from '../../../../shared/validators/match';
import { LogsService } from '../../services/logs.service';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
    selector: 'app-pin-change-form',
    templateUrl: './pin-change-form.component.html',
    styleUrls: ['./pin-change-form.component.scss'],
})
export class PinChangeFormComponent implements OnDestroy, OnInit {
    constructor(
        public translateService: TranslocoService,
        private toastService: ToastService,
        private router: Router,
        private smartCardCommunicationClientService: SmartCardCommunicationClientService,
        private oauthService: OAuthService,
        private deviceLogsService: LogsService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.cardManagement.txtTitle'),
                    routerLink: '/card-management',
                },
            ];
        });
    }

    form = new FormGroup(
        {
            readerName: new FormControl<string | null>(null, [Validators.required]),
            currentPIN: new FormControl<string | null>(null, [
                Validators.required,
                Validators.minLength(6),
                Validators.maxLength(6),
                Validators.pattern('[0-9]+'),
            ]),
            newPIN: new FormControl<string | null>(null, [
                Validators.required,
                Validators.minLength(6),
                Validators.maxLength(6),
                Validators.pattern('[0-9]+'),
            ]),
            repeatNewPIN: new FormControl<string | null>(null, [
                Validators.required,
                Validators.minLength(6),
                Validators.maxLength(6),
                Validators.pattern('[0-9]+'),
            ]),
        },
        {
            validators: MustMatch('newPIN', 'repeatNewPIN'),
        }
    );
    requestInProgress = false;
    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    isSmartCardServiceAvailable = false;
    loadingSmartCardService = true;
    cardReaders: string[] = [];
    submitted = false;
    numberRegex = /[\d]/;

    ngOnInit() {
        if (!this.isSmartCardServiceAvailable) {
            this.form.disable();
        }
        this.initSmartCardService();
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    onSubmit() {
        this.submitted = true;
        Object.keys(this.form.controls).forEach(key => {
            if (
                (this.form.get(key) && typeof this.form.get(key)?.value === 'string') ||
                this.form.get(key)?.value instanceof String
            ) {
                this.form.get(key)?.setValue(this.form.get(key)?.value.trim());
            }
            this.form.get(key)?.markAsDirty();
        });
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.requestInProgress = true;
            const accessToken = this.oauthService.getAccessToken();
            this.smartCardCommunicationClientService
                .changePin({
                    readerName: this.form.controls.readerName.value as string,
                    currentPin: this.form.controls.currentPIN.value as string,
                    newPin: this.form.controls.newPIN.value as string,
                    token: accessToken,
                })
                .subscribe({
                    next: response => {
                        this.form.reset();
                        this.showSuccessToast(
                            this.translateService.translate('modules.cardManagement.txtPinChangeSuccess')
                        );
                        this.deviceLogsService
                            .submitDeviceLog({
                                eventType: 'ONLINE_SUCCESSFUL_PIN_CHANGE',
                                eventPayload: { certificateId: response.certificateSerialNumber },
                            })
                            .subscribe({
                                next: () => {
                                    this.requestInProgress = false;
                                },
                                error: () => {
                                    this.requestInProgress = false;
                                },
                            });
                    },
                    error: error => {
                        this.deviceLogsService
                            .submitDeviceLog({
                                eventType: 'ONLINE_UNSUCCESSFUL_PIN_CHANGE',
                                eventPayload: { certificateId: error.certificateSerialNumber },
                            })
                            .subscribe({
                                next: () => {
                                    this.requestInProgress = false;
                                },
                                error: () => {
                                    this.requestInProgress = false;
                                },
                            });
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
                                        const params = error.triesLeft ? { count: error.triesLeft } : {};
                                        this.toastService.showErrorDetailsToast(
                                            this.translateService.translate('global.txtErrorTitle'),
                                            this.translateService.translate(translationKey, params),
                                            errors
                                        );
                                    } else {
                                        this.showErrorToast(
                                            this.translateService.translate('modules.eidManagement.txtPINChangedError')
                                        );
                                    }
                                } else {
                                    this.showErrorToast(
                                        this.translateService.translate('modules.eidManagement.txtPINChangedError')
                                    );
                                }
                                break;
                        }
                    },
                });
        }
    }

    cancel() {
        this.router.navigate(['/card-management']);
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
                this.form.enable();
            },
            error: error => {
                this.loadingSmartCardService = false;
                this.requestInProgress = false;
                this.isSmartCardServiceAvailable = false;
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

    downloadSmartCardCommunicationService() {
        const link = document.createElement('a');
        link.href = 'https://pay.egov.bg/Home/DownloadFile/rulesElectronicServicePdf'; // This is a temporary link
        link.download = 'rulesElectronicServicePdf.pdf'; // This is a temporary name
        document.body.appendChild(link);
        link.click();
        link.remove();
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }
}
