import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { AuthenticationService } from 'src/app/authentication/authentication-service/authentication-service.service';
import { IUser } from 'src/app/core/interfaces/IUser';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { ToastService } from 'src/app/shared/services/toast.service';
import { SmartCardCommunicationClientService } from '../../card-management/services/smart-card-communication-client.service';
import { ActivatedRoute } from '@angular/router';
import { Message } from 'primeng/api';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-personal-document-eid-login',
    templateUrl: './personal-document-eid-login.component.html',
    styleUrls: ['./personal-document-eid-login.component.scss'],
})
export class PersonalDocumentEidLoginComponent implements OnInit {
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
    errorMessage: Message[] = [];
    downloadLink: string | null = null;
    currentVersion = '';
    installerVersion = '';

    constructor(
        private authenticationService: AuthenticationService,
        private translocoService: TranslocoService,
        private toastService: ToastService,
        private smartCardCommunicationClientService: SmartCardCommunicationClientService,
        private route: ActivatedRoute,
        private http: HttpClient
    ) {}

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.originUrl = params['origin'];
            this.realm = params['realm'];
            this.client_id = params['client_id'];
            this.scope = params['scope'];
            this.system_id = params['system_id'];
        });
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
                            if (
                                'citizenProfileNotFound' === formatError(el) ||
                                'citizenProfileNotFoundById' === formatError(el)
                            ) {
                                this.errorMessage.push({
                                    severity: 'error',
                                    summary: this.translocoService.translate('global.txtErrorTitle'),
                                    detail: `<p>${this.translocoService.translate(
                                        'global.txtNoAttachedEid'
                                    )} <a href="${this.originUrl.replace(
                                        'login',
                                        'register'
                                    )}" target="_blank">${this.translocoService.translate('global.txtHere')}</a></p>`,
                                });
                            }
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
                        .codeFlowAuth(
                            {
                                challenge: response.challenge,
                                signature: response.signature,
                                certificate: response.certificate,
                                certificateChain: response.certificateChain,
                            },
                            this.originUrl,
                            this.client_id,
                            this.scope,
                            this.system_id
                        )
                        .subscribe({
                            next: () => {
                                this.requestInProgress = false;
                            },
                            error: error => {
                                let showDefaultError = true;
                                JSON.parse(error).errors.forEach((el: string) => {
                                    if (errorDataStorage.has(el)) {
                                        this.toastService.showErrorToast(
                                            this.translocoService.translate('global.txtErrorTitle'),
                                            this.translocoService.translate('errors.' + formatError(el))
                                        );
                                        showDefaultError = false;
                                        if (
                                            'citizenProfileNotFound' === formatError(el) ||
                                            'citizenProfileNotFoundById' === formatError(el)
                                        ) {
                                            this.errorMessage.push({
                                                severity: 'error',
                                                summary: this.translocoService.translate('global.txtErrorTitle'),
                                                detail: `<p>${this.translocoService.translate(
                                                    'global.txtNoAttachedEid'
                                                )} <a href="${this.originUrl.replace(
                                                    'login',
                                                    'eid-attachment-details'
                                                )}" target="_blank">${this.translocoService.translate(
                                                    'global.txtHere'
                                                )}</a></p>`,
                                            });
                                        }
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
                this.checkForInstallerUpdate();
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

    checkForInstallerUpdate(): void {
        this.smartCardCommunicationClientService.getCurrentVersion().subscribe({
            next: res => {
                const os = this.detectOS();
                const baseInstallerUrl = `/externalassets/files/installer/${os}`;
                const latestVersionFileUrl = `${baseInstallerUrl}/version.txt`;
                this.currentVersion = res.current;

                this.http.get(latestVersionFileUrl, { responseType: 'text' }).subscribe({
                    next: versionFileContent => {
                        const fileName = versionFileContent.trim();
                        const match = fileName.match(/-(\d+\.\d+\.\d+)/);
                        this.installerVersion = match ? match[1] : '';

                        if (this.installerVersion && this.installerVersion !== this.currentVersion) {
                            this.downloadLink = `${baseInstallerUrl}/${fileName}`;
                        }
                    },
                    error: () => {
                        this.toastService.showErrorToast(
                            this.translocoService.translate('global.txtErrorTitle'), // Cant get latest version
                            this.translocoService.translate('global.txtUnexpectedError')
                        );
                    },
                });
            },
            error: () => {
                this.toastService.showErrorToast(
                    this.translocoService.translate('global.txtErrorTitle'), // Cant get current version
                    this.translocoService.translate('global.txtUnexpectedError')
                );
            },
        });
    }

    detectOS(): string {
        const userAgent = navigator.userAgent.toLowerCase();

        if (userAgent.includes('windows')) {
            return 'windows';
        } else if (userAgent.includes('mac os') || userAgent.includes('macintosh')) {
            return 'mac';
        } else if (userAgent.includes('fedora')) {
            return 'fedora';
        } else if (
            userAgent.includes('debian') || // Debian-based systems
            userAgent.includes('linux') || // General Linux systems
            userAgent.includes('android') || // Android is Linux-based
            userAgent.includes('ubuntu') || // Ubuntu is a Linux distro
            userAgent.includes('x11') // X11 often indicates a Linux desktop
        ) {
            return 'debian'; // Fallback for other Linux-based systems
        }

        return 'windows'; // OS could not be determined
    }

    downloadSmartCardCommunicationService(): void {
        if (!this.downloadLink) {
            return;
        }
        const anchor = document.createElement('a');
        anchor.href = this.downloadLink;
        anchor.download = '';
        anchor.target = '_blank';
        anchor.click();
    }
}
