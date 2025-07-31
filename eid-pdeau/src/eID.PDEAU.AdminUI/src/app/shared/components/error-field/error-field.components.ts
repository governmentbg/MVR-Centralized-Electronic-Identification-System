import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
@Component({
    selector: 'app-error-field',
    template: `
        <div
            *ngIf="formControlInstance?.invalid && (formControlInstance.dirty || formControlInstance.touched)"
            class="text-danger validation-text">
            {{ errorMessage }}
        </div>
    `,
})
export class ErrorFieldComponent implements OnInit, OnDestroy {
    @Input() formControlInstance!: AbstractControl;
    @Input() translationPrefix = '';
    @Input() order?: string[] = [];

    errorMessage = '';
    subscriptions = new Subscription();

    constructor(private translationService: TranslocoService) {}

    ngOnInit(): void {
        const tranlationSubscription = this.translationService.langChanges$.subscribe(() => {
            this.errorMessage = this.translatedErrorMessage;
        });
        this.subscriptions.add(tranlationSubscription);

        const formControlSubscription = this.formControlInstance.valueChanges.subscribe(() => {
            if (!this.formControlInstance.invalid) {
                this.errorMessage = '';
                return;
            }
            this.errorMessage = this.translatedErrorMessage;
        });
        this.subscriptions.add(formControlSubscription);
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    get translatedErrorMessage(): string {
        const errorKeys = Object.keys(this.formControlInstance.errors || {});

        if (errorKeys.length === 0) {
            return '';
        }

        const priorityError = this.order?.find(error => errorKeys.includes(error));

        if (priorityError) {
            const priorityErrorValue = this.formControlInstance.errors?.[priorityError];
            const translationParams = typeof priorityErrorValue === 'object' ? priorityErrorValue : {};
            return this.translationService.translate(`${this.translationPrefix}.${priorityError}`, translationParams);
        }
        const errorKey = errorKeys[0];
        const errorValue = this.formControlInstance.errors?.[errorKey];
        const translationParams = typeof errorValue === 'object' ? errorValue : {};
        return this.translationService.translate(`${this.translationPrefix}.${errorKey}`, translationParams);
    }
}
