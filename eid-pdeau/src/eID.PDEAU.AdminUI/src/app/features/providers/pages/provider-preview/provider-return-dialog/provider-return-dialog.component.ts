import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ProviderService } from '@app/features/providers/provider.service';
import { ToastService } from '@app/shared/services/toast.service';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-provider-return-dialog',
    templateUrl: './provider-return-dialog.component.html',
    styleUrls: ['./provider-return-dialog.component.scss'],
})
export class ProviderReturnDialogComponent {
    constructor(
        private providerService: ProviderService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() providerId: string | null = null;

    @Output() successCallback = new EventEmitter();

    form = new FormGroup({
        reason: new FormControl('', [Validators.required]),
    });

    showDialog = false;

    returnMutation = new RequestHandler({
        requestFunction: this.providerService.returnProvider,
        onInit: () => {
            this.form.disable();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.providers.application-preview.return-dialog.toasts.success')
            );
            this.successCallback.emit();
            this.hide();
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.providers.application-preview.return-dialog.toasts.error')
            );
        },
        onComplete: () => {
            this.form.enable();
        },
    });

    show() {
        this.showDialog = true;
    }

    hide() {
        this.showDialog = false;
    }

    returnProvider() {
        if (!this.providerId) {
            return;
        }
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        const reason = this.form.controls.reason.value as string;

        this.returnMutation.execute(this.providerId, reason);
    }
}
