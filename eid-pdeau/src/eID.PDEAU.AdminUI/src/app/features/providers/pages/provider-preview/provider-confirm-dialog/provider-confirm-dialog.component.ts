import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ProviderService } from '@app/features/providers/provider.service';
import { ToastService } from '@app/shared/services/toast.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-provider-confirm-dialog',
    templateUrl: './provider-confirm-dialog.component.html',
    styleUrls: ['./provider-confirm-dialog.component.scss'],
})
export class ProviderConfirmDialogComponent {
    constructor(
        private providerService: ProviderService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() providerId: string | null = null;
    @Output() successCallback = new EventEmitter();

    showDialog = false;
    textAreaValue = '';

    approveMutation = new RequestHandler({
        requestFunction: this.providerService.approveProvider,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.providers.application-preview.approve-dialog.toasts.success')
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
    });

    show() {
        this.showDialog = true;
    }

    hide() {
        this.showDialog = false;
        this.textAreaValue = '';
    }

    approveProvider() {
        if (!this.providerId) {
            return;
        }
        this.approveMutation.execute(this.providerId, this.textAreaValue);
    }
}
