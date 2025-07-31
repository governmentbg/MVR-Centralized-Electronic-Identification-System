import { Component, OnDestroy, OnInit } from '@angular/core';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { DatasetsService } from '../../services/datasets.service';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { ToastService } from 'src/app/shared/services/toast.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { IDataset, IDatasetRequestPayload } from '../../interfaces/IDatasets';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CronPeriods } from '../../enums/cron-enum';
import { URLValidator } from 'src/app/shared/validators/url';

@Component({
    selector: 'app-dataset-form',
    templateUrl: './dataset-form.component.html',
    styleUrls: ['./dataset-form.component.scss'],
    providers: [ConfirmationService],
})
export class DatasetFormComponent implements OnDestroy {
    constructor(
        private datasetsService: DatasetsService,
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        private router: Router
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                { label: this.translateService.translate('modules.datasets.txtOpenData'), routerLink: '/datasets' },
                {
                    label: this.translateService.translate('modules.datasets.' + this.breadcrumbTitle),
                },
            ];
        });

        this.formChangeSubscription = this.form.controls.isActive.valueChanges.subscribe(value => {
            if (value) {
                this.form.controls.cronPeriod.setValidators([Validators.required]);
                this.form.controls.cronPeriod.patchValue(CronPeriods.Monthly);
                this.form.controls.cronPeriod.updateValueAndValidity();
                this.form.controls.cronPeriod.enable();
            } else {
                this.form.controls.cronPeriod.setValidators([]);
                this.form.controls.cronPeriod.patchValue('');
                this.form.controls.cronPeriod.updateValueAndValidity();
                this.form.controls.cronPeriod.disable();
            }
        });

        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            this.state = navigation.extras.state as any;
            if (this.state) {
                this.isEditMode = this.state.isEditMode;
                this.isPreviewMode = this.state.isPreviewMode;
                this.breadcrumbItems = [
                    { label: this.translateService.translate('modules.datasets.txtOpenData'), routerLink: '/datasets' },
                    {
                        label: this.translateService.translate('modules.datasets.' + this.breadcrumbTitle),
                    },
                ];

                if (
                    this.state.dataset.cronPeriod !== CronPeriods.Weekly &&
                    this.state.dataset.cronPeriod !== CronPeriods.Monthly &&
                    this.state.dataset.cronPeriod !== CronPeriods.Yearly
                ) {
                    this.state.dataset.cronPeriod = null;
                }
                this.form.patchValue({
                    datasetName: this.state.dataset.datasetName,
                    dataSource: this.state.dataset.dataSource,
                    cronPeriod: this.state.dataset.cronPeriod,
                    isActive: this.state.dataset.isActive,
                    description: this.state.dataset.description,
                });

                if (this.isPreviewMode) {
                    this.form.disable();
                }
            }
        }
    }

    isEditMode = false;
    isPreviewMode = false;
    languageChangeSubscription: Subscription;
    breadcrumbItems: IBreadCrumbItems[] = this._initialBreadcrumbs;
    isLoading = false;
    state: any = {};
    formChangeSubscription: Subscription;

    get breadcrumbTitle() {
        return this.isEditMode ? 'txtChange' : 'txtAddNew';
    }

    get _initialBreadcrumbs(): IBreadCrumbItems[] {
        return ([] as IBreadCrumbItems[]).concat([
            { label: this.translateService.translate('modules.datasets.txtOpenData'), routerLink: '/datasets' },
            {
                label: this.translateService.translate('modules.datasets.' + this.breadcrumbTitle),
            },
        ]);
    }

    form = new FormGroup({
        datasetName: new FormControl<string | null>(null, Validators.required),
        dataSource: new FormControl<string | null>('', [Validators.required, URLValidator()]),
        cronPeriod: new FormControl<string | null>(CronPeriods.Monthly, Validators.required),
        isActive: new FormControl<boolean | null>(true),
        description: new FormControl<string>(''),
    });

    cronPeriodTypes = [
        {
            name: this.translateService.translate('modules.datasets.txtWeekly'),
            id: CronPeriods.Weekly,
        },
        {
            name: this.translateService.translate('modules.datasets.txtMonthly'),
            id: CronPeriods.Monthly,
        },
        {
            name: this.translateService.translate('modules.datasets.txtYearly'),
            id: CronPeriods.Yearly,
        },
    ];

    addDataset() {
        this.isLoading = true;
        const payload: IDatasetRequestPayload = {
            datasetName: this.form.controls.datasetName.value as string,
            cronPeriod: this.form.controls.cronPeriod.value as string,
            dataSource: this.form.controls.dataSource.value as string,
            isActive: this.form.controls.isActive.value as boolean,
            description: this.form.controls.description.value as string,
        };
        this.datasetsService.createDataset(payload).subscribe({
            next: (response: any) => {
                this.isLoading = false;
                this.showSuccessToast(this.translateService.translate('modules.datasets.txtSuccessfullyAdded'));
                this.router.navigate(['/datasets']);
            },
            error: error => {
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                        break;
                }
                this.isLoading = false;
            },
        });
    }

    editDatasetConfirmation() {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger p-button-sm',
            header: this.translateService.translate('modules.datasets.txtConfirmation'),
            message: this.translateService.translate('modules.datasets.txtConfirmChange'),
            acceptLabel: this.translateService.translate('modules.datasets.txtConfirm'),
            rejectLabel: this.translateService.translate('modules.datasets.txtCancel'),
            acceptButtonStyleClass: 'p-button-sm',
            acceptIcon: '-',
            rejectIcon: '-',
            accept: () => {
                this.editDataset();
            },
        });
    }

    editDataset() {
        this.isLoading = true;
        const payload: IDataset = {
            id: this.state.dataset.id,
            datasetName: this.form.controls.datasetName.value as string,
            cronPeriod: this.form.controls.cronPeriod.value as string,
            dataSource: this.form.controls.dataSource.value as string,
            isActive: this.form.controls.isActive.value as boolean,
            description: this.form.controls.description.value as string,
        };
        this.datasetsService.editDataset(payload).subscribe({
            next: (response: any) => {
                this.isLoading = false;
                this.showSuccessToast(this.translateService.translate('modules.datasets.txtSuccessfullyChanged'));
                this.router.navigate(['/datasets']);
            },
            error: error => {
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                        break;
                }
                this.isLoading = false;
            },
        });
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
                this.editDatasetConfirmation();
            } else {
                this.addDataset();
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
        this.formChangeSubscription.unsubscribe();
    }
}
