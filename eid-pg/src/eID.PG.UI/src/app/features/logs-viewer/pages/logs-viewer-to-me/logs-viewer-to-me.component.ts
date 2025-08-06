import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { ToastService } from 'src/app/shared/services/toast.service';
import * as moment from 'moment/moment';
import { LogsService } from '../../services/logs.service';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';

@Component({
    selector: 'app-logs-viewer-to-me',
    templateUrl: './logs-viewer-to-me.component.html',
    styleUrls: ['./logs-viewer-to-me.component.scss'],
})
export class LogsViewerToMeComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private toastService: ToastService,
        private logsService: LogsService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                { label: this.translateService.translate('modules.logsViewer.txtTitle'), routerLink: '/logs-viewer' },
            ];
        });
    }

    breadcrumbItems: IBreadCrumbItems[] = [];
    logs: any[] = [];
    pageSize = 20;
    loading = false;
    languageChangeSubscription: Subscription = new Subscription();
    moment = moment;
    searchAfter: string[] = [];
    infiniteScrollDisabled = false;
    logsFilterState: any = {};
    exportCsvRunning = false;

    @HostListener('window:beforeunload', ['$event'])
    beforeUnloadHandler() {
        // returning true will navigate without confirmation
        // returning false will show a confirm dialog before navigating away
        return !this.exportCsvRunning;
    }

    ngOnInit() {
        this.breadcrumbItems = [
            { label: this.translateService.translate('modules.logsViewer.txtTitle'), routerLink: '/logs-viewer' },
        ];
        this.loadLogs();
    }

    loadLogs(logsFilterData?: any): void {
        this.loading = true;
        let payload = {
            cursorSize: this.pageSize,
            cursorSearchAfter: this.searchAfter,
        };
        if (logsFilterData !== null && logsFilterData !== undefined) {
            payload = { ...payload, ...logsFilterData };
            this.logs = [];
        } else if (this.logsFilterState !== null && Object.keys(this.logsFilterState).length > 0) {
            payload = { ...payload, ...this.logsFilterState };
            this.logs = [];
        }
        this.logsService.getLogsTo(payload).subscribe({
            next: (response: any) => {
                // When scrolling the page new slices of items are added to the list
                const currentLogs = this.logs;
                const newLogs = response.data;
                this.logs = currentLogs.concat(newLogs);

                // This marks the slice of items received from Backend
                this.searchAfter = response.searchAfter;

                if (response.data.length === 0 || response.searchAfter.length === 0) {
                    this.infiniteScrollDisabled = true;
                }
                this.loading = false;
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
                this.loading = false;
            },
        });
    }

    getEventTypeTranslation(key: string) {
        if (this.translateService.getTranslation(this.translateService.getActiveLang())[`logs.${key}`]) {
            return this.translateService.translate(`logs.${key}`);
        } else {
            return key;
        }
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    onFilteredDataChange(filteredData: any) {
        this.logsFilterState = filteredData;
        this.searchAfter = [];
        this.loadLogs(filteredData);
    }

    onExportData(eventData: any) {
        const pageSize = 1000;
        if (
            this.logsFilterState === null ||
            this.logsFilterState === undefined ||
            Object.keys(this.logsFilterState).length === 0
        ) {
            this.showErrorToast(
                this.translateService.translate('modules.logsViewer.txtExportDataMissingFilterError', {
                    value: pageSize,
                })
            );
            return;
        }

        this.exportCsvRunning = true;
        const payload = {
            cursorSize: pageSize,
            cursorSearchAfter: [],
            ...this.logsFilterState,
        };
        this.logsService.getLogsTo(payload).subscribe({
            next: (response: any) => {
                this.downloadCSV(this.convertToCSV(response.data));
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
                this.exportCsvRunning = false;
            },
        });
    }

    private convertToCSV(dataArray: any) {
        if (!dataArray || !dataArray.length) {
            console.error('Empty data array!');
            return '';
        }
        const headers = Object.keys(dataArray[0]);
        const csvRows = [];

        csvRows.push(headers.join(','));

        dataArray.forEach((obj: any) => {
            const row = headers.map(header => obj[header]);
            csvRows.push(row.join(','));
        });

        return csvRows.join('\n');
    }

    private downloadCSV(rawCsv: string, filename = 'data.csv') {
        const blob = new Blob([rawCsv], { type: 'text/csv;charset=utf-8;' });

        const link = document.createElement('a');
        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
        this.exportCsvRunning = false;
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }
}
