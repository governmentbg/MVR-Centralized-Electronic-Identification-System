import { Component, EventEmitter, Output, OnDestroy } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { GenerateFormFilters } from '@app/shared/types';
import { ProviderServiceType } from '@app/features/services/services.dto';
export type ServicesFilterT = GenerateFormFilters<ProviderServiceType, 'name' | 'serviceNumber'> & {
    includeApprovedOnly?: boolean;
    includeInactive?: boolean;
    includeEmpowermentOnly?: boolean;
    includeDeleted?: boolean;
    includeWithoutScope?: boolean;
};

@Component({
    selector: 'app-services-filter',
    templateUrl: './services-filter.component.html',
    styleUrls: ['./services-filter.component.scss'],
})
export class ServicesFilterComponent implements OnDestroy {
    constructor(public translateService: TranslocoService, private translocoLocaleService: TranslocoLocaleService) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.empowerments = [
                {
                    name: this.translateService.translate(`modules.services.services-table.filter.isEmpowerment`),
                    code: true,
                },
                {
                    name: this.translateService.translate(`modules.services.services-table.filter.isNotEmpowerment`),
                    code: false,
                },
            ];
        });

        this.subscriptions.add(languageChangeSubscription);
    }

    subscriptions: Subscription = new Subscription();

    @Output() filteredData: EventEmitter<ServicesFilterT> = new EventEmitter();

    empowerments: { name: string; code: boolean }[] = [];

    visible = false;
    filterForm = new FormGroup({
        serviceNumber: new FormControl<ServicesFilterT['serviceNumber']>(null),
        name: new FormControl<ServicesFilterT['name']>(''),
        includeApprovedOnly: new FormControl<ServicesFilterT['includeApprovedOnly']>(false, { nonNullable: true }),
        includeInactive: new FormControl<ServicesFilterT['includeInactive']>(true, { nonNullable: true }),
        includeEmpowermentOnly: new FormControl<ServicesFilterT['includeEmpowermentOnly']>(false, {
            nonNullable: true,
        }),
        includeDeleted: new FormControl<ServicesFilterT['includeDeleted']>(false, { nonNullable: true }),
        includeWithoutScope: new FormControl<ServicesFilterT['includeWithoutScope']>(true, { nonNullable: true }),
    });
    appliedFilterForm = new FormGroup({
        serviceNumber: new FormControl<ServicesFilterT['serviceNumber']>(null),
        name: new FormControl<ServicesFilterT['name']>(''),
        includeApprovedOnly: new FormControl<ServicesFilterT['includeApprovedOnly']>(false, { nonNullable: true }),
        includeInactive: new FormControl<ServicesFilterT['includeInactive']>(true, { nonNullable: true }),
        includeEmpowermentOnly: new FormControl<ServicesFilterT['includeEmpowermentOnly']>(false, {
            nonNullable: true,
        }),
        includeDeleted: new FormControl<ServicesFilterT['includeDeleted']>(false, { nonNullable: true }),
        includeWithoutScope: new FormControl<ServicesFilterT['includeWithoutScope']>(true, { nonNullable: true }),
    });

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    openFilter() {
        this.visible = true;
    }

    closeFilter() {
        this.visible = false;
    }

    get appliedFilters() {
        return Object.keys(this.appliedFilterForm.value).filter(key => {
            const value = this.appliedFilterForm.value[key as keyof ServicesFilterT];
            // if (typeof value === 'boolean') return true;
            return value;
        });
    }

    emitFilteredData() {
        this.filteredData.emit(this.appliedFilterForm.value as ServicesFilterT);
    }

    onSubmit() {
        this.appliedFilterForm.patchValue(this.filterForm.value);
        this.emitFilteredData();
        this.visible = false;
    }

    clearFilterValues() {
        this.resetFormControls(this.filterForm);
        this.resetFormControls(this.appliedFilterForm);
        this.emitFilteredData();
    }

    clearFilteredValue(filteredValue: string) {
        if (typeof this.filterForm.get(filteredValue)?.value === 'boolean') {
            this.appliedFilterForm.get(filteredValue)?.patchValue(!this.filterForm.get(filteredValue)?.value);
            this.filterForm.get(filteredValue)?.patchValue(!this.filterForm.get(filteredValue)?.value);
        } else {
            this.appliedFilterForm.get(filteredValue)?.reset();
            this.filterForm.get(filteredValue)?.reset();
        }
        this.emitFilteredData();
    }

    resetFormControls(form: FormGroup): void {
        Object.keys(form.controls).forEach(key => {
            const control = form.get(key);
            if (control) {
                if (typeof control.value === 'boolean') {
                    control.setValue(false);
                } else {
                    control.reset();
                }
            }
        });
    }
}
