import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { MessageService } from 'primeng/api';
import { ProfileService } from '../../services/profile.service';
import { Subscription, first } from 'rxjs';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { emailValidator } from 'src/app/shared/validators/email';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';

@Component({
    selector: 'app-update-email',
    templateUrl: './update-email.component.html',
    styleUrls: ['./update-email.component.scss'],
})
export class UpdateEmailComponent implements OnInit {
    form!: FormGroup;
    requestInProgress!: boolean;
    submitted!: boolean;
    breadcrumbItems: IBreadCrumbItems[] = [];
    languageChangeSubscription: Subscription;
    constructor(
        private fb: FormBuilder,
        private profileService: ProfileService,
        private messageService: MessageService,
        private translocoService: TranslocoService
    ) {
        this.languageChangeSubscription = translocoService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translocoService.translate('profilePage.txtProfile'),
                    routerLink: '/profile',
                },
                { label: this.translocoService.translate('updateEmail.txtUpdateEmail') },
            ];
        });
    }
    ngOnInit(): void {
        this.form = this.fb.group({
            email: [null, [Validators.required, emailValidator()]],
        });
    }

    submit() {
        this.requestInProgress = true;
        if (this.form.valid) {
            this.profileService
                .updateEmail(this.form.value.email)
                .pipe(first())
                .subscribe({
                    next: () => {
                        this.messageService.add({
                            severity: 'success',
                            summary: this.translocoService.translate('global.txtSuccessTitle'),
                            detail: this.translocoService.translate('profilePage.messages.txtEmailChangeSuccess'),
                        });
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
