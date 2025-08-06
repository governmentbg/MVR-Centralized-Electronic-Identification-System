import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ProfileService } from 'src/app/features/profile/services/profile.service';
import { TranslocoService } from '@ngneat/transloco';
import { ActivatedRoute, Router } from '@angular/router';
import { passwordValidator, matchPasswordLandingPage } from 'src/app/shared/validators/password';
import { ToastService } from 'src/app/shared/services/toast.service';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
@Component({
    selector: 'app-forgotten-password-landing-page',
    templateUrl: './forgotten-password-landing-page.component.html',
    styleUrls: ['./forgotten-password-landing-page.component.scss'],
})
export class ForgottenPasswordLandingPageComponent implements OnInit {
    form!: FormGroup;
    token!: string;
    successMessage!: string;
    errorMessage!: string;
    requestInProgress!: boolean;
    blockSpace = /[^\s]/;
    constructor(
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private profileService: ProfileService,
        private translocoService: TranslocoService,
        private toastService: ToastService
    ) {}

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.token = params['token'];
        });
        if (this.token) {
            this.form = this.fb.group(
                {
                    password: [null, [Validators.required, passwordValidator(), Validators.maxLength(255)]],
                    confirmPassword: [null, [Validators.required, Validators.maxLength(255)]],
                    token: this.token,
                },
                {
                    validators: matchPasswordLandingPage.bind(this),
                }
            );
        } else {
            this.router.navigate(['/home']);
            this.toastService.showErrorToast(
                this.translocoService.translate('global.txtErrorTitle'),
                this.translocoService.translate('global.txtErrorMissingToken')
            );
        }
    }

    submit() {
        this.requestInProgress = true;
        if (this.form.valid) {
            this.profileService.resetPassword(this.form.value).subscribe({
                next: () => {
                    this.toastService.showSuccessToast(
                        this.translocoService.translate('global.txtSuccessTitle'),
                        this.translocoService.translate('forgottenPassword.messages.txtSuccess')
                    );
                    this.requestInProgress = false;
                    this.router.navigate(['/login']);
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
