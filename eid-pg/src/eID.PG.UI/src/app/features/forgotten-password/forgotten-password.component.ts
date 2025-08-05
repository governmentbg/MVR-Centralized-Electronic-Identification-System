import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ProfileService } from '../profile/services/profile.service';
import { MessageService } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { firstLetterCapitalValidator, requiredNamesValidator } from 'src/app/shared/validators/names';
import { emailValidator } from 'src/app/shared/validators/email';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { Router } from '@angular/router';
@Component({
    selector: 'app-forgotten-password',
    templateUrl: './forgotten-password.component.html',
    styleUrls: ['./forgotten-password.component.scss'],
})
export class ForgottenPasswordComponent implements OnInit {
    form!: FormGroup;
    requestInProgress!: boolean;
    patternFirstOrSecondName = /^(?=.{1,40}$)[а-яА-Я\s-']*$/;
    patternLastName = /^(?=.{1,60}$)[а-яА-Я\s-']*$/;
    submitted!: boolean;
    constructor(
        private fb: FormBuilder,
        private profileService: ProfileService,
        private messageService: MessageService,
        private translocoService: TranslocoService,
        private router: Router
    ) {}
    ngOnInit(): void {
        this.form = this.fb.group(
            {
                firstName: [
                    null,
                    [
                        Validators.required,
                        Validators.pattern(this.patternFirstOrSecondName),
                        firstLetterCapitalValidator(),
                    ],
                ],
                secondName: [null, [Validators.pattern(this.patternFirstOrSecondName), firstLetterCapitalValidator()]],
                lastName: [
                    null,
                    [Validators.required, Validators.pattern(this.patternLastName), firstLetterCapitalValidator()],
                ],
                email: [null, [Validators.required, emailValidator()]],
            },
            {
                validators: requiredNamesValidator.bind(this),
            }
        );
    }

    submit() {
        this.requestInProgress = true;
        if (this.form.valid) {
            this.profileService.forgottenPassword(this.form.value).subscribe({
                next: () => {
                    this.messageService.add({
                        severity: 'success',
                        summary: this.translocoService.translate('global.txtSuccessTitle'),
                        detail: this.translocoService.translate('forgottenPassword.txtSuccess'),
                    });
                    this.router.navigate(['/home']);
                },
                error: error => {
                    let showDefaultError = true;
                    error.errors.forEach((el: string) => {
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
