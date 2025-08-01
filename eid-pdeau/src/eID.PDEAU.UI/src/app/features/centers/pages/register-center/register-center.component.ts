import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserService } from '@app/core/services/user.service';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import { ToastService } from '@app/shared/services/toast.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { ConfirmationService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { BulstatValidator } from '@app/shared/validators/uid-validator';
import { CentersService } from '@app/features/centers/centers.service';
import {
    ApplicationForm,
    AttachmentForm,
    documentType,
    documentTypeFormArray,
    documentTypeFormGroup,
    EidManagerFrontOfficeForm,
    EmployeeForm,
    PersonForm,
} from '@app/features/centers/centers.dto';

@Component({
    selector: 'app-register-center',
    templateUrl: './register-center.component.html',
    styleUrls: ['./register-center.component.scss'],
    providers: [ConfirmationService],
})
export class RegisterCenterComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private userService: UserService,
        private translateService: TranslocoService,
        private router: Router,
        private toastService: ToastService,
        private confirmationService: ConfirmationService,
        private centersService: CentersService,
    ) {}

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.centers.applications.title'),
                        onClick: () => this.router.navigate(['/centers']),
                    },
                    {
                        label: this.translateService.translate('modules.centers.register.title'),
                    },
                ];
                this.items = [
                    {
                        id: 0,
                        label: this.translateService.translate('modules.centers.register.generalInfo'),
                    },
                    {
                        id: 1,
                        label: this.translateService.translate('modules.centers.register.legalRepresentatives'),
                    },
                    {
                        id: 2,
                        label: this.translateService.translate('modules.centers.register.offices'),
                    },
                    {
                        id: 3,
                        label: this.translateService.translate('modules.centers.register.employees'),
                    },
                    {
                        id: 4,
                        label: this.translateService.translate('modules.centers.register.documents'),
                    },
                    {
                        id: 5,
                        label: this.translateService.translate('modules.centers.register.preview'),
                    },
                ];
            })
        );
        this.fetchDocumentTypesMutation.execute();
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }
    breadcrumbItems: IBreadCrumbItems[] = [];
    latinNamePattern = /^(?=.)[a-zA-Z\D\d\s']*$/;
    applicationForm: FormGroup<ApplicationForm> = new FormGroup<ApplicationForm>({
        employees: new FormArray<FormGroup<EmployeeForm>>([], [Validators.required]),
        eidManagerFrontOffices: new FormArray<FormGroup<EidManagerFrontOfficeForm>>([], [Validators.required]),
        authorizedPersons: new FormArray<FormGroup<PersonForm>>([]),
        applicant: new FormGroup<PersonForm>({
            citizenIdentifierType: new FormControl(
                { value: this.userService.user.uidType, disabled: true },
                { nonNullable: true }
            ),
            citizenIdentifierNumber: new FormControl(
                { value: this.userService.getUser().uid, disabled: true },
                { nonNullable: true }
            ),
            name: new FormControl({ value: this.userService.getUser().name, disabled: true }, { nonNullable: true }),
            nameLatin: new FormControl(
                { value: this.userService.getUser().nameLatin, disabled: true },
                {
                    nonNullable: true,
                }
            ),
            phoneNumber: new FormControl('', {
                validators: [Validators.required],
                nonNullable: true,
            }),
            email: new FormControl('', {
                validators: [Validators.required, Validators.email],
                nonNullable: true,
            }),
        }),
        applicationType: new FormControl('REGISTER', {
            validators: [Validators.required],
            nonNullable: true,
        }),
        attachments: new FormArray<FormGroup<AttachmentForm>>([]),
        homePage: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
        companyName: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
        companyNameLatin: new FormControl('', {
            validators: [Validators.required, Validators.pattern(this.latinNamePattern)],
            nonNullable: true,
        }),
        eikNumber: new FormControl('', {
            validators: [Validators.required, Validators.pattern('[0-9]+'), BulstatValidator()],
            nonNullable: true,
        }),
        address: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
        email: new FormControl('', {
            validators: [Validators.required, Validators.email],
            nonNullable: true,
        }),
        phone: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
        description: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
        logoUrl: new FormControl('', {
            nonNullable: true,
        }),
    });
    documentTypes: documentType[] = [];
    documentForm: FormGroup<documentTypeFormGroup> = new FormGroup({
        attachments: new FormArray<FormGroup<documentTypeFormArray>>([]),
    });
    fetchDocumentTypesMutation = new RequestHandler({
        requestFunction: this.centersService.fetchDocumentTypes,
        onSuccess: data => {
            this.documentTypes = data;
            data.forEach(documentType => {
                const form = this.createAttachmentForm(documentType);
                this.documentForm.controls.attachments.push(form);
            });
        },
    });
    registerMutation = new RequestHandler({
        requestFunction: this.centersService.registerCenter,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.centers.register.toasts.successfulRegistration')
            );
            this.router.navigate(['/centers']);
        },
        onError: errorResponse => {
            // Default error message
            let errorMessage = this.translateService.translate(
                'modules.centers.register.toasts.failedRegistration'
            );
            switch (errorResponse.status) {
                case 400: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;

                    if (firstKey === 'files[0]') {
                        const fileNameRegex = errorResponse.errors[firstKey]?.[0]?.match(
                            /(?<=File\s)(.*?)(?=\sis not signed\.)/
                        );
                        errorMessage = this.translateService.translate(
                            'modules.centers.administrators.toasts.fileNotSigned',
                            { fileName: fileNameRegex[0] }
                        );

                        break;
                    }
                    break;
                }
                case 403:
                    errorMessage = this.translateService.translate(
                        'modules.centers.register.toasts.failedRegistryCheck'
                    );
                    break;
                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Uid') {
                        errorMessage = this.translateService.translate('modules.centers.register.toasts.Uid');
                    }
                }
            }

            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
        },
    });
    items: any[] = [];
    activeIndex = 0;
    Validators = Validators;

    onActiveIndexChange(event: any) {
        this.activeIndex = event;
    }

    get applicantPersonalIdentifier(): string {
        return this.translateService.translate('modules.centers.register.' + this.userService.user.uidType);
    }

    createAttachmentForm(documentType: documentType) {
        return new FormGroup({
            files: new FormArray<FormGroup<AttachmentForm>>([], {
                validators: documentType.requiredForCenter ? [Validators.required] : [],
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

    onSubmit() {
        this.applicationForm.markAllAsTouched();
        this.documentForm.markAllAsTouched();

        markFormGroupAsDirty(this.applicationForm);
        markFormGroupAsDirty(this.documentForm);
        if (this.applicationForm.invalid || this.documentForm.invalid) {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('global.txtApplicationValidationError')
            );
            return;
        }

        const payload = this.applicationForm.getRawValue();
        this.documentForm
            .getRawValue()
            .attachments.forEach(attachment => payload.attachments.push(...attachment.files));

        this.registerMutation.execute(payload);
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

    cancelRegister() {
        this.confirmationService.confirm({
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate('modules.services.service-preview.backDialogTitle'),
            rejectButtonStyleClass: 'p-button-danger',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.router.navigate(['centers']);
            },
        });
    }
}
