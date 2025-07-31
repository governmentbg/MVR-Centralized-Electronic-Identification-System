import { Injectable } from '@angular/core';
import { Confirmation, ConfirmationService } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
@Injectable({
    providedIn: 'root',
})
export class ConfirmationDialogService {
    constructor(private confirmationService: ConfirmationService, private translateService: TranslocoService) {}
    showConfirmation(confirmation: Confirmation) {
        const defaultConfirmation: Confirmation = {
            key: 'global-confirm',
            rejectButtonStyleClass: 'p-button-danger',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: this.translateService.translate('global.txtContinue'),
            rejectLabel: this.translateService.translate('global.txtCancel'),
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate('global.txtUnsavedChangesConfirmation'),
        };
        const mergedConfirmation = { ...defaultConfirmation, ...confirmation };
        this.confirmationService.confirm(mergedConfirmation);
    }
}
