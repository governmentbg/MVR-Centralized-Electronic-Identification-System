import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { dateMoreThanOrEqualValidate } from '../../../../shared/validators/date';
import { PersonalIdTypes } from '../../../eid-management/enums/eid-management.enum';
import { EGNValidator, PinValidator } from '../../../../shared/validators/egn';
import { IFindEidentityByNumberAndTypeResponseData } from '../../../eid-management/interfaces/eid-management.interfaces';
import { MpozeiClientService } from '../../../eid-management/services/mpozei-client.service';
import { emailValidator } from 'src/app/shared/validators/email';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
    selector: 'app-logs-filter',
    templateUrl: './logs-filter.component.html',
    styleUrls: ['./logs-filter.component.scss'],
})
export class LogsFilterComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private translocoLocaleService: TranslocoLocaleService,
        private mpozeiClientService: MpozeiClientService,
        private toastService: ToastService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.buildEventTypesDropdownData();
        });
    }

    @Input() disableExportBtn = false;
    @Output() filteredData: EventEmitter<any> = new EventEmitter();
    @Output() exportDataEvent: EventEmitter<any> = new EventEmitter();
    languageChangeSubscription: Subscription;
    eventTypes: any[] = [];
    submitted = false;
    visible = false;
    chosenFilters: any[] = [];
    form = {
        eventId: new FormControl<string | null>(null),
        eventTypes: new FormControl<string[] | null>(null),
        startDate: new FormControl<Date | null>(null),
        endDate: new FormControl<Date | null>(null),
        targetUid: new FormControl<string | null>('', {
            validators: [Validators.pattern('[0-9]+'), EGNValidator()],
            updateOn: 'change',
        }),
        targetUidType: new FormControl<string>(PersonalIdTypes.EGN, { nonNullable: true }),
        requesterUid: new FormControl<string | null>('', {
            validators: [Validators.pattern('[0-9]+'), EGNValidator()],
            updateOn: 'change',
        }),
        requesterUidType: new FormControl<string>(PersonalIdTypes.EGN, { nonNullable: true }),
    };
    filterForm: FormGroup = new FormGroup({});
    filterFormUnSavedState: any;
    maxDate = new Date();
    subscriptions: Subscription[] = [];
    identifierTypes = [
        {
            name: this.translateService.translate('modules.eidManagement.txtEGN'),
            id: PersonalIdTypes.EGN,
        },
        {
            name: this.translateService.translate('modules.eidManagement.txtLNCH'),
            id: PersonalIdTypes.LNCH,
        },
    ];
    targetUidIdentifierTypes = [
        ...this.identifierTypes,
        {
            name: this.translateService.translate('modules.eidManagement.txtEmail'),
            id: 'Email',
        },
    ];
    loadingEidStatus = false;
    currentEid: IFindEidentityByNumberAndTypeResponseData | null = null;

    ngOnInit() {
        this.filterForm = new FormGroup(this.form);
        const formChangeSubscription = this.filterForm.valueChanges.subscribe({
            next: () => {
                const endDateControl = this.filterForm.controls['endDate'];
                const startDateValue = this.filterForm.controls['startDate']?.value;
                if (startDateValue) {
                    endDateControl?.setValidators([dateMoreThanOrEqualValidate('startDate')]);
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

    buildEventTypesDropdownData() {
        const obj = this.translateService.getTranslation(`logs/${this.translateService.getActiveLang()}`);
        this.eventTypes = Object.keys(obj).map(key => {
            return { value: key, name: obj[key] || key };
        });
    }

    endDateMinValue(): Date {
        return this.filterForm.controls['startDate'].value || null;
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

    async onSubmit() {
        this.filterForm.markAllAsTouched();
        this.submitted = true;
        if (this.filterForm.valid) {
            if (this.filterForm.controls['targetUid'].value) {
                await this.checkForEid();
                if (this.currentEid === null) {
                    this.toastService.showErrorToast(
                        this.translateService.translate('global.txtErrorTitle'),
                        this.translateService.translate('modules.logsViewer.txtNoEidForThisEmail')
                    );
                    return;
                }
            }
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
                if (preparedObject.key !== 'targetUidType' && preparedObject.key !== 'requesterUidType') {
                    this.chosenFilters.push(preparedObject);
                }
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

                        if (preparedObject.key !== 'targetUidType' && preparedObject.key !== 'requesterUidType') {
                            chosenFilters.push(preparedObject);
                        }
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
        this.exportDataEvent.emit();
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

        // We do not want the uidType to be added in the chips
        if (extractedValues && !extractedValues.requesterUid) {
            delete extractedValues.requesterUidType;
        }
        // We do not want the uidType to be added in the chips
        if (extractedValues && !extractedValues.targetUid) {
            delete extractedValues.targetUidType;
        }

        // When we have a targetUid we also need to send citizenProfileId and eidentityId
        if (extractedValues && extractedValues.targetUid && this.currentEid) {
            if (this.currentEid.citizenProfileId) {
                extractedValues.userId = this.currentEid.citizenProfileId;
            }
            if (this.currentEid.eidentityId) {
                extractedValues.eId = this.currentEid.eidentityId;
            }
        } else {
            delete extractedValues.userId;
            delete extractedValues.eId;
        }
        // If targetUidType is Email we don't want to send it to the API
        if (extractedValues.targetUidType === 'Email') {
            delete extractedValues.targetUidType;
            delete extractedValues.targetUid;
        }

        return extractedValues;
    }

    onTargetIdentifierChange(event: any) {
        if (event.value === PersonalIdTypes.EGN) {
            this.form.targetUid.setValidators([Validators.pattern('[0-9]+'), EGNValidator()]);
        } else if (event.value === PersonalIdTypes.LNCH) {
            this.form.targetUid.setValidators([Validators.pattern('[0-9]+'), PinValidator()]);
        } else if (event.value === 'Email') {
            this.form.targetUid.setValidators([emailValidator()]);
        }
        this.form.targetUid.updateValueAndValidity();
    }

    onRequesterIdentifierChange(event: any) {
        if (event.value === PersonalIdTypes.EGN) {
            this.form.requesterUid.setValidators([Validators.pattern('[0-9]+'), EGNValidator()]);
        } else if (event.value === PersonalIdTypes.LNCH) {
            this.form.requesterUid.setValidators([Validators.pattern('[0-9]+'), PinValidator()]);
        }
        this.form.requesterUid.updateValueAndValidity();
    }

    checkForEid() {
        let params: any = {};
        if (this.filterForm.controls['targetUidType'].value === 'Email') {
            params = {
                email: this.filterForm.controls['targetUid'].value,
            };
        } else {
            params = {
                type: this.filterForm.controls['targetUidType'].value,
                number: this.filterForm.controls['targetUid'].value,
            };
        }

        return new Promise<any>(resolve => {
            this.mpozeiClientService.findEidentityByNumberAndType(params).subscribe({
                next: response => {
                    this.currentEid = response;
                    this.loadingEidStatus = false;
                    resolve(true);
                },
                error: (error: any) => {
                    this.currentEid = null;
                    this.loadingEidStatus = false;
                    resolve(true);
                },
            });
        });
    }
}
