import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { AuthenticationService } from 'src/app/authentication/authentication-service/authentication-service.service';
import { EGNAdultValidator, EGNValidator, PinOrAdultEGNValidator, PinValidator } from 'src/app/shared/validators/egn';
import { catchError, finalize, of, Subject, Subscription, switchMap, takeUntil, timer } from 'rxjs';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { ToastService } from 'src/app/shared/services/toast.service';
import { IUser } from 'src/app/core/interfaces/IUser';
import { IdentifierType } from 'src/app/shared/identifier-type';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-mobile-eid-login',
    templateUrl: './mobile-eid-login.component.html',
    styleUrls: ['./mobile-eid-login.component.scss'],
})
export class MobileEidLoginComponent implements OnInit, OnDestroy {
    form!: FormGroup;
    requestForm = {
        auth_req_id: '',
        expires_in: '',
        interval: '',
    };
    requestInProgress!: boolean;
    languageChangeSubscription: Subscription;
    user!: IUser | null;
    cancelPolling$ = new Subject<void>();
    originUrl = '';
    realm = '';
    client_id = '';
    scope = '';
    system_id = '';
    constructor(
        private fb: FormBuilder,
        private authenticationService: AuthenticationService,
        private translocoService: TranslocoService,
        private toastService: ToastService,
        private route: ActivatedRoute
    ) {
        this.languageChangeSubscription = translocoService.langChanges$.subscribe(() => {
            this.identifierTypes = [
                {
                    name: this.translocoService.translate('modules.authorizationStatement.statementForm.EGN'),
                    id: IdentifierType.EGN,
                },
                {
                    name: this.translocoService.translate('modules.authorizationStatement.statementForm.LNCh'),
                    id: IdentifierType.LNCh,
                },
            ];
        });
    }

    identifierTypes = [
        {
            name: this.translocoService.translate('modules.authorizationStatement.statementForm.EGN'),
            id: IdentifierType.EGN,
        },
        {
            name: this.translocoService.translate('modules.authorizationStatement.statementForm.LNCh'),
            id: IdentifierType.LNCh,
        },
    ];

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.originUrl = params['origin'];
            this.realm = params['realm'];
            this.client_id = params['client_id'];
            this.scope = params['scope'];
            this.system_id = params['system_id'];
        });
        this.form = this.fb.group({
            type: [null, Validators.required],
            phoneNumber: [null],
            citizenNumber: [null, [Validators.required, Validators.pattern('[0-9]+'), PinOrAdultEGNValidator()]],
            termsAgreement: [false, Validators.requiredTrue],
        });
    }

    submit() {
        if (this.form.valid) {
            this.requestInProgress = true;
            this.authenticationService.submitForm(this.form.value, this.client_id, this.scope, this.system_id).subscribe({
                next: res => {
                    this.requestForm = {
                        auth_req_id: res.auth_req_id,
                        expires_in: res.expires_in,
                        interval: res.interval,
                    };
                    const intervalTime = res.interval * 1000;
                    const expiresIn = res.expires_in * 1000;
                    timer(0, intervalTime)
                        .pipe(
                            finalize(() => (this.requestInProgress = false)),
                            takeUntil(timer(expiresIn)),
                            takeUntil(this.cancelPolling$),
                            switchMap(() => {
                                return this.authenticationService
                                    .getToken(this.requestForm.auth_req_id, this.client_id)
                                    .pipe(
                                        catchError(error => {
                                            if (error && error.detail) {
                                                try {
                                                    const detail = JSON.parse(error.detail);
                                                    if (detail.error === 'access_denied') {
                                                        this.toastService.showErrorToast(
                                                            this.translocoService.translate('global.txtErrorTitle'),
                                                            this.translocoService.translate(
                                                                'global.txtAuthorizationRejected'
                                                            )
                                                        );
                                                        this.cancelPolling$.next();
                                                        return of(null);
                                                    }
                                                } catch (e) {
                                                    console.error(e);
                                                }
                                            }
                                            return of(null);
                                        })
                                    );
                            })
                        )
                        .subscribe({
                            next: res => {
                                if (res) {
                                    window.opener.postMessage(
                                        {
                                            status: 'success',
                                            access_token: res.access_token,
                                            refresh_token: res.refresh_token,
                                        },
                                        this.originUrl
                                    );
                                    window.close();
                                }
                            },
                        });
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

    cancel() {
        this.requestInProgress = false;
        this.cancelPolling$.next();
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

    onIdentifierChange(event: any, formGroup: FormGroup) {
        if (event.value === IdentifierType.EGN) {
            formGroup.controls['citizenNumber'].setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                EGNAdultValidator(),
                EGNValidator(),
            ]);
        } else if (event.value === IdentifierType.LNCh) {
            formGroup.controls['citizenNumber'].setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                PinValidator(),
            ]);
        }
        formGroup.controls['citizenNumber'].updateValueAndValidity();
    }

    ngOnDestroy(): void {
        this.cancelPolling$.next();
        this.cancelPolling$.complete();
    }
}
