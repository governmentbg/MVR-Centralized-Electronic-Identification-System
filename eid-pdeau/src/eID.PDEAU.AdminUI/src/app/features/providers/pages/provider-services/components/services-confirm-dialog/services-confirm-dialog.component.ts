import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastService } from '@app/shared/services/toast.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { ProviderServicesService } from '@app/features/providers/provider-services.service';

@Component({
    selector: 'app-services-confirm-dialog',
    templateUrl: './services-confirm-dialog.component.html',
    styleUrls: ['./services-confirm-dialog.component.scss'],
})
export class ServicesConfirmDialogComponent {
    constructor(
        private providerServicesService: ProviderServicesService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() serviceId: string | null = null;
    @Output() successCallback = new EventEmitter();

    showDialog = false;
    textAreaValue = '';

    approveMutation = new RequestHandler({
        requestFunction: this.providerServicesService.approveService,
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

    approveService() {
        if (!this.serviceId) {
            return;
        }
        this.approveMutation.execute(this.serviceId);
    }
}
