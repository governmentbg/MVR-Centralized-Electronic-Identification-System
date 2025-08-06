import { Component, EventEmitter, Output, OnInit, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { ApplicationTypes, CertificateStatus } from '../../enums/eid-management';
import { IEidAdministrator, IDevices } from '../../interfaces/eid-management.interface';

@Component({
    selector: 'app-certificates-filter',
    templateUrl: './certificates-filter.component.html',
    styleUrls: ['./certificates-filter.component.scss'],
})
export class CertificatesFilterComponent implements OnInit {
    constructor(
        public translateService: TranslocoService,
        private translocoLocaleService: TranslocoLocaleService,
        private formBuilder: FormBuilder
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.statuses = [
                {
                    name: this.translateService.translate('modules.eidManagement.certificateStatus.txtActive'),
                    code: CertificateStatus.ACTIVE,
                    icon: 'e877',
                },
                {
                    name: this.translateService.translate('modules.eidManagement.certificateStatus.txtRevoked'),
                    code: CertificateStatus.REVOKED,
                    icon: 'e877',
                },
                {
                    name: this.translateService.translate('modules.eidManagement.certificateStatus.txtStopped'),
                    code: CertificateStatus.STOPPED,
                    icon: 'e644',
                },
                {
                    name: this.translateService.translate('modules.eidManagement.certificateStatus.txtExpired'),
                    code: CertificateStatus.EXPIRED,
                    icon: 'e644',
                },
            ];

            this.applicationTypes = [
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
        });
    }
    @Output() filteredData: EventEmitter<any> = new EventEmitter();
    @Input() administratorsList!: IEidAdministrator[];
    @Input() devicesList!: IDevices[];
    languageChangeSubscription: Subscription;
    statuses = [
        {
            name: this.translateService.translate('modules.eidManagement.applicationStatus.txtCertificateStored'),
            code: CertificateStatus.ACTIVE,
            icon: 'e877',
        },
        {
            name: this.translateService.translate('modules.eidManagement.applicationStatus.txtCertificateGenerated'),
            code: CertificateStatus.REVOKED,
            icon: 'e877',
        },
        {
            name: this.translateService.translate('modules.eidManagement.applicationStatus.txtDenied'),
            code: CertificateStatus.STOPPED,
            icon: 'e644',
        },
    ];

    applicationTypes = [
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
    submitted = false;
    visible = false;
    chosenFilters: any[] = [];
    form!: FormGroup;
    filterForm: FormGroup = new FormGroup({});
    filterFormUnSavedState: any;
    maxCertificateUids = 10;
    requestInProgress!: boolean;
    ngOnInit() {
        this.filterForm = this.formBuilder.group({
            serialNumber: [null],
            alias: [null],
            status: [null],
            deviceId: [null],
            eidAdministratorId: [null],
            validityFrom: [null],
            validityUntil: [null],
        });
    }

    openFilter() {
        this.visible = true;
        // Save state
        this.filterFormUnSavedState = this.filterForm.value;
    }

    closeFilter() {
        this.visible = false;
        // Use state
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
            if (formValue[1] !== null) {
                // We don't want to add empty certificateUids to the chosenFilters
                const preparedObject = {
                    key: formValue[0],
                    value: formValue[1],
                    label: `modules.eidManagement.tableFilter.txt${formValue[0][0].toUpperCase()}${formValue[0].substring(
                        1
                    )}`,
                };
                this.chosenFilters.push(preparedObject);
            }
        });
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
                    // We don't want to add empty certificateUids to the chosenFilters
                    const preparedObject = {
                        key: formValue[0],
                        value: formValue[1],
                        label: `modules.eidManagement.tableFilter.txt${formValue[0][0].toUpperCase()}${formValue[0].substring(
                            1
                        )}`,
                    };
                    this.chosenFilters.push(preparedObject);
                }
            });
        }
    }

    // Check if value is not null but 'number | string | array | boolean | Date'
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

    clearFilteredValue(filteredValue: any) {
        const control = this.filterForm.controls[filteredValue.key];

        if (control instanceof FormArray) {
            control.clear();
        } else {
            control.setValue(null);
        }
        this.filterForm.markAllAsTouched();
        this.chosenFilters = this.chosenFilters.filter(fV => fV.key !== filteredValue.key);
        this.filteredData.emit(this.extractValues());
    }

    clearAllFilteredValues() {
        this.chosenFilters = [];
        this.filterForm.reset();
        this.filterFormUnSavedState = this.filterForm.value;
        this.filteredData.emit(null);
    }

    prepareFilteredValueLabel(filteredValue: any): string {
        if (filteredValue.key === 'validityFrom' || filteredValue.key === 'validityUntil') {
            const formattedDate = this.formatDate(filteredValue.value);
            return (
                this.translateService.translate(filteredValue.label) +
                ': ' +
                this.translocoLocaleService.localizeDate(formattedDate, this.translocoLocaleService.getLocale())
            );
        } else {
            return this.translateService.translate(filteredValue.label);
        }
    }

    extractValues(): any {
        const extractedValues: any = Object.fromEntries(
            Object.entries(this.filterForm.value).filter(([, v]: any) => this.performDataTypeCheck(v))
        );

        if (extractedValues && extractedValues.validityFrom) {
            extractedValues.validityFrom = this.formatDate(extractedValues.validityFrom);
        }

        if (extractedValues && extractedValues.validityUntil) {
            extractedValues.validityUntil = this.formatDate(extractedValues.validityUntil);
        }

        return extractedValues;
    }

    formatDate(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
    }

    translateDeviceName() {
        const language = this.translocoLocaleService.getLocale();
        if (language === 'bg-BG') {
            return 'name';
        } else {
            return 'description';
        }
    }

    refreshData() {
        this.filteredData.emit(this.extractValues());
    }
}
