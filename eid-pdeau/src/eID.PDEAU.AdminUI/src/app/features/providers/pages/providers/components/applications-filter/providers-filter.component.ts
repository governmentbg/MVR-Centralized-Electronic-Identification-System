import { Component, EventEmitter, Output, OnDestroy } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { ProviderStatusType } from '@app/features/providers/provider.dto';
import { statuses } from '@app/features/providers/components/application-status/application-status.component';

export interface ProviderFilter {
    status?: ProviderStatusType | null;
    providerName?: string | null;
    issuer?: string | null;
}

@Component({
    selector: 'app-providers-filter',
    templateUrl: './providers-filter.component.html',
    styleUrls: ['./providers-filter.component.scss'],
})
export class ProvidersFilterComponent implements OnDestroy {
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

    @Output() filteredData: EventEmitter<ProviderFilter> = new EventEmitter();

    statuses: { name: string; code: string }[] = [];

    visible = false;
    filterForm = new FormGroup({
        status: new FormControl<ProviderStatusType | null>(null),
        providerName: new FormControl<string | null>(null),
        issuer: new FormControl<string | null>(null),
        number: new FormControl<string | null>(null),
    });
    appliedFilterForm = new FormGroup({
        status: new FormControl<ProviderStatusType | null>(null),
        providerName: new FormControl<string | null>(null),
        issuer: new FormControl<string | null>(null),
        number: new FormControl<string | null>(null),
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
        return Object.keys(this.appliedFilterForm.value).filter(
            key => this.appliedFilterForm.value[key as keyof ProviderFilter]
        );
    }

    emitFilteredData() {
        this.filteredData.emit(this.appliedFilterForm.value);
    }

    onSubmit() {
        this.appliedFilterForm.patchValue(this.filterForm.value);
        this.emitFilteredData();
        this.visible = false;
    }

    clearFilterValues() {
        this.filterForm.reset();
        this.appliedFilterForm.reset();
        this.emitFilteredData();
    }

    clearFilteredValue(filteredValue: string) {
        this.appliedFilterForm.get(filteredValue)?.reset();
        this.filterForm.get(filteredValue)?.reset();
        this.emitFilteredData();
    }

    refreshData() {
        this.emitFilteredData();
    }
}
