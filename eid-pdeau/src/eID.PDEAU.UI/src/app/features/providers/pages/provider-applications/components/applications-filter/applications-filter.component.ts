import { Component, EventEmitter, Output, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { ProviderStatusType } from '@app/features/providers/provider.dto';
import { statuses } from '@app/features/providers/components/application-status/application-status.component';

export interface ApplicationFilter {
    status?: ProviderStatusType | null;
    providerName?: string | null;
}

@Component({
    selector: 'app-applications-filter',
    templateUrl: './applications-filter.component.html',
    styleUrls: ['./applications-filter.component.scss'],
})
export class ApplicationsFilterComponent implements OnDestroy, OnInit {
    constructor(public translateService: TranslocoService, private translocoLocaleService: TranslocoLocaleService) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.statuses = Object.entries(statuses).map(([key, { translationKey }]) => {
                return {
                    name: this.translateService.translate(`modules.providers.applications.statuses.${translationKey}`),
                    code: key,
                };
            });
        });

        this.subscriptions.add(languageChangeSubscription);
    }

    subscriptions: Subscription = new Subscription();

    @Output() filteredData: EventEmitter<ApplicationFilter> = new EventEmitter();

    statuses: { name: string; code: string }[] = [];

    visible = false;
    filterForm = new FormGroup({
        status: new FormControl<ProviderStatusType | null>(null),
        providerName: new FormControl<string | null>(null),
        number: new FormControl<string | null>(null),
    });
    appliedFilterForm = new FormGroup({
        status: new FormControl<ProviderStatusType | null>(null),
        providerName: new FormControl<string | null>(null),
        number: new FormControl<string | null>(null),
    });
    sessionStorageKey = 'applications-filter';

    ngOnInit() {
        const filtersInState = sessionStorage.getItem(this.sessionStorageKey)
            ? JSON.parse(sessionStorage[this.sessionStorageKey])
            : null;
        if (filtersInState) {
            this.filterForm.patchValue(filtersInState);
            this.onSubmit();
        }
    }

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
        return Object.keys(this.appliedFilterForm.value).filter(
            key => this.appliedFilterForm.value[key as keyof ApplicationFilter]
        );
    }

    emitFilteredData() {
        this.filteredData.emit(this.appliedFilterForm.value);
    }

    onSubmit() {
        this.appliedFilterForm.patchValue(this.filterForm.value);
        this.emitFilteredData();
        sessionStorage.setItem(this.sessionStorageKey, JSON.stringify(this.filterForm.value));
        this.visible = false;
    }

    clearFilterValues() {
        this.filterForm.reset();
        this.appliedFilterForm.reset();
        this.emitFilteredData();
        sessionStorage.removeItem(this.sessionStorageKey);
    }

    clearFilteredValue(filteredValue: string) {
        this.appliedFilterForm.get(filteredValue)?.reset();
        this.filterForm.get(filteredValue)?.reset();
        this.emitFilteredData();
        sessionStorage.setItem(this.sessionStorageKey, JSON.stringify(this.filterForm.value));
    }
}
