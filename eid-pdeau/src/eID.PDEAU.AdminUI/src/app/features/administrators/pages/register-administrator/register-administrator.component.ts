import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import { ToastService } from '@app/shared/services/toast.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { ConfirmationService } from 'primeng/api';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import {
    ApplicationForm,
    AttachmentForm,
    DeviceForm,
    documentType,
    documentTypeFormArray,
    documentTypeFormGroup,
    EidManagerFrontOfficeForm,
    EmployeeForm,
    PersonForm,
} from '@app/features/administrators/administrators.dto';
import { FileUpload } from 'primeng/fileupload';
import { IdentifierType } from '@app/shared/enums';
import { BulstatValidator, EGNAdultValidator, EGNValidator, PinValidator } from '@app/shared/validators/uid-validator';

@Component({
    selector: 'app-register-administrator',
    templateUrl: './register-administrator.component.html',
    styleUrls: ['./register-administrator.component.scss'],
    providers: [ConfirmationService],
})
export class RegisterAdministratorComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private translateService: TranslocoService,
        private router: Router,
        private toastService: ToastService,
        private confirmationService: ConfirmationService,
        private administratorService: AdministratorsService
    ) {}

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.administrators.applications.title'),
                        onClick: () => this.router.navigate(['/administrators']),
                    },
                    {
                        label: this.translateService.translate('modules.administrators.register.title'),
                    },
                ];
                this.items = [
                    {
                        id: 0,
                        label: this.translateService.translate('modules.administrators.register.generalInfo'),
                    },
                    {
                        id: 1,
                        label: this.translateService.translate('modules.administrators.register.legalRepresentatives'),
                    },
                    {
                        id: 2,
                        label: this.translateService.translate('modules.administrators.register.devices'),
                    },
                    {
                        id: 3,
                        label: this.translateService.translate('modules.administrators.register.offices'),
                    },
                    {
                        id: 4,
                        label: this.translateService.translate('modules.administrators.register.employees'),
                    },
                    {
                        id: 5,
                        label: this.translateService.translate('modules.administrators.register.documents'),
                    },
                    {
                        id: 6,
                        label: this.translateService.translate('modules.administrators.register.preview'),
                    },
                ];
                this.identifierTypes = [
                    {
                        name: this.translateService.translate('enums.identifierType.EGN'),
                        id: IdentifierType.EGN,
                    },
                    {
                        name: this.translateService.translate('enums.identifierType.LNCh'),
                        id: IdentifierType.LNCh,
                    },
                ];
            })
        );
        this.subscriptions.add(
            this.applicationForm.controls.applicant.controls.citizenIdentifierType.valueChanges.subscribe(value => {
                const validators = [Validators.required];
                if (value === IdentifierType.EGN) {
                    validators.push(EGNValidator(), EGNAdultValidator());
                } else if (value === IdentifierType.LNCh) {
                    validators.push(PinValidator());
                }
                const control = this.applicationForm.controls.applicant.controls.citizenIdentifierNumber;
                control.setValidators(validators);
                control.updateValueAndValidity();
            })
        );
        this.fetchDocumentTypesMutation.execute();
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }
    breadcrumbItems: IBreadCrumbItems[] = [];
    latinNamePattern = /^(?=.)[a-zA-Z\s']*$/;
    latinCompanyNamePattern = /^(?=.)[a-zA-Z\D\d\s']*$/;
    applicationForm: FormGroup<ApplicationForm> = new FormGroup<ApplicationForm>({
        referenceNumber: new FormControl('', {
            validators: [Validators.required, Validators.pattern(/^[a-zA-Z0-9-]+$/)],
            nonNullable: true,
        }),
        employees: new FormArray<FormGroup<EmployeeForm>>([], [Validators.required]),
        eidManagerFrontOffices: new FormArray<FormGroup<EidManagerFrontOfficeForm>>([]),
        devices: new FormArray<FormGroup<DeviceForm>>([]),
        authorizedPersons: new FormArray<FormGroup<PersonForm>>([]),
        applicant: new FormGroup<PersonForm>({
            citizenIdentifierType: new FormControl(IdentifierType.EGN, {
                validators: [Validators.required],
                nonNullable: true,
            }),
            citizenIdentifierNumber: new FormControl('', {
                validators: [Validators.required, EGNAdultValidator(), EGNValidator()],
                nonNullable: true,
            }),
            name: new FormControl('', {
                validators: [Validators.required],
                nonNullable: true,
            }),
            nameLatin: new FormControl('', {
                validators: [Validators.required, Validators.pattern(this.latinNamePattern)],
                nonNullable: true,
            }),
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
            validators: [Validators.required, Validators.pattern(this.latinCompanyNamePattern)],
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
        downloadUrl: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
    });
    documentTypes: documentType[] = [];
    documentForm: FormGroup<documentTypeFormGroup> = new FormGroup({
        attachments: new FormArray<FormGroup<documentTypeFormArray>>([]),
    });
    identifierTypes: { name: string; id: string }[] = [];
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
    registerMutation = new RequestHandler({
        requestFunction: this.administratorService.registerAdministrator,
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.administrators.register.toasts.successfulRegistration')
            );
            this.router.navigate(['/administrators']);
        },
        onError: errorResponse => {
            // Default error message
            let errorMessage = this.translateService.translate(
                'modules.administrators.register.toasts.failedRegistration'
            );
            switch (errorResponse.status) {
                case 400: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;

                    if (firstKey === 'files[0]') {
                        const fileNameRegex = errorResponse.errors[firstKey]?.[0]?.match(
                            /(?<=File\s)(.*?)(?=\sis not signed\.)/
                        );
                        errorMessage = this.translateService.translate(
                            'modules.administrators.administrators.toasts.fileNotSigned',
                            { fileName: fileNameRegex[0] }
                        );

                        break;
                    }
                    break;
                }
                case 403:
                    errorMessage = this.translateService.translate(
                        'modules.administrators.register.toasts.failedRegistryCheck'
                    );
                    break;
                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Uid') {
                        errorMessage = this.translateService.translate('modules.administrators.register.toasts.Uid');
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
                this.router.navigate(['administrators']);
            },
        });
    }

    get files() {
        return this.applicationForm.controls.attachments;
    }
}
