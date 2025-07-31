import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { UserService } from '@app/core/services/user.service';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { Subscription } from 'rxjs';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import {
    ApplicationType,
    CircumstanceStatus,
    eidManagerStatus,
} from '@app/features/administrators/enums/administrators.enum';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { ToastService } from '@app/shared/services/toast.service';
import { AttachmentForm } from '@app/features/administrators/administrators.dto';
import { FileUpload } from 'primeng/fileupload';

@Component({
    selector: 'app-administrator-preview',
    templateUrl: './administrator-preview.component.html',
    styleUrls: ['./administrator-preview.component.scss'],
})
export class AdministratorPreviewComponent implements OnInit {
    constructor(
        private translateService: TranslocoService,
        private userService: UserService,
        private administratorService: AdministratorsService,
        private route: ActivatedRoute,
        private toastService: ToastService,
        private router: Router
    ) {
        const langChanges$ = this.translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.administrators.registrations.title'),
                    onClick: () => this.router.navigate(['/administrators/registrations']),
                },
                {
                    label: this.translateService.translate('modules.administrators.registrations.preview'),
                },
            ];
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
        });
        this.subscriptions.add(langChanges$);
        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            this.state = navigation.extras.state as any;
        }
    }

    @ViewChild('documentComponent') documentComponent!: FileUpload;
    detailsQuery = new RequestHandler({
        requestFunction: this.administratorService.fetchAdministratorDetails,
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
        },
    });
    application: any | null = null;
    subscriptions = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading = false;
    roles: { name: string; value: string }[] = [];
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
    state: any;
    ApplicationType = ApplicationType;

    ngOnInit(): void {
        this.loading = true;
        const applicationId = this.route.snapshot.paramMap.get('id');
        if (!applicationId) {
            this.router.navigate(['/administrators/registrations']);
            return;
        }
        this.fetchDocumentTypesMutation.execute();
        if (this.state) {
            this.detailsQuery.execute(applicationId, this.state.onlyActiveElements);
        } else {
            this.detailsQuery.execute(applicationId);
        }
    }

    onClosePreview() {
        this.router.navigate(['/administrators/registrations']);
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    actionMutation = new RequestHandler({
        requestFunction: this.administratorService.approveAdministratorCircumstanceChange,
        onInit: () => {
            this.form.disable();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate(`${this.translationKey}.toasts.success`)
            );
            this.onClosePreview();
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate(`${this.translationKey}.toasts.error`)
            );
            this.form.enable();
        },
    });

    show(action: CircumstanceStatus) {
        this.action = action;
        this.showDialog = true;
    }

    hide() {
        this.showDialog = false;
    }

    submit() {
        if (!this.application || !this.action) {
            return;
        }
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        this.actionMutation.execute(this.application.id, this.form.getRawValue(), {
            newCircumstancesStatus: this.action,
        });
    }

    get translationKey() {
        switch (this.action) {
            case CircumstanceStatus.FOR_CORRECTION:
                return 'modules.administrators.circumstances.return-dialog';
            case CircumstanceStatus.APPROVED:
                return 'modules.administrators.circumstances.active-dialog';
            default:
                return '';
        }
    }

    get titleTranslation() {
        switch (this.action) {
            case CircumstanceStatus.FOR_CORRECTION:
                return this.translateService.translate('modules.administrators.circumstances.return-dialog.txtTitle');
            case CircumstanceStatus.APPROVED:
                return this.translateService.translate('modules.administrators.circumstances.active-dialog.txtTitle');
            default:
                return '';
        }
    }

    get descriptionTranslation() {
        switch (this.action) {
            case CircumstanceStatus.FOR_CORRECTION:
                return this.translateService.translate(
                    'modules.administrators.circumstances.return-dialog.txtDescription'
                );
            case CircumstanceStatus.APPROVED:
                return this.translateService.translate(
                    'modules.administrators.circumstances.active-dialog.txtDescription'
                );
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

    createApplication(type: string) {
        // this.createApplicationQuery.execute({ type });
    }

    protected readonly ManagerStatus = eidManagerStatus;
    protected readonly CircumstanceStatus = CircumstanceStatus;
}
