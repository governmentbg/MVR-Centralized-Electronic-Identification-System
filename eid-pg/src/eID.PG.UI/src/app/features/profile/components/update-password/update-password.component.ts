import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { ProfileService } from '../../services/profile.service';
import { Subscription, first } from 'rxjs';
import { matchPasswordUpdate, newPasswordValidator, passwordValidator } from 'src/app/shared/validators/password';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { Router } from '@angular/router';
import { ToastService } from 'src/app/shared/services/toast.service';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
@Component({
    selector: 'app-update-password',
    templateUrl: './update-password.component.html',
    styleUrls: ['./update-password.component.scss'],
})
export class UpdatePasswordComponent implements OnInit {
    form!: FormGroup;
    requestInProgress!: boolean;
    submitted!: boolean;
    breadcrumbItems: IBreadCrumbItems[] = [];
    blockSpace = /[^\s]/;
    languageChangeSubscription: Subscription;
    constructor(
        private fb: FormBuilder,
        private profileService: ProfileService,
        private translocoService: TranslocoService,
        private router: Router,
        private toastService: ToastService
    ) {
        this.languageChangeSubscription = translocoService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translocoService.translate('profilePage.txtProfile'),
                    routerLink: '/profile',
                },
                { label: this.translocoService.translate('updatePassword.txtUpdatePassword') },
            ];
        });
    }

    ngOnInit(): void {
        this.form = this.fb.group(
            {
                oldPassword: [null, [Validators.required, Validators.maxLength(255)]],
                newPassword: [
                    null,
                    [
                        Validators.required,
                        passwordValidator(),
                        newPasswordValidator('oldPassword'),
                        Validators.maxLength(255),
                    ],
                ],
                confirmPassword: [null, [Validators.required, Validators.maxLength(255)]],
            },
            {
                validators: matchPasswordUpdate.bind(this),
            }
        );
    }

    submit() {
        this.requestInProgress = true;
        if (this.form.valid) {
            this.profileService
                .updatePassword(this.form.value)
                .pipe(first())
                .subscribe({
                    next: () => {
                        this.toastService.showSuccessToast(
                            this.translocoService.translate('global.txtSuccessTitle'),
                            this.translocoService.translate('profilePage.messages.txtPasswordChangeSuccess')
                        );
                        this.requestInProgress = false;
                        this.router.navigate(['/profile']);
                    },
                    error: error => {
                        let showDefaultError = true;
                        error.errors.forEach((el: string) => {
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
}
