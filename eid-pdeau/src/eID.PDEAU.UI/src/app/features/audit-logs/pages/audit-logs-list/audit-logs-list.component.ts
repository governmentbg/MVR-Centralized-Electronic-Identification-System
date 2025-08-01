import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from '../../../../shared/services/toast.service';
import { PjsClientService } from '../../services/pjs-client.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';

@Component({
    selector: 'app-audit-logs-list',
    templateUrl: './audit-logs-list.component.html',
    styleUrls: ['./audit-logs-list.component.scss'],
})
export class AuditLogsListComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private pjsClientService: PjsClientService,
        private toastService: ToastService,
    ) {}

    breadcrumbItems: IBreadCrumbItems[] = [];
    logs: any[] = [];
    pageSize = 20;
    loading!: boolean;
    languageChangeSubscription: Subscription = new Subscription();
    searchAfter: string[] = [];
    loadMoreDisabled = false;
    logsFilterState: any = null;
    totalRecords = 0;
    isDialogVisible = false;
    selectedLog: any = null;
    exportCsvRunning = false;

    @HostListener('window:beforeunload', ['$event'])
    beforeUnloadHandler() {
        // returning true will navigate without confirmation
        // returning false will show a confirm dialog before navigating away
        return !this.exportCsvRunning;
    }

    ngOnInit() {
        this.breadcrumbItems = [{ label: this.translateService.translate('sidebar.txtAuditJournals') }];
        this.loadLogs();
    }

    loadLogs(logsFilterData?: any, clearLogsData = false): void {
        // When we first load the page we do not want to load any logs until a filter is applied
        if (!logsFilterData && !this.logsFilterState) {
            this.loading = false;
            return;
        }

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
        }
        if (clearLogsData) {
            this.logs = [];
        }

        this.pjsClientService.getLogsFrom(payload).subscribe({
            next: (response: any) => {
                // When scrolling the page new slices of items are added to the list
                const currentLogs = this.logs;
                const newLogs = response.data;
                this.logs = currentLogs.concat(newLogs);
                this.totalRecords = this.logs.length;

                // This marks the slice of items received from Backend
                this.searchAfter = response.searchAfter;

                if (response.data.length < this.pageSize) {
                    this.loadMoreDisabled = true;
                } else if (response.data.length === 0 || response.searchAfter.length === 0) {
                    this.loadMoreDisabled = true;
                    this.toastService.showSuccessToast(
                        this.translateService.translate('modules.logsViewer.txtTitle'),
                        this.translateService.translate('modules.logsViewer.txtNoMoreResults')
                    );
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

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    getEventTypeTranslation(key: string) {
        if (this.translateService.getTranslation(this.translateService.getActiveLang())[`logs.${key}`]) {
            return this.translateService.translate(`logs.${key}`);
        } else {
            return key;
        }
    }

    onFilteredDataChange(filteredData: any) {
        let clearLogsData = false;
        if (this.logsFilterState && filteredData === null) {
            clearLogsData = true;
        }
        this.logsFilterState = filteredData;
        this.searchAfter = [];
        this.loadMoreDisabled = false;
        this.loadLogs(filteredData, clearLogsData);
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
        this.pjsClientService.getLogsFrom(payload).subscribe({
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
