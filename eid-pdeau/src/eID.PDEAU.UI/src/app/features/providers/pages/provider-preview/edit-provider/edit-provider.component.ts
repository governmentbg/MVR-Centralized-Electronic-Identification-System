import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '@app/core/services/user.service';
import { IdentifierType } from '@app/shared/enums';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { BulstatValidator, EGNAdultValidator, EGNValidator } from '@app/shared/validators/uid-validator';
import { TranslocoService } from '@ngneat/transloco';
import { format } from 'date-fns';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { ToastService } from '@app/shared/services/toast.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { ProviderService } from '@app/features/providers/provider.service';
import {
    AdministrativeBodyType,
    DetailedProviderType,
    providerSubject,
    ProviderSubjectType,
    UserType,
} from '@app/features/providers/provider.dto';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { cyrillicAddressValidator } from '@app/shared/validators/cyrillic-address-validator';
import { maxFileSizeValidator } from '@app/shared/validators/max-file-size-validator';

@Component({
    selector: 'app-edit-provider',
    templateUrl: './edit-provider.component.html',
    styleUrls: ['./edit-provider.component.scss'],
    providers: [MessageService],
})
export class EditProviderComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private userService: UserService,
        private translateService: TranslocoService,
        private providerService: ProviderService,
        private router: Router,
        private toastService: ToastService,
        private messageService: MessageService
    ) {}

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                const issuerUidTypeTranslation = this.getIssuerUidTypeTranslation();
                this.form.controls.applicant.controls.uidType.setValue(issuerUidTypeTranslation);

                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.providers.applications.title'),
                        onClick: () => this.router.navigate(['/providers']),
                    },
                    {
                        label: this.translateService.translate('modules.providers.provider-preview.txtTitle'),
                        onClick: () => this.closeEdit.emit(),
                    },
                    {
                        label: this.translateService.translate('modules.providers.provider-edit.txtTitle'),
                    },
                ];
            })
        );

        const administrator = this.provider.users.find(user => user.isAdministrator);

        if (!administrator) return;

        this.form.patchValue({
            applicant: {
                uidType: this.getIssuerUidTypeTranslation(),
                uid: this.provider.issuerUid,
                name: this.provider.issuerName,
            },
            administrativeBody: {
                name: this.provider.name,
                bulstat: this.provider.bulstat,
                identificationNumber: this.provider.identificationNumber,
                headquarters: this.provider.headquarters,
                address: this.provider.address,
                email: this.provider.email,
                phone: this.provider.phone,
            },
            administrator: {
                uidType: administrator.uidType as IdentifierType,
                uid: administrator.uid,
                name: administrator.name,
                email: administrator.email,
                phone: administrator.phone,
            },
        });

        if (this.provider.type === providerSubject.Enum.PrivateLawSubject) {
            (this.form.controls.administrativeBody as FormGroup).removeControl('identificationNumber');
        }

        this.providerSubjectType = this.provider.type || providerSubject.Enum.Administration;
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    @Input() provider!: DetailedProviderType;
    @Output() closeEdit = new EventEmitter<boolean>();

    @ViewChild('fileInput') fileInput!: FileUpload;

    breadcrumbItems: IBreadCrumbItems[] = [];

    showDialog = false;

    getIssuerUidTypeTranslation() {
        return this.translateService.translate('enums.identifierType.' + this.provider.issuerUidType);
    }

    hideDialog() {
        this.showDialog = false;
    }

    form = new FormGroup({
        applicant: new FormGroup({
            uidType: new FormControl({ value: '', disabled: true }),
            uid: new FormControl({ value: '', disabled: true }),
            name: new FormControl({ value: '', disabled: true }),
        }),
        administrativeBody: new FormGroup({
            name: new FormControl('', {
                validators: [Validators.required],
                nonNullable: true,
            }),
            bulstat: new FormControl(
                {
                    value: '',
                    disabled: true,
                },
                [Validators.required, BulstatValidator()]
            ),
            identificationNumber: new FormControl(
                {
                    value: '',
                    disabled: true,
                },
                [Validators.required]
            ),
            headquarters: new FormControl('', [Validators.required, cyrillicAddressValidator()]),
            address: new FormControl(''),
            email: new FormControl('', [Validators.required, Validators.email]),
            phone: new FormControl('', {
                validators: [Validators.required],
                nonNullable: true,
            }),
        }),
        administrator: new FormGroup({
            uidType: new FormControl(IdentifierType.EGN, [Validators.required]),
            uid: new FormControl('', [Validators.required, EGNAdultValidator(), EGNValidator()]),
            name: new FormControl('', [Validators.required]),
            email: new FormControl('', [Validators.required, Validators.email]),
            phone: new FormControl('', [Validators.required]),
        }),

        files: new FormControl<File[] | null>(null, [Validators.required, maxFileSizeValidator(10)]),
    });

    providerSubjectType: ProviderSubjectType = providerSubject.Enum.Administration;

    commentForm = new FormGroup({
        comment: new FormControl('', [Validators.required]),
    });

    updateMutation = new RequestHandler({
        requestFunction: this.providerService.updateProvider,
        onInit: () => {
            this.clearDuplicatedFiles();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.providers.provider-edit.toasts.successfulEdit')
            );
            this.closeEdit.emit(true);
        },
        onError: errorResponse => {
            let errorMessage = this.translateService.translate('global.txtSomethingWentWrong');
            switch (errorResponse.status) {
                case 400: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;

                    if (firstKey === 'Files') {
                        const fileNames = errorResponse.errors.Files[0].match(/\b[^\s,]+\.pdf\b/g);
                        this.showDuplicatedFiles(fileNames);
                        this.showDialog = false;
                        return;
                    }
                    if (firstKey === 'files[0]') {
                        const fileNameRegex = errorResponse.errors[firstKey]?.[0]?.match(
                            /(?<=File\s)(.*?)(?=\sis not signed\.)/
                        );
                        errorMessage = this.translateService.translate(
                            'modules.providers.provider-edit.toasts.fileNotSigned',
                            { fileName: fileNameRegex[0] }
                        );
                        this.showDialog = false;

                        break;
                    }
                    break;
                }

                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Uid') {
                        errorMessage = this.translateService.translate('modules.providers.provider-edit.toasts.Uid');
                    }
                }
            }

            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);

            this.closeEdit.emit();
        },
    });

    currentDate = format(new Date(), 'dd.MM.yyyy');

    get applicantPersonalIdentifier(): string {
        return this.translateService.translate('modules.providers.register.' + this.userService.user.uidType);
    }

    onSubmit() {
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        this.showDialog = true;
    }

    onCommentFormSubmit() {
        this.commentForm.markAllAsTouched();

        markFormGroupAsDirty(this.commentForm);
        if (this.commentForm.invalid) return;

        const comment = this.commentForm.controls.comment.value || '';

        this.updateMutation.execute({
            providerId: this.provider.id,
            administrativeBody: this.form.controls.administrativeBody.getRawValue() as AdministrativeBodyType,
            administrator: this.form.controls.administrator.value as UserType,
            files: this.form.controls.files.getRawValue() as File[],
            comment,
        });
    }

    showDuplicatedFiles(files: string[]) {
        this.messageService.clear();
        this.messageService.add({
            key: 'edit-provider-file-upload',
            severity: 'error',
            detail: files as any,
            sticky: true,
        });
    }

    clearDuplicatedFiles() {
        this.messageService.clear('edit-provider-file-upload');
    }

    onFileChange({ currentFiles }: { currentFiles: File[] }) {
        let _currentFiles = [...currentFiles];
        const duplicatedFiles = _currentFiles.filter(file => {
            const files = this.provider.files;
            return files?.some(f => f.fileName === file.name);
        });

        if (duplicatedFiles.length) {
            this.showDuplicatedFiles(duplicatedFiles.map(file => file.name));
            this.fileInput.files = this.fileInput.files.filter(
                file => !duplicatedFiles.some(f => f.name === file.name)
            );
            _currentFiles = _currentFiles.filter(file => !duplicatedFiles.some(f => f.name === file.name));
        }

        if (_currentFiles.length) {
            this.form.controls.files.setValue(_currentFiles);
        }
    }

    onFileClear() {
        this.form.controls.files.setValue(null);
    }

    onFileRemove(fileName: string) {
        const files = this.form.controls.files.value;
        if (!files?.length) return;
        const filteredFiles = files.filter(file => file.name !== fileName);
        this.form.controls.files.setValue(filteredFiles);
        this.fileInput.files = this.fileInput.files.filter(file => file.name !== fileName);
    }
}
