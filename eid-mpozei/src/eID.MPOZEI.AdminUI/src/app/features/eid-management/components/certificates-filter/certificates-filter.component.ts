import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { CertificateStatus } from '../../enums/eid-management.enum';
import { Subscription } from 'rxjs';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { EidDeviceService } from '../../services/eid-device.service';

@Component({
    selector: 'app-certificates-filter',
    templateUrl: './certificates-filter.component.html',
    styleUrls: ['./certificates-filter.component.scss'],
})
export class CertificatesFilterComponent {
    constructor(
        public translateService: TranslocoService,
        private translocoLocaleService: TranslocoLocaleService,
        private eidDeviceService: EidDeviceService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.statuses = [
                {
                    name: this.translateService.translate(
                        'modules.eidManagement.tableFilter.certificateStatuses.Active'
                    ),
                    code: CertificateStatus.ACTIVE,
                    icon: 'e644',
                },
                {
                    name: this.translateService.translate(
                        'modules.eidManagement.tableFilter.certificateStatuses.Revoked'
                    ),
                    code: CertificateStatus.REVOKED,
                    icon: 'e644',
                },
                {
                    name: this.translateService.translate(
                        'modules.eidManagement.tableFilter.certificateStatuses.Stopped'
                    ),
                    code: CertificateStatus.STOPPED,
                    icon: 'e644',
                },
                {
                    name: this.translateService.translate(
                        'modules.eidManagement.tableFilter.certificateStatuses.Expired'
                    ),
                    code: CertificateStatus.EXPIRED,
                    icon: 'e644',
                },
                {
                    name: this.translateService.translate(
                        'modules.eidManagement.tableFilter.certificateStatuses.Failed'
                    ),
                    code: CertificateStatus.FAILED,
                    icon: 'e644',
                },
            ];
            this.carriers = this.eidDeviceService.devices.map(device => {
                return {
                    id: device.id,
                    name:
                        device.translations.find(
                            (translation: any) => translation.language === this.translateService.getActiveLang()
                        )?.name || '',
                };
            });
        });
    }

    @Input() isFromMePage = false;
    @Output() filteredData: EventEmitter<any> = new EventEmitter();
    languageChangeSubscription: Subscription;
    statuses = [
        {
            name: this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Active'),
            code: CertificateStatus.ACTIVE,
            icon: 'e644',
        },
        {
            name: this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Revoked'),
            code: CertificateStatus.REVOKED,
            icon: 'e644',
        },
        {
            name: this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Stopped'),
            code: CertificateStatus.STOPPED,
            icon: 'e644',
        },
    ];
    carriers: { id: string; name: string }[] = [];
    submitted = false;
    visible = false;
    chosenFilters: any[] = [];
    filterForm = new FormGroup({
        serialNumber: new FormControl<number | null>(null),
        validityFrom: new FormControl<string | null>(null),
        validityUntil: new FormControl<string | null>(null),
        status: new FormControl<{ name: string; code: number }[] | null>(null),
        deviceId: new FormControl<{ name: string; code: number }[] | null>(null),
    });
    filterFormUnSavedState: any;

    openFilter() {
        this.visible = true;
        // Save state
        this.filterFormUnSavedState = this.filterForm.value;
        const bodyTag = document.body;
        bodyTag.classList.add('overflow-hidden');
    }

    closeFilter() {
        this.visible = false;
        // Use state
        this.filterForm.setValue(this.filterFormUnSavedState);
        const bodyTag = document.body;
        bodyTag.classList.remove('overflow-hidden');
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
        this.submitted = true;
        if (this.filterForm.valid) {
            const bodyTag = document.body;
            bodyTag.classList.remove('overflow-hidden');
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
                    label: `modules.eidManagement.tableFilter.filterLabels.certificates.${formValue[0]}`,
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

                        // If current filter is not preselected in the chosenFilters array but its value is not null it should be added
                    } else if (
                        this.performDataTypeCheck(formValue[1]) &&
                        !this.chosenFilters.find(fV => fV.key === formValue[0])
                    ) {
                        const preparedObject = {
                            key: formValue[0],
                            value: formValue[1],
                            label: `modules.eidManagement.tableFilter.filterLabels.certificates.${formValue[0]}`,
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
        Object.values(this.filterForm.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    innerControl.markAsDirty();
                });
            } else {
                control.markAsDirty();
            }
        });
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
        if (filteredValue.key === 'validityUntil' || filteredValue.key === 'validityFrom') {
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

        if (extractedValues) {
            if (extractedValues.validityFrom) {
                extractedValues.validityFrom = this.translocoLocaleService.localizeDate(
                    extractedValues.validityFrom,
                    this.translocoLocaleService.getLocale()
                );
            }
            if (extractedValues.validityUntil) {
                extractedValues.validityUntil = this.translocoLocaleService.localizeDate(
                    extractedValues.validityUntil,
                    this.translocoLocaleService.getLocale()
                );
            }
        }
        return extractedValues;
    }

    refreshData() {
        this.filteredData.emit(this.extractValues());
    }
}
