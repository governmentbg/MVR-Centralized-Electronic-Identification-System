import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ProviderService } from '@app/features/providers/provider.service';
import { ToastService } from '@app/shared/services/toast.service';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-provider-deny-dialog',
    templateUrl: './provider-deny-dialog.component.html',
    styleUrls: ['./provider-deny-dialog.component.scss'],
})
export class ProviderDenyDialogComponent {
    constructor(
        private providerService: ProviderService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() providerId: string | null = null;
    @Input() actionType!: 'Deny' | 'Terminate';
    @Output() successCallback = new EventEmitter();

    form = new FormGroup({
        reason: new FormControl('', [Validators.required]),
    });

    showDialog = false;

    denyMutation = new RequestHandler({
        requestFunction: this.providerService.denyProvider,
        onInit: () => {
            this.form.disable();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.providers.application-preview.deny-dialog.toasts.success')
            );
            this.successCallback.emit();
            this.hide();
        },
        onError: errorResponse => {
            let errorMessage = '';

            const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
            switch (firstKey) {
                case 'Id':
                    errorMessage = this.translateService.translate(
                        'modules.providers.application-preview.deny-dialog.toasts.Id'
                    );
                    break;
                default:
                    errorMessage = this.translateService.translate(
                        'modules.providers.application-preview.deny-dialog.toasts.error'
                    );
                    break;
            }

            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
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

    denyProvider() {
        if (!this.providerId) {
            return;
        }
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        const reason = this.form.controls.reason.value as string;

        this.denyMutation.execute(this.providerId, reason);
    }

    get translationScope() {
        return this.actionType === 'Deny'
            ? 'modules.providers.application-preview.deny-dialog'
            : 'modules.providers.application-preview.terminate-dialog';
    }
}
