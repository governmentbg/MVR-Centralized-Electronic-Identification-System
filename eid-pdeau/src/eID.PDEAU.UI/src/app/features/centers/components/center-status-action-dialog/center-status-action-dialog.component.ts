import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { CentersService } from '@app/features/centers/centers.service';
import { ToastService } from '@app/shared/services/toast.service';
import { TranslocoService } from '@ngneat/transloco';
import { FileUpload } from 'primeng/fileupload';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { AttachmentForm } from '@app/features/administrators/administrators.dto';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { ApplicationType, ManagerStatus } from '@app/features/centers/enums/centers.enum';

@Component({
    selector: 'app-center-status-action-dialog',
    templateUrl: './center-status-action-dialog.component.html',
    styleUrls: ['./center-status-action-dialog.component.scss'],
})
export class CenterStatusActionDialogComponent implements OnInit {
    constructor(
        private centersService: CentersService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() eidManager: any | null = null;
    @Output() successCallback = new EventEmitter();
    @ViewChild('documentComponent') documentComponent!: FileUpload;

    form = new FormGroup({
        note: new FormGroup({
            content: new FormControl(''),
        }),
        documents: new FormArray<FormGroup<AttachmentForm>>([]),
    });
    action: ApplicationType | null = null;
    showDialog = false;
    documentTypes: any[] = [];
    documentType = '';
    EidManagerStatus = ManagerStatus;

    actionMutation = new RequestHandler({
        requestFunction: this.centersService.createApplication,
        onInit: () => {
            this.form.disable();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate(`modules.centers.center-preview.txtRegistrationStatusChange`)
            );
            this.successCallback.emit();
            this.hide();
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate(`${this.translationKey}.toasts.error`)
            );
            this.form.enable();
        },
    });
    fetchDocumentTypesMutation = new RequestHandler({
        requestFunction: this.centersService.fetchDocumentTypes,
        onSuccess: data => {
            this.documentTypes = data;
        },
    });

    ngOnInit() {
        this.fetchDocumentTypesMutation.execute();
    }

    show(action: ApplicationType) {
        this.action = action;
        this.showDialog = true;
    }

    hide() {
        this.showDialog = false;
        this.form.reset();
        this.form.controls.documents.clear();
    }

    submit() {
        if (!this.eidManager || !this.action) {
            return;
        }
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        const payload = {
            applicant: this.eidManager.applicant,
            email: this.eidManager.email,
            phone: this.eidManager.phone,
            description: this.eidManager.description,
            address: this.eidManager.address,
            companyName: this.eidManager.name,
            companyNameLatin: this.eidManager.nameLatin,
            downloadUrl: this.eidManager.downloadUrl,
            logoUrl: this.eidManager.logoUrl,
            eikNumber: this.eidManager.eikNumber,
            applicationType: this.action,
        };
        this.actionMutation.execute({ ...payload, ...this.form.getRawValue() });
    }

    get translationKey() {
        switch (this.action) {
            case ApplicationType.REVOKE:
                return 'modules.centers.center-preview.stop-dialog';
            case ApplicationType.RESUME:
                return 'modules.centers.center-preview.resume-dialog';
            case ApplicationType.STOP:
                return 'modules.centers.center-preview.suspend-dialog';
            default:
                return '';
        }
    }

    get titleTranslation() {
        switch (this.action) {
            case ApplicationType.REVOKE:
                return this.translateService.translate('modules.centers.center-preview.stop-dialog.txtTitle');
            case ApplicationType.RESUME:
                return this.translateService.translate('modules.centers.center-preview.resume-dialog.txtTitle');
            case ApplicationType.STOP:
                return this.translateService.translate('modules.centers.center-preview.suspend-dialog.txtTitle');
            default:
                return '';
        }
    }

    get descriptionTranslation() {
        switch (this.action) {
            case ApplicationType.REVOKE:
                return this.translateService.translate('modules.centers.center-preview.stop-dialog.txtDescription');
            case ApplicationType.RESUME:
                return this.translateService.translate('modules.centers.center-preview.resume-dialog.txtDescription');
            case ApplicationType.STOP:
                return this.translateService.translate('modules.centers.center-preview.suspend-dialog.txtDescription');
            default:
                return '';
        }
    }

    get dialogStyleClass() {
        switch (this.action) {
            case ApplicationType.REVOKE:
                return 'error-dialog';
            case ApplicationType.RESUME:
                return 'success-dialog';
            case ApplicationType.STOP:
                return 'warning-dialog';
            default:
                return '';
        }
    }

    get buttonStyleClass() {
        switch (this.action) {
            case ApplicationType.REVOKE:
                return 'p-button-danger';
            case ApplicationType.RESUME:
                return 'p-button-success';
            case ApplicationType.STOP:
                return 'p-button-warning';
            default:
                return '';
        }
    }

    addFile(event: any) {
        this.documentComponent.choose();
    }

    onFileChange(event: any) {
        event.currentFiles.forEach((file: any) => {
            const reader = new FileReader();
            reader.onload = (e: any) => {
                const fileContent = e.target.result.split(',')[1]; // Base64 content
                const formGroup = new FormGroup<AttachmentForm>({
                    fileName: new FormControl(file.name, {
                        nonNullable: true,
                    }),
                    content: new FormControl(fileContent, {
                        nonNullable: true,
                    }),
                    documentType: new FormControl(this.documentType, {
                        nonNullable: true,
                    }),
                });
                this.form.controls.documents.push(formGroup);
            };
            reader.readAsDataURL(file);
        });
    }

    protected readonly ApplicationType = ApplicationType;
}
