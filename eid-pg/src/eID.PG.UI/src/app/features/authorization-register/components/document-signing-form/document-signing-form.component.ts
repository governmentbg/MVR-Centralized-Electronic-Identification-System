import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SignClientService } from '../../services/sign-client.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { Subject, Subscription, switchMap, takeUntil, timer } from 'rxjs';
import {
    BoricaSignRequestData,
    KEPDigestRequestData,
    KEPSignRequestData,
    SignTransactions,
} from '../../interfaces/document-sign.interfaces';
import { BoricaSignatureStatus, SignatureProvider, SignatureStatus } from '../../enums/document-sign.enum';
import { AuthorizationRegisterService } from '../../services/authorization-register.service';
import { Base64 } from 'js-base64';

declare global {
    interface Window {
        nexu_get_certificates: any;
        nexu_sign_with_token_infos: any;
    }
}

@Component({
    selector: 'app-document-signing-form',
    templateUrl: './document-signing-form.component.html',
    styleUrls: ['./document-signing-form.component.scss'],
})
export class DocumentSigningFormComponent implements OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private signService: SignClientService,
        private toastService: ToastService,
        private authorizationRegisterService: AuthorizationRegisterService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
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
    }

    @Input() documentToSign = '';
    @Input() empowermentId = '';
    @Input() uid = '';
    @Output() successCallback = new EventEmitter();

    languageChangeSubscription: Subscription = new Subscription();
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
    form = new FormGroup({
        signatureMethod: new FormControl<string | null>(null, Validators.required),
    });
    submitted = false;
    nexuDetected = false;
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
    documentFileName = 'EmpowermentForm';
    retryCountIfServerError = 2;
    retryCountsLeft = this.retryCountIfServerError;

    ngOnDestroy() {
        this.closeTimer$.next();
        this.languageChangeSubscription.unsubscribe();
    }

    onSignatureOptionsChange(e: any): void {
        this.submitted = false;
        this.selectedSignatureProvider = e.value;
        this.retryCountsLeft = this.retryCountIfServerError;
    }

    get showNexuNotDetectedMessage(): boolean | null {
        return (
            this.submitted &&
            !this.loading &&
            !this.nexuDetected &&
            this.selectedSignatureProvider &&
            this.selectedSignatureProvider === SignatureProvider.KEP
        );
    }

    onSubmit(): void {
        this.submitted = true;
        this.form.markAllAsTouched();
        this.form.controls.signatureMethod.markAsDirty();
        if (this.form.valid) {
            switch (this.form.get('signatureMethod')?.value) {
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
                    if (certificate.response !== null && this.documentToSign) {
                        certificate = certificate.response;
                        const digest = await this.getDigest(this.documentToSign);
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
        this.signService.checkUserBorica(this.uid).subscribe({
            next: response => {
                if (response.responseCode === 'OK') {
                    const signBody: BoricaSignRequestData = {
                        contents: [
                            {
                                confirmText: `Confirm sign`,
                                contentFormat: 'BINARY_BASE64',
                                mediaType: 'text/xml',
                                data: Base64.encode(this.documentToSign),
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
                        uid: this.uid,
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
        this.signService.checkUserEvrotrust(this.uid).subscribe({
            next: response => {
                if (response['isReadyToSign']) {
                    const dateExpire = new Date();
                    dateExpire.setDate(dateExpire.getDate() + 1);

                    const signBody = {
                        dateExpire: dateExpire,
                        documents: [
                            {
                                content: Base64.encode(this.documentToSign),
                                fileName: `${this.documentFileName}.xml`,
                                contentType: 'text/xml',
                            },
                        ],
                        uid: this.uid,
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
        switch (type) {
            case SignatureProvider.Evrotrust:
                timer(0, this.INTERVAL)
                    .pipe(
                        switchMap(() =>
                            this.signService.checkSignedDataEvrotrust({
                                transactionId:
                                    this.signTransactions[SignatureProvider.Evrotrust].transactions[0].transactionID,
                                groupSigning: this.signTransactions[SignatureProvider.Evrotrust].groupSigning,
                            })
                        ),
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
                        switchMap(() =>
                            this.signService.checkSignedDataBorica({
                                transactionId: this.signTransactions[SignatureProvider.Borica].data.callbackId,
                            })
                        ),
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
        if (this.selectedSignatureProvider && this.empowermentId) {
            this.authorizationRegisterService
                .signEmpowerment({
                    empowermentId: this.empowermentId,
                    detachedSignature: data.content,
                    signatureProvider: this.selectedSignatureProvider,
                })
                .subscribe({
                    next: () => {
                        this.hideLoader();
                        this.showSuccessToast(
                            this.translateService.translate(
                                'modules.authorizationStatement.electronicSignature.txtSignatureSuccessMessage'
                            )
                        );
                        this.successCallback.emit();
                    },
                    error: error => {
                        this.hideLoader();
                        switch (error.status) {
                            case 400: {
                                // Default error message
                                let errorMessage = this.translateService.translate('global.txtInvalidDataError');
                                const firstKey = error.errors ? Object.keys(error.errors)[0] : undefined;
                                if (firstKey === 'Invalid signature format') {
                                    errorMessage = this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtInvalidSignatureFormat'
                                    );
                                } else if (firstKey === 'Archived') {
                                    errorMessage = this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtCertificateArchived'
                                    );
                                } else if (firstKey === 'certificate.Subject') {
                                    errorMessage = this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtCertificateSubject'
                                    );
                                }
                                this.showErrorToast(errorMessage);
                                break;
                            }
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
                            case 500: {
                                // Default error message
                                let errorMessage = this.translateService.translate('global.txtSignatureErrorMessage');
                                const firstKey = error.errors ? Object.keys(error.errors)[0] : undefined;
                                if (firstKey === 'Failed validating signature') {
                                    errorMessage = this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtFailedValidatingSignature'
                                    );
                                } else if (firstKey === 'Signer certificate is not found') {
                                    errorMessage = this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtSignerCertificateIsNotFound'
                                    );
                                } else if (firstKey === 'Failed certificate status check') {
                                    errorMessage = this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtFailedCertificateStatusCheck'
                                    );
                                }
                                this.showErrorToast(errorMessage);
                                break;
                            }
                            case 502:
                                this.showErrorToast(
                                    this.translateService.translate(
                                        'modules.authorizationStatement.electronicSignature.txtBadOcspCertificateStatus'
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
        this.form.controls.signatureMethod.disable();
        this.loading = true;
    }

    hideLoader(): void {
        this.form.controls.signatureMethod.enable();
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
