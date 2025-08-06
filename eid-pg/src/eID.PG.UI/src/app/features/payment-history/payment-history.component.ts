import { Component, OnInit, ViewChild } from '@angular/core';
import { EidManagementService } from '../eid-management/services/eid-management.service';
import { TranslocoService } from '@ngneat/transloco';
import { MessageService } from 'primeng/api';
import { first } from 'rxjs';
import { IHidePaymentHistoryEventEmitter, IPaymentHistory } from './interfaces/payment-history';
import { ApplicationTypes } from '../eid-management/enums/eid-management';
import { Table } from 'primeng/table';

@Component({
    selector: 'app-payment-history',
    templateUrl: './payment-history.component.html',
    styleUrls: ['./payment-history.component.scss'],
})
export class PaymentHistoryComponent implements OnInit {
    @ViewChild('dt') table!: Table;
    constructor(
        private eidManagementService: EidManagementService,
        private translocoService: TranslocoService,
        private messageService: MessageService
    ) {}
    requestInProgress = false;
    paymentHistory: IPaymentHistory[] = [];
    filteredPaymentHistory: IPaymentHistory[] = [];
    pageSize = 10;
    sortField: keyof IPaymentHistory = 'createdOn';
    sortOrder = 1;
    ngOnInit(): void {
        this.getPaymentHistory();
    }

    getPaymentHistory(): void {
        this.requestInProgress = true;
        this.eidManagementService
            .getPaymentHistory()
            .pipe(first())
            .subscribe({
                next: (data: IPaymentHistory[]) => {
                    this.paymentHistory = data;
                    this.filteredPaymentHistory = [...data];
                    this.requestInProgress = false;
                },
                error: () => {
                    this.messageService.add({
                        severity: 'error',
                        summary: this.translocoService.translate('global.txtErrorTitle'),
                        detail: this.translocoService.translate('global.txtNoRecordsFound'),
                    });
                    this.requestInProgress = false;
                },
            });
    }

    computedStatus(status: any) {
        return this.translocoService.translate('paymentHistory.statuses.txt' + status);
    }

    translatePaymentType(appType: ApplicationTypes | null) {
        switch (appType) {
            case ApplicationTypes.ISSUE_EID:
                return this.translocoService.translate('modules.eidManagement.applicationTypes.txtIssueEID');
            case ApplicationTypes.RESUME_EID:
                return this.translocoService.translate('modules.eidManagement.applicationTypes.txtResumeEID');
            case ApplicationTypes.REVOKE_EID:
                return this.translocoService.translate('modules.eidManagement.applicationTypes.txtRevokeEID');
            case ApplicationTypes.STOP_EID:
                return this.translocoService.translate('modules.eidManagement.applicationTypes.txtStopEID');
            default:
                return appType;
        }
    }

    onSort(event: { field: keyof IPaymentHistory; order: number }) {
        this.sortField = event.field;
        this.sortOrder = event.order;

        this.filteredPaymentHistory.sort((a, b) => {
            const valueA = a[this.sortField];
            const valueB = b[this.sortField];

            if (valueA == null || valueB == null) {
                return 0;
            }

            if (typeof valueA === 'string' && typeof valueB === 'string') {
                return this.sortOrder * valueA.localeCompare(valueB);
            }

            if (typeof valueA === 'number' && typeof valueB === 'number') {
                return this.sortOrder * (valueA - valueB);
            }

            return 0;
        });
    }

    onFilteredDataChange(filteredData: IHidePaymentHistoryEventEmitter): void {
        if (filteredData.refreshTable) {
            this.getPaymentHistory();
        } else {
            this.filteredPaymentHistory = filteredData.paymentHistoryEvent;
            if (this.table?.first > 0) {
                this.table.first = 0;
            }
        }
    }
}
