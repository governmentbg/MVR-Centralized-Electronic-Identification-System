import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { IHidePaymentHistoryEventEmitter, IPaymentHistory } from '../../interfaces/payment-history';
import { PaymentStatuses } from '../../enums/statuses';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import * as moment from 'moment';
import { ApplicationTypes } from 'src/app/features/eid-management/enums/eid-management';
import { PaymentAmounts } from '../../enums/payment-amounts';

@Component({
    selector: 'app-payment-history-filter',
    templateUrl: './payment-history-filter.component.html',
    styleUrls: ['./payment-history-filter.component.scss'],
})
export class PaymentHistoryFilterComponent {
    @Output() filteredData: EventEmitter<IHidePaymentHistoryEventEmitter> =
        new EventEmitter<IHidePaymentHistoryEventEmitter>();
    @Input() paymentHistoryList!: IPaymentHistory[];
    visible = false;
    filterForm: FormGroup;
    filterFormUnSavedState: any;
    chosenFilters: any[] = [];
    statuses = [
        {
            name: this.translateService.translate('paymentHistory.statuses.txtCanceled'),
            code: PaymentStatuses.Canceled,
        },
        {
            name: this.translateService.translate('paymentHistory.statuses.txtPaid'),
            code: PaymentStatuses.Paid,
        },
        {
            name: this.translateService.translate('paymentHistory.statuses.txtPending'),
            code: PaymentStatuses.Pending,
        },
        {
            name: this.translateService.translate('paymentHistory.statuses.txtTimedOut'),
            code: PaymentStatuses.TimedOut,
        },
    ];
    reasonTypes = [
        {
            name: this.translateService.translate('modules.eidManagement.applicationTypes.txtStopEID'),
            code: ApplicationTypes.STOP_EID,
        },
        {
            name: this.translateService.translate('modules.eidManagement.applicationTypes.txtRevokeEID'),
            code: ApplicationTypes.REVOKE_EID,
        },
        {
            name: this.translateService.translate('modules.eidManagement.applicationTypes.txtResumeEID'),
            code: ApplicationTypes.RESUME_EID,
        },
        {
            name: this.translateService.translate('modules.eidManagement.applicationTypes.txtIssueEID'),
            code: ApplicationTypes.ISSUE_EID,
        },
    ];
    amounts = [
        {
            name: this.translateService.translate('paymentHistory.amountLabels.txtBelow'),
            range: PaymentAmounts.BELOW_25,
        },
        {
            name: this.translateService.translate('paymentHistory.amountLabels.txtBetween'),
            range: PaymentAmounts.BETWEEN_25_50,
        },
        { name: this.translateService.translate('paymentHistory.amountLabels.txtOver'), range: PaymentAmounts.OVER_50 },
    ];

    constructor(
        private formBuilder: FormBuilder,
        private translateService: TranslocoService,
        private translocoLocaleService: TranslocoLocaleService
    ) {
        this.filterForm = this.formBuilder.group({
            ePaymentId: [null],
            status: [null],
            reason: [null],
            amount: [null],
            createdOn: [null],
            paymentDeadline: [null],
            paymentDate: [null],
        });
    }

    onSubmit(): void {
        const filters = this.filterForm.value;
        this.applyFilters(filters);
    }

    applyFilters(filters: any): void {
        let filteredData = [...this.paymentHistoryList];
        if (filters.ePaymentId) {
            filteredData = filteredData.filter(item => item.ePaymentId.includes(filters.ePaymentId));
        }
        if (filters.createdOn) {
            const selectedDate = moment(filters.createdOn).format('YYYY-MM-DD');
            filteredData = filteredData.filter(item => item.createdOn.startsWith(selectedDate));
        }
        if (filters.paymentDate) {
            const selectedDate = moment(filters.paymentDate).format('YYYY-MM-DD');
            filteredData = filteredData.filter(item => item.paymentDate && item.paymentDate.startsWith(selectedDate));
        }
        if (filters.paymentDeadline) {
            const selectedDate = moment(filters.paymentDeadline).format('YYYY-MM-DD');
            filteredData = filteredData.filter(item => moment(item.paymentDeadline).isBefore(selectedDate, 'day'));
        }
        if (filters.status) {
            filteredData = filteredData.filter(item => item.status === filters.status);
        }
        if (filters.amount) {
            filteredData = this.filterByAmount(filteredData, filters.amount);
        }
        if (filters.reason) {
            filteredData = filteredData.filter(item => item.reason === filters.reason);
        }

        this.filteredData.emit({ paymentHistoryEvent: filteredData, refreshTable: false });
        this.setFilteredValues();
    }

    clearAllFilteredValues(): void {
        this.filterForm.reset();
        this.chosenFilters = [];
        this.filteredData.emit({ paymentHistoryEvent: this.paymentHistoryList, refreshTable: false });
    }

    openFilter() {
        this.visible = true;
        this.filterFormUnSavedState = this.filterForm.value;
    }

    closeFilter(): void {
        this.visible = false;
        this.filterForm.reset(this.filterFormUnSavedState);
    }

    clearFilteredValue(filteredValue: any): void {
        this.filterForm.get(filteredValue.key)?.reset();
        this.chosenFilters = this.chosenFilters.filter(filter => filter.key !== filteredValue.key);
        this.onSubmit();
    }

    setFilteredValues() {
        // If chosenFilters is empty, populate it with filters
        if (this.chosenFilters.length === 0) {
            this.createFilterValues();
        } else {
            Object.entries(this.filterForm.value).forEach((formValue: any) => {
                // Check if the filter is preselected
                const chosenFilter = this.chosenFilters.find(filter => filter.key === formValue[0]);
                if (chosenFilter) {
                    // Update the value
                    chosenFilter.value = formValue[1];

                    // If current value is null | empty string | boolean false, then remove it from the chosenFilters array
                    if (!this.performDataTypeCheck(formValue[1])) {
                        this.chosenFilters.splice(this.chosenFilters.indexOf(chosenFilter), 1);
                    }
                } else if (this.performDataTypeCheck(formValue[1])) {
                    // If current filter is not preselected in the chosenFilters array but its value is not null it should be added
                    // We don't want to add empty applicationUids to the chosenFilters
                    const preparedObject = {
                        key: formValue[0],
                        value: formValue[1],
                        label: `paymentHistory.tableFilter.txt${formValue[0][0].toUpperCase()}${formValue[0].substring(
                            1
                        )}`,
                    };
                    this.chosenFilters.push(preparedObject);
                }
            });
        }
    }

    performDataTypeCheck(value: null | any[] | string | boolean | Date): boolean {
        return (
            (value !== null && typeof value === 'string' && value.length > 0) ||
            (value !== null && typeof value === 'string' && value.length > 0) ||
            (Array.isArray(value) && value['length'] > 1) ||
            (Array.isArray(value) && value['length'] === 1 && value[0] !== '' && value[0] !== null) ||
            (typeof value === 'object' && Object.prototype.toString.call(value) === '[object Date]') ||
            (typeof value === 'boolean' && value)
        );
    }

    createFilterValues() {
        Object.entries(this.filterForm.value).forEach((formValue: any) => {
            // Check if value is string|array|boolean|date|number and not null
            if (formValue[1] !== null) {
                // We don't want to add empty applicationUids to the chosenFilters
                const preparedObject = {
                    key: formValue[0],
                    value: formValue[1],
                    label: `paymentHistory.tableFilter.txt${formValue[0][0].toUpperCase()}${formValue[0].substring(1)}`,
                };
                this.chosenFilters.push(preparedObject);
            }
        });
    }

    prepareFilteredValueLabel(filteredValue: any): string {
        if (['createdOn', 'paymentDeadline', 'paymentDate'].includes(filteredValue.key)) {
            const formattedDate = this.formatDate(filteredValue.value);
            return (
                this.translateService.translate(filteredValue.label) +
                ': ' +
                this.translocoLocaleService.localizeDate(formattedDate, this.translocoLocaleService.getLocale())
            );
        } else if (filteredValue.key === 'reason') {
            return (
                this.translateService.translate(filteredValue.label) +
                ': ' +
                this.translatePaymentType(filteredValue.value)
            );
        } else {
            return this.translateService.translate(filteredValue.label);
        }
    }

    refreshData() {
        this.clearAllFilteredValues();
        this.filteredData.emit({ paymentHistoryEvent: this.paymentHistoryList, refreshTable: true });
    }

    formatDate(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
    }

    filterByAmount(data: IPaymentHistory[], amountFilter: string): IPaymentHistory[] {
        switch (amountFilter) {
            case 'below-25':
                return data.filter(item => item.amount < 25);
            case '25-50':
                return data.filter(item => item.amount > 25 && item.amount <= 50);
            case 'over-50':
                return data.filter(item => item.amount > 50);
            default:
                return data;
        }
    }

    translatePaymentType(appType: ApplicationTypes | null) {
        switch (appType) {
            case ApplicationTypes.ISSUE_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtIssueEID');
            case ApplicationTypes.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtResumeEID');
            case ApplicationTypes.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtRevokeEID');
            case ApplicationTypes.STOP_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtStopEID');
            default:
                return appType;
        }
    }
}
