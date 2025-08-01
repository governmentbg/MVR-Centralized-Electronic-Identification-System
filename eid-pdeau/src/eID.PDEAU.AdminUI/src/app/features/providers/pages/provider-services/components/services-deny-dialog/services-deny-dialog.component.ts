import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ToastService } from '@app/shared/services/toast.service';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { ProviderServicesService } from '@app/features/providers/provider-services.service';

@Component({
    selector: 'app-services-deny-dialog',
    templateUrl: './services-deny-dialog.component.html',
    styleUrls: ['./services-deny-dialog.component.scss'],
})
export class ServicesDenyDialogComponent {
    constructor(
        private providerServicesService: ProviderServicesService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() serviceId: string | null = null;
    @Output() successCallback = new EventEmitter();

    form = new FormGroup({
        reason: new FormControl('', [Validators.required]),
    });

    showDialog = false;

    denyMutation = new RequestHandler({
        requestFunction: this.providerServicesService.denyService,
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
                        'modules.providers.application-preview.approve-dialog.toasts.Id'
                    );
                    break;
                default:
                    errorMessage = this.translateService.translate(
                        'modules.providers.application-preview.approve-dialog.toasts.error'
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

    denyService() {
        if (!this.serviceId) {
            return;
        }
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        const reason = this.form.controls.reason.value as string;

        this.denyMutation.execute(this.serviceId, reason);
    }
}
