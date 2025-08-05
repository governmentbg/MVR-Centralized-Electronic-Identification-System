import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { MessageService } from 'primeng/api';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import {
    firstLetterCapitalValidator,
    requiredNamesValidator,
    requiredLatinNamesValidator,
} from 'src/app/shared/validators/names';
import { ProfileService } from '../../services/profile.service';
import { first, Subscription } from 'rxjs';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { User } from '../../interfaces/user';
import { UserService } from 'src/app/core/services/user.service';
import { IUser } from 'src/app/core/interfaces/IUser';

@Component({
    selector: 'app-update-personal-info',
    templateUrl: './update-personal-info.component.html',
    styleUrls: ['./update-personal-info.component.scss'],
})
export class UpdatePersonalInfoComponent implements OnInit {
    form!: FormGroup;
    requestInProgress!: boolean;
    blockSpace = /[^\s]/;
    patternFirstOrSecondName = /^(?=.{1,40}$)[а-яА-Я\s-']*$/;
    patternLastName = /^(?=.{1,60}$)[а-яА-Я\s-']*$/;
    latinPatternFirstOrSecondName = /^(?=.{1,40}$)[a-zA-Z\s-']*$/;
    latinPatternLastName = /^(?=.{1,60}$)[a-zA-Z\s-']*$/;
    breadcrumbItems: IBreadCrumbItems[] = [];
    languageChangeSubscription: Subscription;
    user: IUser | any = null;
    constructor(
        private fb: FormBuilder,
        private profileService: ProfileService,
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private userService: UserService
    ) {
        this.languageChangeSubscription = translocoService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translocoService.translate('profilePage.txtProfile'),
                    routerLink: '/profile',
                },
                { label: this.translocoService.translate('updatePersonalInfo.txtUpdatePersonalInfo') },
            ];
        });
    }

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
                phoneNumber: [null],
                is2FaEnabled: [false],
            },
            {
                validators: [requiredNamesValidator.bind(this), requiredLatinNamesValidator.bind(this)],
            }
        );
        this.getPersonalInfo();
    }

    submit(): void {
        if (this.form.valid) {
            this.requestInProgress = true;
            const formValue = this.convertEmptyStringsToNull(this.form.value);
            this.profileService.updatePersonalInfo(formValue).subscribe({
                next: () => {
                    this.messageService.add({
                        severity: 'success',
                        summary: this.translocoService.translate('global.txtSuccessTitle'),
                        detail: this.translocoService.translate('updatePersonalInfo.txtDataChangeSuccess'),
                    });
                    const user = JSON.parse(sessionStorage.getItem('user') || '');
                    user.name = `${this.form.value.firstName} ${this.form.value.secondName} ${this.form.value.lastName}`;
                    this.userService.setUser(user);
                    sessionStorage.setItem('user', JSON.stringify(user));
                    this.userService.userSubject.next(true);
                    this.requestInProgress = false;
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

    getPersonalInfo() {
        this.requestInProgress = true;
        this.profileService
            .getUser()
            .pipe(first())
            .subscribe({
                next: (user: User) => {
                    this.form.patchValue(user);
                    this.requestInProgress = false;
                },
                error: err => {
                    let showDefaultError = true;
                    err.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.messageService.add({
                                severity: 'error',
                                summary: this.translocoService.translate('global.txtErrorTitle'),
                                detail: this.translocoService.translate('errors.' + formatError(el)),
                            });
                            // if backend has an error/s, turn off the default message
                            showDefaultError = false;
                        }
                    });
                    // if not error was recognized by the storage, throw default message once
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
