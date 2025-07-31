import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastService } from '@app/shared/services/toast.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { ProviderServicesService } from '@app/features/services/provider-services.service';
import { ToggleAction } from '@app/features/services/services.dto';

@Component({
    selector: 'app-services-toggle-dialog',
    templateUrl: './services-toggle-dialog.component.html',
    styleUrls: ['./services-toggle-dialog.component.scss'],
})
export class ServicesToggleDialogComponent {
    constructor(
        private providerServicesService: ProviderServicesService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() serviceId: string | null = null;
    @Output() successCallback = new EventEmitter();
    @Input() toggleAction: ToggleAction | null = null;

    showDialog = false;
    textAreaValue = '';

    deactivateMutation = new RequestHandler({
        requestFunction: this.providerServicesService.deactivateService,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate(
                    `modules.services.service-preview.toggle-dialog.${ToggleAction.Deactivate}.toasts.success`
                )
            );
            this.successCallback.emit();
            this.hide();
        },
        onError: errorResponse => {
            let errorMessage = '';

            const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
            switch (firstKey) {
                default:
                    errorMessage = this.translateService.translate(
                        `modules.services.service-preview.toggle-dialog.${ToggleAction.Deactivate}.toasts.error`
                    );
                    break;
            }

            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
        },
    });

    activateMutation = new RequestHandler({
        requestFunction: this.providerServicesService.activateService,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate(
                    `modules.services.service-preview.toggle-dialog.${ToggleAction.Activate}.toasts.success`
                )
            );
            this.successCallback.emit();
            this.hide();
        },
        onError: errorResponse => {
            let errorMessage = '';

            const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
            switch (firstKey) {
                default:
                    errorMessage = this.translateService.translate(
                        `modules.services.service-preview.toggle-dialog.${ToggleAction.Activate}.toasts.error`
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

    toggleService() {
        if (!this.serviceId) {
            return;
        }
        if (this.toggleAction === ToggleAction.Activate) {
            this.activateMutation.execute(this.serviceId);
        } else {
            this.deactivateMutation.execute(this.serviceId);
        }
    }

    get titleTranslation() {
        return this.translateService.translate(
            `modules.services.service-preview.toggle-dialog.${this.toggleAction}.txtTitle`
        );
    }

    get descriptionTranslation() {
        return this.translateService.translate(
            `modules.services.service-preview.toggle-dialog.${this.toggleAction}.txtDescription`
        );
    }
}
