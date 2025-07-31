import { Component, OnDestroy } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import { ToastService } from '@app/shared/services/toast.service';
import { ConfirmationService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { DocumentTypesService } from '@app/features/document-types/document-types.service';

@Component({
    selector: 'app-document-types-form',
    templateUrl: './document-types-form.component.html',
    styleUrls: ['./document-types-form.component.scss'],
    providers: [ConfirmationService],
})
export class DocumentTypesFormComponent implements OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        private documentTypesService: DocumentTypesService,
        private router: Router
    ) {
        if (this.router.url === '/document-types/new') {
            this.isEditMode = false;
        } else if (this.router.url === '/document-types/edit') {
            this.isEditMode = true;
        }

        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.documentTypes.title'),
                    routerLink: '/document-types',
                },
                {
                    label: this.translateService.translate(this.breadcrumbTitle),
                },
            ];
        });

        if (!this.isEditMode) return;
        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            this.state = navigation.extras.state as any;
            if (this.state) {
                this.form.patchValue({
                    name: this.state.documentType.name,
                    descriptions: this.state.documentType.descriptions,
                    requiredForAdministrator: this.state.documentType.requiredForAdministrator,
                    requiredForCenter: this.state.documentType.requiredForCenter,
                });
                this.documentTypeId = this.state.documentType.id;
            }
        } else {
            this.router.navigate(['document-types']);
        }
    }

    isEditMode = false;
    languageChangeSubscription: Subscription;
    breadcrumbItems: IBreadCrumbItems[] = [];
    isLoading = false;
    state: any = {};
    documentTypeId = '';

    get breadcrumbTitle() {
        return this.isEditMode ? 'modules.documentTypes.edit.title' : 'modules.documentTypes.create.title';
    }

    form = new FormGroup({
        name: new FormControl('', Validators.required),
        descriptions: new FormGroup({
            EN: new FormControl('', Validators.required),
            BG: new FormControl('', Validators.required),
        }),
        requiredForAdministrator: new FormControl(false),
        requiredForCenter: new FormControl(false),
    });

    addDocumentTypeQuery = new RequestHandler({
        requestFunction: this.documentTypesService.createDocumentType,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.documentTypes.create.toasts.successfulCreation')
            );
            this.router.navigate(['/document-types']);
        },
        onError: error => {
            this.form.enable();
            switch (error.status) {
                case 400:
                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                    break;
                default:
                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                    break;
            }
        },
        onInit: () => {
            this.form.disable();
        },
    });

    updateDocumentTypeQuery = new RequestHandler({
        requestFunction: this.documentTypesService.updateDocumentType,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.documentTypes.create.toasts.successfulCreation')
            );
            this.router.navigate(['/document-types']);
        },
        onError: error => {
            this.form.enable();
            switch (error.status) {
                case 400:
                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                    break;
                default:
                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                    break;
            }
        },
        onInit: () => {
            this.form.disable();
        },
    });

    addDocumentType() {
        this.isLoading = true;
        const payload = this.form.getRawValue();
        this.addDocumentTypeQuery.execute(payload);
    }

    editDocumentTypeConfirmation() {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            icon: 'pi pi-exclamation-triangle',
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate('modules.tariffs.confirmChange'),
            accept: () => {
                this.editDocumentType();
            },
        });
    }

    editDocumentType() {
        const payload: any = this.form.getRawValue();
        payload['id'] = this.documentTypeId;
        this.updateDocumentTypeQuery.execute(payload);
    }

    onSubmit() {
        Object.keys(this.form.controls).forEach(key => {
            if (
                (this.form.get(key) && typeof this.form.get(key)?.value === 'string') ||
                this.form.get(key)?.value instanceof String
            ) {
                this.form.get(key)?.setValue(this.form.get(key)?.value.trim());
            }
            this.form.get(key)?.markAsDirty();
        });
        this.form.markAllAsTouched();
        if (this.form.valid) {
            if (this.isEditMode) {
                this.editDocumentTypeConfirmation();
            } else {
                this.addDocumentType();
            }
        }
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }
}
