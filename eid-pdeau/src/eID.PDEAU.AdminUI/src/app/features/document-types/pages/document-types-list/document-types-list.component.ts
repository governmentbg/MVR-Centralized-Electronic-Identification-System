import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { ToastService } from '@app/shared/services/toast.service';
import { Router } from '@angular/router';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { DocumentTypesService } from '@app/features/document-types/document-types.service';

@Component({
    selector: 'app-document-types-list',
    templateUrl: './document-types-list.component.html',
    styleUrls: ['./document-types-list.component.scss'],
    providers: [ConfirmationService],
})
export class DocumentTypesListComponent implements OnInit {
    constructor(
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        private router: Router,
        private documentTypesService: DocumentTypesService
    ) {}

    documentTypes: any[] = [];
    isConfirm = true;
    documentTypesQuery = new RequestHandler({
        requestFunction: this.documentTypesService.fetchDocumentTypes,
        onSuccess: response => {
            this.documentTypes = response;
        },
        onError: error => {
            switch (error.status) {
                case 400:
                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                    break;
                default:
                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                    break;
            }
        },
    });
    deleteDocumentTypeQuery = new RequestHandler({
        requestFunction: this.documentTypesService.deleteDocumentType,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.documentTypes.delete.toasts.successfulDelete')
            );
            this.documentTypesQuery.execute();
        },
        onError: error => {
            switch (error.status) {
                case 400:
                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                    break;
                default:
                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                    break;
            }
        },
    });

    ngOnInit() {
        this.documentTypesQuery.execute();
    }

    deleteDiscountConfirmation(data: any) {
        this.isConfirm = false;
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            header: this.translateService.translate('modules.documentTypes.delete.title'),
            message: this.translateService.translate('modules.documentTypes.delete.description'),
            accept: () => {
                this.deleteDocumentType(data);
            },
        });
    }

    deleteDocumentType(data: any) {
        this.deleteDocumentTypeQuery.execute(data);
    }

    navigateToEdit(data: any) {
        this.router.navigate(['/document-types/edit'], {
            state: { documentType: data },
        });
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }
}
