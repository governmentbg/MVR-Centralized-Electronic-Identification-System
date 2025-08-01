import { Component, OnDestroy, OnInit } from '@angular/core';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { UserService } from '@app/core/services/user.service';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import {
    ApplicationDetails,
    AttachmentForm,
    documentType,
    documentTypeFormArray,
    documentTypeFormGroup,
} from '@app/features/administrators/administrators.dto';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationStatus } from '@app/features/administrators/enums/administrators.enum';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { FileUpload } from 'primeng/fileupload';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { ToastService } from '@app/shared/services/toast.service';

@Component({
    selector: 'app-application-preview',
    templateUrl: './application-preview.component.html',
    styleUrls: ['./application-preview.component.scss'],
})
export class ApplicationPreviewComponent implements OnInit, OnDestroy {
    constructor(
        private translateService: TranslocoService,
        private userService: UserService,
        private administratorService: AdministratorsService,
        private route: ActivatedRoute,
        private router: Router,
        private toastService: ToastService
    ) {
        const langChanges$ = this.translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.administrators.applications.title'),
                    onClick: () => this.router.navigate(['/administrators']),
                },
                {
                    label: this.translateService.translate('modules.administrators.administrator-preview.txtTitle'),
                },
            ];
        });
        this.subscriptions.add(langChanges$);
    }

    detailsQuery = new RequestHandler({
        requestFunction: this.administratorService.fetchApplicationDetails,
        onSuccess: data => {
            this.application = data;
        },
        onInit: () => {
            this.loading = true;
        },
        onComplete: () => {
            this.loading = false;
        },
    });
    fetchDocumentTypesMutation = new RequestHandler({
        requestFunction: this.administratorService.fetchDocumentTypes,
        onSuccess: data => {
            this.documentTypes = data;
            data.forEach(documentType => {
                const form = this.createAttachmentForm(documentType);
                this.documentForm.controls.attachments.push(form);
            });
        },
    });
    uploadDocumentsQuery = new RequestHandler({
        requestFunction: this.administratorService.uploadDocuments,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.administrators.register.toasts.successfulEdit')
            );
            this.router.navigate(['/administrators']);
        },
        onError: error => {
            switch (error.status) {
                case 400:
                    this.toastService.showErrorToast(
                        this.translateService.translate('global.txtErrorTitle'),
                        this.translateService.translate('global.txtInvalidDataError')
                    );
                    break;
                default:
                    this.toastService.showErrorToast(
                        this.translateService.translate('global.txtErrorTitle'),
                        this.translateService.translate('global.txtUnexpectedError')
                    );
                    break;
            }
        },
        onInit: () => {
            this.loading = true;
        },
        onComplete: () => {
            this.loading = false;
        },
    });
    roles: { name: string; value: string }[] = [];
    application: ApplicationDetails | null = null;
    subscriptions = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading = false;
    ApplicationStatus = ApplicationStatus;
    mode: 'view' | 'edit' = 'view';
    documentTypes: documentType[] = [];
    documentForm: FormGroup<documentTypeFormGroup> = new FormGroup({
        attachments: new FormArray<FormGroup<documentTypeFormArray>>([]),
    });
    Validators = Validators;

    ngOnInit(): void {
        this.loading = true;
        const applicationId = this.route.snapshot.paramMap.get('id');
        if (!applicationId) {
            this.router.navigate(['/administrators']);
            return;
        }
        this.detailsQuery.execute(applicationId);
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.roles = [
                    {
                        name: this.translateService.translate('enums.administratorsAndCentersRoles.administrator'),
                        value: 'ADMINISTRATOR',
                    },
                    {
                        name: this.translateService.translate('enums.administratorsAndCentersRoles.user'),
                        value: 'USER',
                    },
                ];
            })
        );
    }

    ngOnDestroy() {
        this.subscriptions.unsubscribe();
    }

    get applicantPersonalIdentifier(): string {
        return this.translateService.translate('modules.administrators.register.' + this.userService.user.uidType);
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    changeMode(mode: 'view' | 'edit') {
        this.mode = mode;
        if (mode === 'edit') {
            this.fetchDocumentTypesMutation.execute();
        }
        window.scrollTo({
            top: 0,
            behavior: 'smooth',
        });
    }

    createAttachmentForm(documentType: documentType) {
        return new FormGroup({
            files: new FormArray<FormGroup<AttachmentForm>>([], {
                validators: documentType.requiredForAdministrator ? [Validators.required] : [],
            }),
            documentType: new FormControl(documentType.id, {
                nonNullable: true,
            }),
        });
    }

    getDocumentTypeDescriptionById(id: string) {
        const activeLang = this.translateService.getActiveLang().toUpperCase();
        return this.documentTypes.find(documentType => documentType.id === id)?.descriptions[activeLang];
    }

    onFileChange(event: any, attachment: FormGroup<documentTypeFormArray>) {
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
                    documentType: new FormControl(attachment.controls.documentType.value, {
                        nonNullable: true,
                    }),
                });
                attachment.controls.files.push(formGroup);
            };
            reader.readAsDataURL(file);
        });
    }

    onFileClear(attachment: FormGroup<documentTypeFormArray>) {
        attachment.controls.files.clear();
    }

    onFileRemove(fileName: string, attachment: FormGroup<documentTypeFormArray>, fileUpload: FileUpload) {
        const files = attachment.controls.files.getRawValue();
        if (!files?.length) return;
        const index = files?.findIndex(file => file.fileName === fileName);
        attachment.controls.files.removeAt(index);
        fileUpload.remove(new Event('remove'), index);
    }

    onSubmit() {
        this.documentForm.markAllAsTouched();

        markFormGroupAsDirty(this.documentForm);
        if (this.documentForm.invalid || !this.application) return;
        const payload: any[] = [];
        this.documentForm.getRawValue().attachments.forEach(attachment => payload.push(...attachment.files));
        this.uploadDocumentsQuery.execute(payload, this.application.id);
    }

    getApplicationTypeText(type: string) {
        return this.translateService.translate(`modules.administrators.applications.types.${type}`);
    }
}
