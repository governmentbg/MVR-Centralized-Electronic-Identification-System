import { Component, EventEmitter, Output, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
// import { statuses } from '../application-status/application-status.component';
import { GenerateFormFilters } from '@app/shared/types';
import { GetUserType } from '@app/features/users/users.dto';

export type UserFilterType = GenerateFormFilters<GetUserType, 'name' | 'email' | 'isAdministrator'>;

@Component({
    selector: 'app-users-filter',
    templateUrl: './users-filter.component.html',
    styleUrls: ['./users-filter.component.scss'],
})
export class UsersFilterComponent implements OnDestroy, OnInit {
    constructor(public translateService: TranslocoService, private translocoLocaleService: TranslocoLocaleService) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.roles = [
                {
                    name: this.translateService.translate(`modules.users.usersTable.table.roles.admin`),
                    isAdministartor: true,
                },
                {
                    name: this.translateService.translate(`modules.users.usersTable.table.roles.user`),
                    isAdministartor: false,
                },
            ];
        });
        this.subscriptions.add(languageChangeSubscription);
    }

    subscriptions: Subscription = new Subscription();

    @Output() filteredData: EventEmitter<UserFilterType> = new EventEmitter();

    roles: { name: string; isAdministartor: boolean }[] = [];

    visible = false;
    filterForm = new FormGroup({
        name: new FormControl<UserFilterType['name']>(null),
        email: new FormControl<UserFilterType['email']>(null),
        isAdministrator: new FormControl<UserFilterType['isAdministrator']>(null),
    });
    appliedFilterForm = new FormGroup({
        name: new FormControl<UserFilterType['name']>(null),
        email: new FormControl<UserFilterType['email']>(null),
        isAdministrator: new FormControl<UserFilterType['isAdministrator']>(null),
    });
    sessionStorageKey = 'users-filter';

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
        return Object.keys(this.appliedFilterForm.value).filter(key => {
            const value = this.appliedFilterForm.value[key as keyof UserFilterType];
            if (typeof value === 'boolean') return true;
            return value;
        });
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
