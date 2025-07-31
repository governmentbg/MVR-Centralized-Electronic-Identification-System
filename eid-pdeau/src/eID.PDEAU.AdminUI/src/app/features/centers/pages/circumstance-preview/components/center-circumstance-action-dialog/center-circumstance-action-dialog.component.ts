import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ToastService } from '@app/shared/services/toast.service';
import { TranslocoService } from '@ngneat/transloco';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { CircumstanceStatus } from '@app/features/administrators/enums/administrators.enum';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { CentersService } from '@app/features/centers/centers.service';
import { AttachmentForm } from '@app/features/administrators/administrators.dto';
import { FileUpload } from 'primeng/fileupload';

@Component({
    selector: 'app-center-circumstance-action-dialog',
    templateUrl: './center-circumstance-action-dialog.component.html',
    styleUrls: ['./center-circumstance-action-dialog.component.scss'],
})
export class CenterCircumstanceActionDialogComponent implements OnInit {
    constructor(
        private centersService: CentersService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() circumstance: any | null = null;
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
            case CircumstanceStatus.APPROVED:
                return 'modules.centers.circumstances.approve-dialog';
            case CircumstanceStatus.FOR_CORRECTION:
                return 'modules.centers.circumstances.return-dialog';
            default:
                return '';
        }
    }

    get titleTranslation() {
        switch (this.action) {
            case CircumstanceStatus.APPROVED:
                return this.translateService.translate('modules.centers.circumstances.approve-dialog.txtTitle');
            case CircumstanceStatus.FOR_CORRECTION:
                return this.translateService.translate('modules.centers.circumstances.return-dialog.txtTitle');
            default:
                return '';
        }
    }

    get descriptionTranslation() {
        switch (this.action) {
            case CircumstanceStatus.APPROVED:
                return this.translateService.translate('modules.centers.circumstances.approve-dialog.txtDescription');
            case CircumstanceStatus.FOR_CORRECTION:
                return this.translateService.translate('modules.centers.circumstances.return-dialog.txtDescription');
            default:
                return '';
        }
    }

    get dialogStyleClass() {
        switch (this.action) {
            case CircumstanceStatus.APPROVED:
                return 'success-dialog';
            case CircumstanceStatus.FOR_CORRECTION:
                return 'warning-dialog';
            default:
                return '';
        }
    }

    get buttonStyleClass() {
        switch (this.action) {
            case CircumstanceStatus.APPROVED:
                return 'p-button-success';
            case CircumstanceStatus.FOR_CORRECTION:
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
}
