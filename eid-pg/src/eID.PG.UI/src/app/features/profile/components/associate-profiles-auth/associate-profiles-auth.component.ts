import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { MessageService } from 'primeng/api';
import { AuthenticationService } from 'src/app/authentication/authentication-service/authentication-service.service';
import { IUser } from 'src/app/core/interfaces/IUser';
import { SmartCardCommunicationClientService } from 'src/app/features/card-management/services/smart-card-communication-client.service';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
    selector: 'app-associate-profiles-auth',
    templateUrl: './associate-profiles-auth.component.html',
    styleUrls: ['./associate-profiles-auth.component.scss'],
})
export class AssociateProfilesAuthComponent implements OnInit {
    @Output() closeDialog = new EventEmitter<{ associated: boolean }>();
    form = new FormGroup({
        readerName: new FormControl<string | null>({ value: null, disabled: true }, [Validators.required]),
        PIN: new FormControl<string | null>(null, [
            Validators.required,
            Validators.minLength(6),
            Validators.maxLength(6),
            Validators.pattern('[0-9]+'),
        ]),
        requestForm: new FormControl<null>(null),
        termsAgreement: new FormControl<boolean>(false, Validators.requiredTrue),
        leverOfAssurance: new FormControl<string>('LOW'),
        CAN: new FormControl<string | null>(null),
    });
    requestInProgress!: boolean;
    user: IUser | any = null;
    isSmartCardServiceAvailable = false;
    loadingSmartCardService = true;
    cardReaders: string[] = [];
    submitted = false;
    originUrl = '';
    realm = '';
    client_id = '';
    scope = '';
    system_id = '';
    PINIsSuspended = false;
    constructor(
        private authenticationService: AuthenticationService,
        private translocoService: TranslocoService,
        private toastService: ToastService,
        private messageService: MessageService,
        private smartCardCommunicationClientService: SmartCardCommunicationClientService
    ) {}

    ngOnInit(): void {
        if (!this.isSmartCardServiceAvailable) {
            this.form.disable();
        }
        this.initSmartCardService();
    }

    submit() {
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

            this.authenticationService.getAuthChallenge(this.form.value).subscribe({
                next: res => {
                    const challenge = res.challenge;
                    this.authenticateUser(challenge);
                },
                error: error => {
                    let showDefaultError = true;
                    error.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.toastService.showErrorToast(
                                this.translocoService.translate('global.txtErrorTitle'),
                                this.translocoService.translate('errors.' + formatError(el))
                            );
                            showDefaultError = false;
                        }
                    });
                    if (showDefaultError) {
                        this.toastService.showErrorToast(
                            this.translocoService.translate('global.txtErrorTitle'),
                            this.translocoService.translate('global.txtUnexpectedError')
                        );
                    }
                    this.markFormGroupTouched(this.form);
                    this.requestInProgress = false;
                },
            });
        } else {
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

    authenticateUser(challenge: string) {
        this.smartCardCommunicationClientService
            .signChallenge({
                readerName: this.form.controls.readerName.value as string,
                pin: this.form.controls.PIN.value as string,
                challenge: challenge,
                can: this.form.controls.CAN.value as string,
            })
            .subscribe({
                next: response => {
                    this.authenticationService
                        .associateProfile({
                            challenge: response.challenge,
                            signature: response.signature,
                            certificate: response.certificate,
                            certificateChain: response.certificateChain,
                        })
                        .subscribe({
                            next: () => {
                                this.requestInProgress = false;
                                this.closeDialog.emit({ associated: true });
                            },
                            error: error => {
                                let showDefaultError = true;
                                this.close();
                                error.errors?.forEach((el: string) => {
                                    if (errorDataStorage.has(el)) {
                                        this.messageService.add({
                                            severity: 'error',
                                            summary: this.translocoService.translate('global.txtErrorTitle'),
                                            detail: this.translocoService.translate('errors.' + formatError(el)),
                                        });
                                        showDefaultError = false;
                                    }
                                });
                                if (showDefaultError) {
                                    this.messageService.add({
                                        severity: 'error',
                                        summary: this.translocoService.translate('global.txtErrorTitle'),
                                        detail: this.translocoService.translate('global.txtUnexpectedError'),
                                    });
                                }
                                this.markFormGroupTouched(this.form);
                                this.requestInProgress = false;
                            },
                        });
                },
                error: error => {
                    this.requestInProgress = false;
                    this.isSmartCardServiceAvailable = false;
                    switch (error.status) {
                        case 400:
                            this.toastService.showErrorToast(
                                this.translocoService.translate('global.txtErrorTitle'),
                                this.translocoService.translate('global.txtInvalidDataError')
                            );
                            break;
                        case 404:
                            this.toastService.showErrorToast(
                                this.translocoService.translate('global.txtErrorTitle'),
                                this.translocoService.translate('global.txtPlugInReader')
                            );
                            break;
                        case 502:
                            switch (error.detail) {
                                case 'PINIsNotValid':
                                    this.toastService.showErrorToast(
                                        this.translocoService.translate('global.txtErrorTitle'),
                                        this.translocoService.translate('modules.cardManagement.PINIsNotValid')
                                    );
                                    break;
                                case 'PINIsSuspended':
                                    this.PINIsSuspended = true;
                                    this.toastService.showErrorToast(
                                        this.translocoService.translate('global.txtErrorTitle'),
                                        this.translocoService.translate('modules.cardManagement.PINIsNotValid')
                                    );
                                    break;
                                case 'PINIsBlocked':
                                    this.toastService.showErrorToast(
                                        this.translocoService.translate('global.txtErrorTitle'),
                                        this.translocoService.translate('modules.cardManagement.PINIsBlocked')
                                    );
                                    break;
                                case 'CANIsNotValid':
                                    this.toastService.showErrorToast(
                                        this.translocoService.translate('global.txtErrorTitle'),
                                        this.translocoService.translate('modules.cardManagement.CANIsNotValid')
                                    );
                                    break;
                            }
                            break;
                        default:
                            this.toastService.showErrorToast(
                                this.translocoService.translate('global.txtErrorTitle'),
                                this.translocoService.translate('global.txtUnexpectedError')
                            );
                            break;
                    }
                    this.form.reset();
                    if (this.PINIsSuspended) {
                        this.form.controls.CAN.setValidators([Validators.required]);
                    }
                },
            });
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
                        this.toastService.showErrorToast(
                            this.translocoService.translate('global.txtErrorTitle'),
                            this.translocoService.translate('global.txtInvalidDataError')
                        );
                        break;
                    default:
                        this.toastService.showErrorToast(
                            this.translocoService.translate('global.txtErrorTitle'),
                            this.translocoService.translate('global.txtNoConnectionReaders')
                        );
                        break;
                }
            },
        });
    }

    close() {
        this.closeDialog.emit({ associated: false });
    }
}
