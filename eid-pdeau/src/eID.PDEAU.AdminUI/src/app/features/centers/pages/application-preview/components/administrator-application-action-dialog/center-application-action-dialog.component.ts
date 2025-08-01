import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ToastService } from '@app/shared/services/toast.service';
import { TranslocoService } from '@ngneat/transloco';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ApplicationStatus, ApplicationType } from '@app/features/centers/enums/centers.enum';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { ApplicationDetails } from '@app/features/centers/centers.dto';
import { CentersService } from '@app/features/centers/centers.service';
import { FileUpload } from 'primeng/fileupload';
import { AttachmentForm } from '@app/features/administrators/administrators.dto';

@Component({
    selector: 'app-center-application-action-dialog',
    templateUrl: './center-application-action-dialog.component.html',
    styleUrls: ['./center-application-action-dialog.component.scss'],
})
export class CenterApplicationActionDialogComponent implements OnInit {
    constructor(
        private centersService: CentersService,
        private toastService: ToastService,
        private translateService: TranslocoService
    ) {}

    @Input() application: ApplicationDetails | null = null;
    @Output() successCallback = new EventEmitter();
    @ViewChild('documentComponent') documentComponent!: FileUpload;

    form = new FormGroup({
        note: new FormGroup({
            content: new FormControl('', [Validators.required]),
        }),
        documents: new FormArray<FormGroup<AttachmentForm>>([]),
        code: new FormControl(''),
    });
    action: ApplicationStatus | null = null;
    showDialog = false;
    ApplicationStatus = ApplicationStatus;
    documentTypes: any[] = [];
    documentType = '';
    ApplicationType = ApplicationType;

    actionMutation = new RequestHandler({
        requestFunction: this.centersService.changeApplicationStatus,
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

    show(action: ApplicationStatus) {
        this.action = action;
        if (
            this.action === ApplicationStatus.ACCEPTED &&
            this.application?.applicationType === ApplicationType.REGISTER
        ) {
            this.form.controls.code.setValidators([Validators.required]);
            this.form.controls.code.updateValueAndValidity();
        }
        this.showDialog = true;
    }

    hide() {
        this.showDialog = false;
        this.form.reset();
        this.form.controls.documents.clear();
    }

    submit() {
        if (!this.application || !this.action) {
            return;
        }
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        this.actionMutation.execute(this.application.id, this.form.getRawValue(), this.action);
    }

    get translationKey() {
        switch (this.action) {
            case ApplicationStatus.DENIED:
                return 'modules.centers.center-preview.deny-dialog';
            case ApplicationStatus.ACTIVE:
                return 'modules.centers.center-preview.activate-dialog';
            case ApplicationStatus.ACCEPTED:
                return 'modules.centers.center-preview.approve-dialog';
            case ApplicationStatus.RETURNED_FOR_CORRECTION:
                return 'modules.centers.center-preview.return-dialog';
            default:
                return '';
        }
    }

    get titleTranslation() {
        switch (this.action) {
            case ApplicationStatus.DENIED:
                return this.translateService.translate('modules.centers.center-preview.deny-dialog.txtTitle');
            case ApplicationStatus.ACTIVE:
                return this.translateService.translate('modules.centers.center-preview.activate-dialog.txtTitle');
            case ApplicationStatus.ACCEPTED:
                return this.translateService.translate('modules.centers.center-preview.approve-dialog.txtTitle');
            case ApplicationStatus.RETURNED_FOR_CORRECTION:
                return this.translateService.translate('modules.centers.center-preview.return-dialog.txtTitle');
            default:
                return '';
        }
    }

    get descriptionTranslation() {
        switch (this.action) {
            case ApplicationStatus.DENIED:
                return this.translateService.translate('modules.centers.center-preview.deny-dialog.txtDescription');
            case ApplicationStatus.ACTIVE:
                return this.translateService.translate('modules.centers.center-preview.activate-dialog.txtDescription');
            case ApplicationStatus.ACCEPTED:
                return this.translateService.translate('modules.centers.center-preview.approve-dialog.txtDescription');
            case ApplicationStatus.RETURNED_FOR_CORRECTION:
                return this.translateService.translate('modules.centers.center-preview.return-dialog.txtDescription');
            default:
                return '';
        }
    }

    get dialogStyleClass() {
        switch (this.action) {
            case ApplicationStatus.DENIED:
                return 'error-dialog';
            case ApplicationStatus.ACTIVE:
            case ApplicationStatus.ACCEPTED:
                return 'success-dialog';
            case ApplicationStatus.RETURNED_FOR_CORRECTION:
                return 'warning-dialog';
            default:
                return '';
        }
    }

    get buttonStyleClass() {
        switch (this.action) {
            case ApplicationStatus.DENIED:
                return 'p-button-danger';
            case ApplicationStatus.ACTIVE:
            case ApplicationStatus.ACCEPTED:
                return 'p-button-success';
            case ApplicationStatus.RETURNED_FOR_CORRECTION:
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
}
