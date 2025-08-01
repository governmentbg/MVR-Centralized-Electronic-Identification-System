import { Component, OnInit, ViewChild } from '@angular/core';
import { ToastService } from '@app/shared/services/toast.service';
import { TranslocoService } from '@ngneat/transloco';
import {
    AttachmentForm,
    AttachmentFull,
    documentType,
    documentTypeFormArray,
} from '@app/features/administrators/administrators.dto';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { FileUpload } from 'primeng/fileupload';
import { CentersService } from '@app/features/centers/centers.service';
import { ManagerStatusAlertService } from '@app/shared/services/manager-status-alert.service';

@Component({
    selector: 'app-documents',
    templateUrl: './documents.component.html',
    styleUrls: ['./documents.component.scss'],
})
export class DocumentsComponent implements OnInit {
    constructor(
        private toastService: ToastService,
        private translateService: TranslocoService,
        private centersService: CentersService,
        private managerStatusAlertService: ManagerStatusAlertService
    ) {}

    @ViewChild('documentComponent') documentComponent!: FileUpload;
    files: AttachmentFull[] = [];
    documentTypes: documentType[] = [];
    documentType = '';
    showDialog = false;
    documentForm = new FormGroup({
        attachments: new FormArray<FormGroup<AttachmentForm>>([]),
    });
    addDocumentQuery = new RequestHandler({
        requestFunction: this.centersService.createDocument,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.fetchDocumentList.execute();
            this.onClosePreview();
        },
    });
    fetchDocumentTypesMutation = new RequestHandler({
        requestFunction: this.centersService.fetchDocumentTypes,
        onSuccess: data => {
            this.documentTypes = data;
        },
    });
    fetchDocumentList = new RequestHandler({
        requestFunction: this.centersService.fetchAttachments,
        onSuccess: response => {
            this.files = response.attachments;
        },
    });

    ngOnInit() {
        this.fetchDocumentTypesMutation.execute();
        this.fetchDocumentList.execute();
    }

    downloadFile = new RequestHandler({
        requestFunction: this.centersService.downloadFile,
        onSuccess: (attachment, args) => {
            const fileName = args[0];
            const url = `data:application/pdf;base64,${attachment.content}`;
            const a = document.createElement('a');
            a.href = url;
            a.download = fileName;
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.providers.provider-preview.errors.file-unavailable')
            );
        },
    });

    onSubmit() {
        this.documentForm.markAllAsTouched();

        markFormGroupAsDirty(this.documentForm);
        if (this.documentForm.invalid || this.documentForm.controls.attachments.length === 0) {
            return;
        }

        const payload: any = [];
        this.documentForm.getRawValue().attachments.forEach(attachment => payload.push(attachment));

        this.addDocumentQuery.execute(payload);
        this.documentForm.reset();
    }

    add() {
        this.showDialog = true;
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
                this.documentForm.controls.attachments.push(formGroup);
            };
            reader.readAsDataURL(file);
        });
    }

    removeFile(index: number) {
        this.documentForm.controls.attachments.removeAt(index);
    }

    onClosePreview() {
        this.documentForm.reset();
        this.documentForm.controls.attachments.clear();
        this.showDialog = false;
    }
}
