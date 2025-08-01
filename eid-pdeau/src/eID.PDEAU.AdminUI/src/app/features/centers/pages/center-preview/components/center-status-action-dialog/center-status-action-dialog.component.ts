import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { CentersService } from '@app/features/centers/centers.service';
import { ToastService } from '@app/shared/services/toast.service';
import { TranslocoService } from '@ngneat/transloco';
import { FileUpload } from 'primeng/fileupload';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { AttachmentForm } from '@app/features/administrators/administrators.dto';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { CircumstanceStatus, eidManagerStatus } from '@app/features/centers/enums/centers.enum';

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

    @Input() circumstance: any | null = null;
    @Input() managerStatus: eidManagerStatus | null = null;
    @Output() successCallback = new EventEmitter();
    @ViewChild('documentComponent') documentComponent!: FileUpload;

    form = new FormGroup({
        note: new FormGroup({
            content: new FormControl('', [Validators.required]),
        }),
        documents: new FormArray<FormGroup<AttachmentForm>>([]),
    });
    action: CircumstanceStatus | null = null;
    showDialog = false;
    documentTypes: any[] = [];
    documentType = '';

    actionMutation = new RequestHandler({
        requestFunction: this.centersService.approveCenterCircumstanceChange,
        onInit: () => {
            this.form.disable();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate(`${this.translationKey}.toasts.success`)
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

    show(action: CircumstanceStatus) {
        this.action = action;
        this.showDialog = true;
    }

    hide() {
        this.showDialog = false;
        this.form.reset();
        this.form.controls.documents.clear();
    }

    submit() {
        if (!this.circumstance || !this.action) {
            return;
        }
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        this.actionMutation.execute(this.circumstance.id, this.form.getRawValue(), {
            newCircumstancesStatus: this.action,
        });
    }

    get translationKey() {
        switch (this.action) {
            case CircumstanceStatus.STOPPED:
                return 'modules.centers.center-preview.stop-dialog';
            case CircumstanceStatus.ACTIVE:
                return 'modules.centers.center-preview.resume-dialog';
            case CircumstanceStatus.SUSPENDED:
                return 'modules.centers.center-preview.suspend-dialog';
            default:
                return '';
        }
    }

    get titleTranslation() {
        switch (this.action) {
            case CircumstanceStatus.STOPPED:
                return this.translateService.translate('modules.centers.center-preview.stop-dialog.txtTitle');
            case CircumstanceStatus.ACTIVE:
                return this.translateService.translate('modules.centers.center-preview.resume-dialog.txtTitle');
            case CircumstanceStatus.SUSPENDED:
                return this.translateService.translate('modules.centers.center-preview.suspend-dialog.txtTitle');
            default:
                return '';
        }
    }

    get descriptionTranslation() {
        switch (this.action) {
            case CircumstanceStatus.STOPPED:
                return this.translateService.translate('modules.centers.center-preview.stop-dialog.txtDescription');
            case CircumstanceStatus.ACTIVE:
                return this.translateService.translate('modules.centers.center-preview.resume-dialog.txtDescription');
            case CircumstanceStatus.SUSPENDED:
                return this.translateService.translate('modules.centers.center-preview.suspend-dialog.txtDescription');
            default:
                return '';
        }
    }

    get dialogStyleClass() {
        switch (this.action) {
            case CircumstanceStatus.STOPPED:
                return 'error-dialog';
            case CircumstanceStatus.ACTIVE:
                return 'success-dialog';
            case CircumstanceStatus.SUSPENDED:
                return 'warning-dialog';
            default:
                return '';
        }
    }

    get buttonStyleClass() {
        switch (this.action) {
            case CircumstanceStatus.STOPPED:
                return 'p-button-danger';
            case CircumstanceStatus.ACTIVE:
                return 'p-button-success';
            case CircumstanceStatus.SUSPENDED:
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

    protected readonly CircumstanceStatus = CircumstanceStatus;
    protected readonly EidManagerStatus = eidManagerStatus;
}
