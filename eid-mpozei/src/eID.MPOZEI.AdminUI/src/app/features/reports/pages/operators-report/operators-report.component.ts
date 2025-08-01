import { Component } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from '../../../../shared/services/toast.service';
import { ReportsClientService } from '../../reports-client.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';

@Component({
    selector: 'app-operators-report',
    templateUrl: './operators-report.component.html',
    styleUrls: ['./operators-report.component.scss'],
})
export class OperatorsReportComponent {
    constructor(
        private translateService: TranslocoService,
        private toastService: ToastService,
        private reportsClientService: ReportsClientService,
        private translocoLocaleService: TranslocoLocaleService
    ) {}
    report: any = [];
    reportsFilterState: any = null;
    loading = false;

    onFilterChange(filterValue: any) {
        this.loading = true;
        this.reportsFilterState = filterValue;
        this.reportsClientService
            .getReportByOperators({
                from: this.translocoLocaleService.localizeDate(
                    filterValue.from,
                    this.translocoLocaleService.getLocale()
                ),
                to: this.translocoLocaleService.localizeDate(filterValue.to, this.translocoLocaleService.getLocale()),
                operators: filterValue.operators,
            })
            .subscribe({
                next: data => {
                    this.loading = false;
                    this.report = data;
                },
                error: () => {
                    this.loading = false;
                    this.toastService.showErrorToast(
                        this.translateService.translate('global.txtErrorTitle'),
                        this.translateService.translate('global.txtUnexpectedError')
                    );
                },
            });
    }

    onExport(filterValue: any) {
        this.loading = true;
        this.reportsClientService
            .getReportByOperatorsCSV({
                from: this.translocoLocaleService.localizeDate(
                    filterValue.from,
                    this.translocoLocaleService.getLocale()
                ),
                to: this.translocoLocaleService.localizeDate(filterValue.to, this.translocoLocaleService.getLocale()),
                operators: filterValue.operators,
            })
            .subscribe({
                next: data => {
                    this.loading = false;
                    const BOM = '\uFEFF';
                    const blob = new Blob([BOM + data], { type: 'text/csv;charset=utf-8' });
                    const downloadUrl = window.URL.createObjectURL(blob);
                    const anchor = document.createElement('a');
                    anchor.href = downloadUrl;
                    anchor.download = `statistics.csv`;
                    anchor.click();
                },
                error: () => {
                    this.loading = false;
                    this.toastService.showErrorToast(
                        this.translateService.translate('global.txtErrorTitle'),
                        this.translateService.translate('global.txtUnexpectedError')
                    );
                },
            });
    }
}
