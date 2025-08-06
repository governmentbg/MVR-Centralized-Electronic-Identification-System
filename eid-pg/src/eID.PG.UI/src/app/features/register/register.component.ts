import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProfileService } from '../profile/services/profile.service';
import { TranslocoService } from '@ngneat/transloco';
import { Message } from 'primeng/api';
import { matchPassword, passwordValidator } from 'src/app/shared/validators/password';
import {
    firstLetterCapitalValidator,
    requiredNamesValidator,
    requiredLatinNamesValidator,
} from 'src/app/shared/validators/names';
import { emailValidator } from 'src/app/shared/validators/email';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
    form!: FormGroup;
    successMessage!: Message[];
    errorMessage: Message[] = [];
    requestInProgress!: boolean;
    blockSpace = /[^\s]/;
    patternFirstOrSecondName = /^(?=.{1,40}$)[а-яА-Я\s-']*$/;
    patternLastName = /^(?=.{1,60}$)[а-яА-Я\s-']*$/;
    latinPatternFirstOrSecondName = /^(?=.{1,40}$)[a-zA-Z\s-']*$/;
    latinPatternLastName = /^(?=.{1,60}$)[a-zA-Z\s-']*$/;
    showSuccess = false;
    constructor(
        private fb: FormBuilder,
        private profileService: ProfileService,
        private translocoService: TranslocoService
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
                firstNameLatin: [
                    null,
                    [
                        Validators.required,
                        Validators.pattern(this.latinPatternFirstOrSecondName),
                        firstLetterCapitalValidator(),
                    ],
                ],
                secondNameLatin: [
                    null,
                    [Validators.pattern(this.latinPatternFirstOrSecondName), firstLetterCapitalValidator()],
                ],
                lastNameLatin: [
                    null,
                    [Validators.required, Validators.pattern(this.latinPatternLastName), firstLetterCapitalValidator()],
                ],
                email: [null, [Validators.required, emailValidator()]],
                phoneNumber: [''],
                baseProfilePassword: [
                    null,
                    {
                        validators: [Validators.required, passwordValidator(), Validators.maxLength(255)],
                        updateOn: 'blur',
                    },
                ],
                matchingPassword: [
                    null,
                    {
                        validators: [Validators.required, Validators.maxLength(255)],
                        updateOn: 'change',
                    },
                ],
            },
            {
                validators: [
                    matchPassword.bind(this),
                    requiredNamesValidator.bind(this),
                    requiredLatinNamesValidator.bind(this),
                ],
            }
        );
    }

    submit(): void {
        if (this.form.valid) {
            this.requestInProgress = true;
            const formValue = this.convertEmptyStringsToNull(this.form.value);
            this.profileService.registerUser(formValue).subscribe({
                next: () => {
                    this.showSuccess = true;
                    this.errorMessage = [];
                    this.form.reset();
                    this.requestInProgress = false;
                },
                error: error => {
                    this.errorMessage = [];
                    let showDefaultError = true;
                    error.errors.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.errorMessage.push({
                                severity: 'error',
                                summary: this.translocoService.translate('global.txtErrorTitle'),
                                detail: this.translocoService.translate('errors.' + formatError(el)),
                            });
                            showDefaultError = false;
                        }
                    });
                    if (showDefaultError) {
                        this.errorMessage.push({
                            severity: 'error',
                            summary: this.translocoService.translate('global.txtErrorTitle'),
                            detail: this.translocoService.translate('global.txtUnexpectedError'),
                        });
                    }
                    this.markFormGroupTouched(this.form);
                    this.successMessage = [];
                    this.requestInProgress = false;
                },
            });
        } else {
            this.markFormGroupTouched(this.form);
            this.requestInProgress = false;
        }
    }

    clearPasswordErrors(): void {
        const passwordControl = this.form.get('baseProfilePassword');
        if (passwordControl) {
            passwordControl.setErrors(null);
        }
    }

    setPasswordValidator(): void {
        const passwordControl = this.form.get('baseProfilePassword');
        passwordControl?.setValidators([Validators.required, passwordValidator()]);
        passwordControl?.updateValueAndValidity();
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

    convertEmptyStringsToNull(obj: any): any {
        const result: any = {};
        for (const key in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, key)) {
                result[key] = obj[key] === '' ? null : obj[key];
            }
        }
        return result;
    }
}
