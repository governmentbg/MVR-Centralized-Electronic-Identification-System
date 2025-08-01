import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { Subscription } from 'rxjs';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { dateMoreThanOrEqualValidate, dateMoreThanValidate } from '../../../../shared/validators/date';

@Component({
    selector: 'app-reports-filter',
    templateUrl: './reports-filter.component.html',
    styleUrls: ['./reports-filter.component.scss'],
})
export class ReportsFilterComponent implements OnInit, OnDestroy {
    constructor(public translateService: TranslocoService, private translocoLocaleService: TranslocoLocaleService) {}

    @Output() filteredData: EventEmitter<any> = new EventEmitter();
    @Output() exportDataEvent: EventEmitter<any> = new EventEmitter();
    @Input() disableExportBtn = false;

    submitted = false;
    visible = false;
    chosenFilters: any[] = [];
    form = {
        from: new FormControl('', { validators: Validators.required, nonNullable: true }),
        to: new FormControl('', {
            validators: [Validators.required, dateMoreThanValidate('from')],
            nonNullable: true,
        }),
        operators: new FormControl<string[]>([], { validators: Validators.required, nonNullable: true }),
    };
    filterForm = new FormGroup(this.form);
    filterFormUnSavedState: any;
    subscriptions: Subscription[] = [];

    ngOnInit() {
        const formChangeSubscription = this.filterForm.valueChanges.subscribe({
            next: () => {
                const endDateControl = this.filterForm.controls['to'];
                const startDateValue = this.filterForm.controls['from']?.value;
                if (startDateValue) {
                    endDateControl?.setValidators([dateMoreThanOrEqualValidate('from')]);
                } else {
                    endDateControl?.clearValidators();
                }
                endDateControl?.updateValueAndValidity({ emitEvent: false });
            },
        });

        this.subscriptions.push(formChangeSubscription);
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    validateToDate() {
        this.filterForm.controls.to.updateValueAndValidity();
    }

    openFilter() {
        this.visible = true;
        // Save state
        this.filterFormUnSavedState = this.filterForm.value;
    }

    closeFilter() {
        this.visible = false;
        this.filterForm.setValue(this.filterFormUnSavedState);
    }

    onSubmit() {
        this.filterForm.markAllAsTouched();
        this.submitted = true;
        if (this.filterForm.valid) {
            this.setFilteredValues();
            this.filteredData.emit(this.extractValues());
            this.visible = false;
            this.submitted = false;
            this.filterForm.markAsPristine();
        }
    }

    createFilterValues() {
        Object.entries(this.filterForm.value).forEach((formValue: any) => {
            // Check if value is string|array|boolean|date|number and not null
            if (this.performDataTypeCheck(formValue[1])) {
                const preparedObject = {
                    key: formValue[0],
                    value: formValue[1],
                    label: `modules.logsViewer.filterLabels.${formValue[0]}`,
                };
                this.chosenFilters.push(preparedObject);
            }
        });
    }

    setFilteredValues() {
        // If chosenFilters is empty, populate it with filters
        if (this.chosenFilters.length === 0) {
            this.createFilterValues();
            // If user edit the filter and we already have populated filters
        } else {
            Object.entries(this.filterForm.value).forEach((formValue: any) => {
                this.chosenFilters.filter((chosenFilter, index, chosenFilters) => {
                    // If filter is preselected
                    if (formValue[0] === chosenFilter.key) {
                        // Update the value
                        chosenFilter.value = formValue[1];

                        // If current value is null | empty string | boolean false, then remove it from the chosenFilters array
                        if (!this.performDataTypeCheck(formValue[1])) {
                            chosenFilters.splice(index, 1);
                        }
                    } else if (
                        this.performDataTypeCheck(formValue[1]) &&
                        !this.chosenFilters.find(fV => fV.key === formValue[0])
                    ) {
                        const preparedObject = {
                            key: formValue[0],
                            value: formValue[1],
                            label: `modules.logsViewer.filterLabels.${formValue[0]}`,
                        };
                        chosenFilters.push(preparedObject);
                    }
                });
            });
        }
    }

    // Check if value is not null but 'number | string | array | boolean | Date'
    performDataTypeCheck(value: null | any[] | string | boolean | Date): boolean {
        return (
            (value !== null && typeof value === 'number' && value > -1) ||
            (value !== null && typeof value === 'string' && value.length > 0) ||
            (Array.isArray(value) && value['length'] > 1) ||
            (Array.isArray(value) && value['length'] === 1 && value[0] !== '' && value[0] !== null) ||
            (typeof value === 'boolean' && value) ||
            (typeof value === 'object' && Object.prototype.toString.call(value) === '[object Date]')
        );
    }

    clearFilteredValue(filteredValue: any) {
        (this.filterForm.controls as any)[filteredValue.key].setValue(null);
        this.filterForm.markAllAsTouched();
        this.chosenFilters = this.chosenFilters.filter(fV => fV.key !== filteredValue.key);
        this.filteredData.emit(this.extractValues());
    }

    clearAllFilteredValues() {
        this.chosenFilters = [];
        this.filterForm.reset();
        this.filterFormUnSavedState = this.filterForm.value;
        this.filteredData.emit({});
    }

    refreshData() {
        this.filteredData.emit(this.extractValues());
    }

    exportData() {
        this.exportDataEvent.emit(this.extractValues());
    }

    prepareFilteredValueLabel(filteredValue: any): string {
        if (filteredValue.key === 'startDate' || filteredValue.key === 'endDate') {
            return (
                this.translateService.translate(filteredValue.label) +
                ': ' +
                this.translocoLocaleService.localizeDate(filteredValue.value, this.translocoLocaleService.getLocale())
            );
        } else {
            return this.translateService.translate(filteredValue.label);
        }
    }

    extractValues(): any {
        const extractedValues: any = Object.fromEntries(
            Object.entries(this.filterForm.value).filter(([, v]: any) => this.performDataTypeCheck(v))
        );

        if (extractedValues && extractedValues.startDate) {
            const startDateLocal = new Date(extractedValues.startDate);
            extractedValues.startDate = startDateLocal.toISOString();
        }

        if (extractedValues && extractedValues.endDate) {
            const endDateLocal = new Date(extractedValues.endDate);
            extractedValues.endDate = endDateLocal.toISOString();
        }

        return extractedValues;
    }
}
