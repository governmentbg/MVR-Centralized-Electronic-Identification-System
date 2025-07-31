import { Component, EventEmitter, Output, OnDestroy } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { ApplicationStatus, ApplicationType } from '@app/features/centers/enums/centers.enum';
import { statuses } from '@app/features/centers/components/application-status/application-status.component';

export interface ApplicationFilter {
    status?: ApplicationStatus | null;
    eidName?: string | null;
    applicationNumber?: string | null;
    applicationType?: string | null;
}

@Component({
    selector: 'app-applications-filter',
    templateUrl: './applications-filter.component.html',
    styleUrls: ['./applications-filter.component.scss'],
})
export class ApplicationsFilterComponent implements OnDestroy {
    constructor(public translateService: TranslocoService) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.statuses = Object.entries(statuses).map(([key, { translationKey }]) => {
                return {
                    name: this.translateService.translate(
                        `modules.centers.applications.statuses.${translationKey}`
                    ),
                    code: key,
                };
            });
            this.applicationTypes = [
                {
                    id: ApplicationType.REGISTER,
                    name: this.translateService.translate(
                        `modules.centers.applications.types.${ApplicationType.REGISTER}`
                    ),
                },
                {
                    id: ApplicationType.RESUME,
                    name: this.translateService.translate(
                        `modules.centers.applications.types.${ApplicationType.RESUME}`
                    ),
                },
                {
                    id: ApplicationType.STOP,
                    name: this.translateService.translate(
                        `modules.centers.applications.types.${ApplicationType.STOP}`
                    ),
                },
                {
                    id: ApplicationType.REVOKE,
                    name: this.translateService.translate(
                        `modules.centers.applications.types.${ApplicationType.REVOKE}`
                    ),
                },
            ];
        });

        this.subscriptions.add(languageChangeSubscription);
    }

    subscriptions: Subscription = new Subscription();

    @Output() filteredData: EventEmitter<ApplicationFilter> = new EventEmitter();

    statuses: { name: string; code: string }[] = [];

    visible = false;
    chosenFilters: any[] = [];
    filterFormUnSavedState: any;
    filterForm = new FormGroup({
        status: new FormControl<ApplicationStatus | null>(null),
        eidName: new FormControl<string | null>(null),
        applicationNumber: new FormControl<string | null>(null),
        applicationType: new FormControl<string | null>(null),
    });
    applicationTypes: any = [];

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    openFilter() {
        this.visible = true;
        // Save state
        this.filterFormUnSavedState = this.filterForm.getRawValue();
    }

    closeFilter() {
        this.visible = false;
        // Use state
        this.filterForm.patchValue(this.filterFormUnSavedState);
    }

    onSubmit() {
        this.filterForm.markAllAsTouched();
        Object.values(this.filterForm.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    innerControl.markAsDirty();
                });
            } else {
                control.markAsDirty();
            }
        });
        if (this.filterForm.valid) {
            this.setFilteredValues();
            this.filteredData.emit(this.extractValues());
            this.visible = false;
            this.filterForm.markAsPristine();
        }
    }

    createFilterValues() {
        Object.entries(this.filterForm.getRawValue()).forEach((formValue: any) => {
            // Check if value is string|array|boolean|date|number and not null
            if (this.performDataTypeCheck(formValue[1])) {
                const preparedObject = {
                    key: formValue[0],
                    value: formValue[1],
                    label: `modules.centers.applications.filter.${formValue[0]}`,
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
            Object.entries(this.filterForm.getRawValue()).forEach((formValue: any) => {
                this.chosenFilters.filter((chosenFilter, index, chosenFilters) => {
                    // If filter is preselected
                    if (formValue[0] === chosenFilter.key) {
                        // Update the value
                        chosenFilter.value = formValue[1];

                        // If current value is null | empty string | boolean false, then remove it from the chosenFilters array
                        if (!this.performDataTypeCheck(formValue[1])) {
                            chosenFilters.splice(index, 1);
                        }

                        // If current filter is not preselected in the chosenFilters array but its value is not null it should be added
                    } else if (
                        this.performDataTypeCheck(formValue[1]) &&
                        !this.chosenFilters.find(fV => fV.key === formValue[0])
                    ) {
                        const preparedObject = {
                            key: formValue[0],
                            value: formValue[1],
                            label: `modules.centers.applications.filter.${formValue[0]}`,
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
        this.chosenFilters = this.chosenFilters.filter(fV => fV.key !== filteredValue.key);

        this.filterForm.markAllAsTouched();
        Object.values(this.filterForm.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    innerControl.markAsDirty();
                });
            } else {
                control.markAsDirty();
            }
        });
        this.filteredData.emit(this.extractValues());
    }

    clearAllFilteredValues() {
        this.chosenFilters = [];
        this.filterForm.reset();
        this.setFilteredValues();
        this.filterFormUnSavedState = this.filterForm.getRawValue();
        this.filteredData.emit(this.extractValues());
    }

    prepareFilteredValueLabel(filteredValue: any): string {
        return this.translateService.translate(filteredValue.label);
    }

    extractValues(): any {
        const extractedValues: any = Object.fromEntries(
            Object.entries(this.filterForm.getRawValue()).filter(([, v]: any) => this.performDataTypeCheck(v))
        );

        return extractedValues;
    }
}
