import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from '../../../../shared/services/toast.service';
import { PjsClientService } from '../../services/pjs-client.service';
import { IBreadCrumbItems } from '../../../eid-management/interfaces/breadcrumb.interfaces';
import { AppConfigService } from 'src/app/core/services/config.service';

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
        private appConfigService: AppConfigService
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
    MAX_DAYS_DIFF = this.appConfigService.config.exportCsvMaxPeriodInDays;

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
        if (
            this.logsFilterState === null ||
            this.logsFilterState === undefined ||
            !this.logsFilterState.startDate ||
            !this.logsFilterState.endDate
        ) {
            this.showErrorToast(
                this.translateService.translate('modules.logsViewer.txtExportDataFilterError', {
                    value: this.MAX_DAYS_DIFF,
                })
            );
            return;
        }

        const startDate = new Date(this.logsFilterState.startDate);
        const endDate = new Date(this.logsFilterState.endDate);
        const diffInMs = endDate.getTime() - startDate.getTime();
        const diffInDays = diffInMs / (1000 * 60 * 60 * 24);

        if (startDate > endDate || diffInDays >= this.MAX_DAYS_DIFF) {
            this.showErrorToast(
                this.translateService.translate('modules.logsViewer.txtExportDataFilterError', {
                    value: this.MAX_DAYS_DIFF,
                })
            );
            return;
        }

        this.exportCsvRunning = true;
        const payload = {
            // TODO: Като е готов backend-а да оправя payload-а
            ...this.logsFilterState,
        };
        this.pjsClientService.exportLogs(payload).subscribe({
            next: (response: any) => {
                const startDateStr = this.formatDateForFilename(startDate);
                const endDateStr = this.formatDateForFilename(endDate);

                const filename = `report-${startDateStr}_${endDateStr}.csv`;
                switch (response.status) {
                    case 202:
                        // Polling
                        setTimeout(() => {
                            this.onExportData(eventData);
                        }, 5000);
                        break;
                    case 200:
                        this.downloadFile(response.body, filename);
                        this.exportCsvRunning = false;
                        break;

                    default:
                        this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                        this.exportCsvRunning = false;
                        break;
                }
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

    formatDateForFilename(date: Date): string {
        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');

        return `${year}-${month}-${day}`;
    }

    downloadFile(fileData: Blob, fileName = 'report.csv'): void {
        const blob = new Blob([fileData], { type: 'text/csv;charset=utf-8;' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.setAttribute('download', fileName);
        a.style.display = 'none';

        document.body.appendChild(a);
        a.click();

        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    showLogDetails(log: any) {
        this.selectedLog = JSON.stringify(log, null, 4);
        this.isDialogVisible = true;
    }
}
