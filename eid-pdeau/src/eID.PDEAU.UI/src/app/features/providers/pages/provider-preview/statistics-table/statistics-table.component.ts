import { Component, Input } from '@angular/core';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { dateMoreThanValidate } from '@app/shared/validators/date';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from '@app/shared/services/toast.service';
import { StatisticsService } from '@app/features/providers/statistics.service';

@Component({
    selector: 'app-statistics-table',
    templateUrl: './statistics-table.component.html',
    styleUrls: ['./statistics-table.component.scss'],
})
export class StatisticsTableComponent {
    constructor(
        private translateService: TranslocoService,
        private toastService: ToastService,
        private statisticsService: StatisticsService
    ) {}

    requestsCountQuery = new RequestHandler({
        requestFunction: this.statisticsService.fetchRequestsCount,
        onError: error => {
            switch (error.status) {
                case 400:
                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                    break;
                default:
                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                    break;
            }
        },
    });
    downloadRequestsCountCsv = new RequestHandler({
        requestFunction: this.statisticsService.downloadRequestsCount,
        onSuccess: (data: string, args) => {
            const BOM = '\uFEFF';
            const blob = new Blob([BOM + data], { type: 'text/csv;charset=utf-8' });
            const downloadUrl = window.URL.createObjectURL(blob);
            const anchor = document.createElement('a');
            anchor.href = downloadUrl;
            anchor.download = `statistics.csv`;
            anchor.click();
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.providers.provider-preview.errors.file-unavailable')
            );
        },
    });

    form = new FormGroup({
        createDateFrom: new FormControl<string>('', { validators: Validators.required, nonNullable: true }),
        createDateTo: new FormControl<string>('', {
            validators: [Validators.required, dateMoreThanValidate('createDateFrom')],
            nonNullable: true,
        }),
    });
    currentPeriod = {
        createDateFrom: '',
        createDateTo: '',
    };

    onSubmit() {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.form.markAsPristine();
            this.currentPeriod.createDateFrom = this.form.controls.createDateFrom.value;
            this.currentPeriod.createDateTo = this.form.controls.createDateTo.value;
            this.requestsCountQuery.execute({
                createDateFrom: new Date(this.form.controls.createDateFrom.value).toISOString(),
                createDateTo: new Date(this.form.controls.createDateTo.value).toISOString()
            });
        }
    }

    validateToDate() {
        this.form.controls.createDateTo.updateValueAndValidity();
    }

    download(data: any) {
        this.downloadRequestsCountCsv.execute({
            createDateFrom: new Date(this.form.controls.createDateFrom.value).toISOString(),
            createDateTo: new Date(this.form.controls.createDateTo.value).toISOString()
        });
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }
}
