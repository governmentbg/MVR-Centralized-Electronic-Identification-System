import { Component, OnInit } from '@angular/core';
import { DatasetsService } from '../../services/datasets.service';
import { ConfirmationService } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from 'src/app/shared/services/toast.service';
import { IDataset } from '../../interfaces/IDatasets';
import { Router } from '@angular/router';
import { CronPeriods } from '../../enums/cron-enum';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';

@Component({
    selector: 'app-homepage',
    templateUrl: './datasets.component.html',
    styleUrls: ['./datasets.component.scss'],
    providers: [ConfirmationService],
})
export class DatasetsComponent implements OnInit {
    constructor(
        private datasetsService: DatasetsService,
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private toastService: ToastService,
        private router: Router,
        private translocoLocaleService: TranslocoLocaleService
    ) {}

    isLoading = false;
    datasets: any = [];
    isConfirm = true;

    ngOnInit() {
        this.getAllDatasets();
    }

    getAllDatasets() {
        this.isLoading = true;
        this.datasetsService.getAllDatasets().subscribe({
            next: (response: any) => {
                this.datasets = response;
                this.isLoading = false;
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

    deleteDatasetConfirmation(dataset: any) {
        this.isConfirm = false;
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger p-button-sm',
            header: this.translateService.translate('modules.datasets.txtDeletion'),
            message: this.translateService.translate('modules.datasets.txtDeletionMainText'),
            acceptLabel: this.translateService.translate('modules.datasets.txtDeleteIt'),
            rejectLabel: this.translateService.translate('modules.datasets.txtCancel'),
            acceptButtonStyleClass: 'p-button-sm',
            acceptIcon: '-',
            rejectIcon: '-',
            accept: () => {
                this.deleteDataset(dataset);
            },
        });
    }

    deleteDataset(dataset: IDataset) {
        this.isLoading = true;
        this.datasetsService.deleteDataset(dataset.id).subscribe({
            next: (response: any) => {
                this.isLoading = false;
                this.showSuccessToast(this.translateService.translate('modules.datasets.txtSuccessfullyDeleted'));
                this.getAllDatasets();
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

    uploadDatasetConfirmation(dataset: any) {
        this.isConfirm = true;
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger p-button-sm',
            header: this.translateService.translate('modules.datasets.txtConfirmation'),
            message: this.translateService.translate('modules.datasets.txtConfirmationMainText'),
            acceptLabel: this.translateService.translate('modules.datasets.txtConfirm'),
            rejectLabel: this.translateService.translate('modules.datasets.txtCancel'),
            acceptButtonStyleClass: 'p-button-sm',
            acceptIcon: '-',
            rejectIcon: '-',
            accept: () => {
                this.uploadDataset(dataset);
            },
        });
    }

    uploadDataset(dataset: IDataset) {
        this.isLoading = true;
        this.datasetsService.uploadDataset(dataset.id).subscribe({
            next: (response: any) => {
                this.isLoading = false;
                this.showSuccessToast(this.translateService.translate('modules.datasets.txtSuccessfullyUpload'));
                this.getAllDatasets();
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

    navigateToEdit(dataset: IDataset) {
        this.router.navigate(['/datasets/form'], {
            state: { dataset: dataset, isEditMode: true },
        });
    }

    showDetails(dataset: IDataset) {
        this.router.navigate(['/datasets/form'], {
            state: { dataset: dataset, isPreviewMode: true },
        });
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    formatDate(date: string | null): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
            timeStyle: 'medium',
        });
    }

    computeCronPeriod(datasetCronPeriod: CronPeriods) {
        switch (datasetCronPeriod) {
            case CronPeriods.Weekly:
                return this.translateService.translate('modules.datasets.txtWeekly');
            case CronPeriods.Monthly:
                return this.translateService.translate('modules.datasets.txtMonthly');
            case CronPeriods.Yearly:
                return this.translateService.translate('modules.datasets.txtYearly');
            default:
                return '';
        }
    }

    computeIsActive(isActive: boolean) {
        switch (isActive) {
            case true:
                return this.translateService.translate('modules.datasets.txtYes');
            case false:
                return this.translateService.translate('modules.datasets.txtNo');
            default:
                return '';
        }
    }
}
