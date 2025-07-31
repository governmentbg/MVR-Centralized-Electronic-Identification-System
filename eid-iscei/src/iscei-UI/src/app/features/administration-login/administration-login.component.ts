import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { first, Subscription } from 'rxjs';
import { IUser } from 'src/app/core/interfaces/IUser';
import { RaeiceiClientService } from 'src/app/core/services/raeicei.service';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { ToastService } from 'src/app/shared/services/toast.service';
export enum RequestScopeTypes {
    eid_aei = 'eid_aei',
    eid_cei = 'eid_cei',
    eid_deau = 'eid_deau',
}
@Component({
    selector: 'app-administration-login',
    templateUrl: './administration-login.component.html',
    styleUrls: ['./administration-login.component.scss'],
})
export class AdministrationLoginComponent implements OnInit, OnDestroy {
    form!: FormGroup;
    requestInProgress!: boolean;
    user: IUser | any = null;
    originUrl = '';
    realm = '';
    client_id = '';
    provider = '';
    authTypes = RequestScopeTypes;
    showLoginMenu = false;
    options: any;
    language!: string;
    subscription!: Subscription;
    requestScopeTypesList = [
        {
            value: RequestScopeTypes.eid_aei,
            name: 'Администратор на електронна идентичност',
            nameLatin: 'Employee of the Administrator of Electronic Identity',
        },
        {
            value: RequestScopeTypes.eid_cei,
            name: 'Центрове за електронна идентификация',
            nameLatin: 'Employee of the Centers for Electronic Identification',
        },
        {
            value: RequestScopeTypes.eid_deau,
            name: 'Служител на Доставчик на услуги',
            nameLatin: 'Employee of the Service Provider',
        },
    ];

    constructor(
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private raeiceiService: RaeiceiClientService,
        private toastService: ToastService,
        public translateService: TranslocoService
    ) {
        this.form = this.fb.group({
            scope: [null, Validators.required],
            systemId: [null, Validators.required],
        });
        this.subscription = translateService.langChanges$.subscribe(language => {
            this.language = language;
        });
    }
    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.originUrl = params['origin'];
            this.realm = params['realm'];
            this.client_id = params['client_id'];
            this.provider = params['provider'];
        });
    }

    onOptionChange(option: any) {
        this.form.get('systemId')?.reset();
        this.options = [];
        if (option.value === 'eid_aei') {
            this.raeiceiService
                .fetchAdministrators()
                .pipe(first())
                .subscribe({
                    next: (res: any) => {
                        this.options = res;
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
        } else if (option.value === 'eid_cei') {
            this.raeiceiService
                .fetchEidCenters()
                .pipe(first())
                .subscribe({
                    next: (res: any) => {
                        this.options = res;
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
        } else {
            this.raeiceiService
                .fetchProviders()
                .pipe(first())
                .subscribe({
                    next: (res: any) => {
                        this.options = res.data;
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
    }

    submit() {
        if (this.form.valid) {
            this.showLoginMenu = true;
        } else {
            this.markFormGroupTouched(this.form);
        }
    }

    hideDetails() {
        this.showLoginMenu = false;
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

    isLangBG() {
        return this.language === 'bg';
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }
}
