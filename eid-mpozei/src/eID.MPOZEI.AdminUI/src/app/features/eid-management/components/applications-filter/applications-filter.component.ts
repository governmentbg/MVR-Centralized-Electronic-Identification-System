import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { ApplicationStatus, SubmissionType } from '../../enums/eid-management.enum';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { EidDeviceService } from '../../services/eid-device.service';
import { EidAdministratorService } from '../../services/eid-administrator.service';
import { RaeiceiClientService } from '../../services/raeicei-client.service';
import { dateMoreThanOrEqualValidate } from '../../../../shared/validators/date';
import { RoleType } from '../../../../core/enums/auth.enum';
import { ToastService } from '../../../../shared/services/toast.service';
import { UserService } from '../../../../core/services/user.service';

@Component({
    selector: 'app-applications-filter',
    templateUrl: './applications-filter.component.html',
    styleUrls: ['./applications-filter.component.scss'],
})
export class ApplicationsFilterComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private translocoLocaleService: TranslocoLocaleService,
        private eidDeviceService: EidDeviceService,
        private eidAdministratorService: EidAdministratorService,
        private raeiceiClientService: RaeiceiClientService,
        private toastService: ToastService,
        private userService: UserService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.statuses = [
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.Submitted'),
                    code: ApplicationStatus.SUBMITTED,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.Signed'),
                    code: ApplicationStatus.SIGNED,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.Completed'),
                    code: ApplicationStatus.COMPLETED,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.Processing'),
                    code: ApplicationStatus.PROCESSING,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.Paid'),
                    code: ApplicationStatus.PAID,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.PendingPayment'),
                    code: ApplicationStatus.PENDING_PAYMENT,
                },
                {
                    name: this.translateService.translate(
                        'modules.eidManagement.tableFilter.statuses.PendingSignature'
                    ),
                    code: ApplicationStatus.PENDING_SIGNATURE,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.Denied'),
                    code: ApplicationStatus.DENIED,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.Approved'),
                    code: ApplicationStatus.APPROVED,
                },
                {
                    name: this.translateService.translate(
                        'modules.eidManagement.tableFilter.statuses.GeneratedCertificate'
                    ),
                    code: ApplicationStatus.GENERATED_CERTIFICATE,
                },
                {
                    name: this.translateService.translate(
                        'modules.eidManagement.tableFilter.statuses.CertificateStored'
                    ),
                    code: ApplicationStatus.CERTIFICATE_STORED,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.tableFilter.statuses.PaymentExpired'),
                    code: ApplicationStatus.PAYMENT_EXPIRED,
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
            this.administrators = [
                ...[
                    {
                        id: null,
                        name: this.translateService.translate('modules.eidManagement.tableFilter.txtAllPlaceholder'),
                    },
                ],
                ...this.eidAdministratorService.administrators,
            ];
            this.submissionTypes = [
                {
                    id: SubmissionType.DESK,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.DESK}`
                    ),
                },
                {
                    id: SubmissionType.EID,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.EID}`
                    ),
                },
                {
                    id: SubmissionType.BASE_PROFILE,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.BASE_PROFILE}`
                    ),
                },
                {
                    id: SubmissionType.PERSO_CENTRE,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.PERSO_CENTRE}`
                    ),
                },
            ];
            this.applicationTypes = [
                {
                    id: 'REVOKE_EID',
                    name: this.translateService.translate('modules.eidManagement.txtApplicationTypeRevoke'),
                },
                {
                    id: 'STOP_EID',
                    name: this.translateService.translate('modules.eidManagement.txtApplicationTypeStop'),
                },
                {
                    id: 'RESUME_EID',
                    name: this.translateService.translate('modules.eidManagement.txtApplicationTypeResume'),
                },
                {
                    id: 'ISSUE_EID',
                    name: this.translateService.translate('modules.eidManagement.txtApplicationTypeIssue'),
                },
            ];
        });
    }

    @Input() isAdminPage = false;
    @Output() filteredData: EventEmitter<any> = new EventEmitter();
    @Output() exportDataEvent: EventEmitter<any> = new EventEmitter();
    languageChangeSubscription: Subscription;
    statuses: any[] = [];
    carriers: { id: string; name: string }[] = [];
    submitted = false;
    visible = false;
    chosenFilters: any[] = [];
    filterForm = new FormGroup({
        applicationNumber: new FormControl<number | null>(null),
        status: new FormControl<{ name: string; code: number }[] | null>(null),
        deviceId: new FormControl<{ name: string; code: number }[] | null>(null),
        createdDateFrom: new FormControl<string | null>(null, { updateOn: 'change' }),
        createdDateTo: new FormControl<string | null>(null, { updateOn: 'change' }),
        administratorId: new FormControl<string | null>(null),
        eidentityId: new FormControl<string | null>(null),
        submissionType: new FormControl<string[] | null>(null),
        eidAdministratorFrontOfficeId: new FormControl<string[]>({ value: [], disabled: false }),
        applicationType: new FormControl<string[] | null>(null),
    });
    filterFormUnSavedState: any;
    administrators: any[] = [];
    administratorOffices: any[] = [];
    formChangeSubscription1 = new Subscription();
    formChangeSubscription = new Subscription();
    submissionTypes: { id: SubmissionType; name: string }[] = [];
    RoleType = RoleType;
    applicationTypes: any[] = [];

    ngOnInit() {
        this.formChangeSubscription1 = this.filterForm.controls.administratorId.valueChanges.subscribe(controlValue => {
            if (controlValue) {
                this.filterForm.controls.eidAdministratorFrontOfficeId.enable();
            } else {
                this.filterForm.controls.eidAdministratorFrontOfficeId.patchValue(null);
                this.filterForm.controls.eidAdministratorFrontOfficeId.disable();
            }
        });

        this.formChangeSubscription = this.filterForm.valueChanges.subscribe(controlValue => {
            const field1 = this.filterForm.controls.createdDateFrom;
            const field2 = this.filterForm.controls.createdDateTo;

            const isField1Filled = field1.value != null && field1.value !== '';
            const isField2Filled = field2.value != null && field2.value !== '';

            if (isField1Filled && !isField2Filled) {
                field2.setValidators([Validators.required, dateMoreThanOrEqualValidate('createdDateFrom')]);
                field2.markAsDirty();
            } else if (isField2Filled && !isField1Filled) {
                field1.setValidators([Validators.required]);
                field1.markAsDirty();
            } else if (isField1Filled && isField2Filled) {
                field2.setValidators([dateMoreThanOrEqualValidate('createdDateFrom')]);
            } else {
                field1.clearValidators();
                field2.clearValidators();
            }

            field1.updateValueAndValidity({ emitEvent: false });
            field2.updateValueAndValidity({ emitEvent: false });
        });

        this.filterForm.controls.administratorId.patchValue(this.eidAdministratorService.currentAdministratorId);
        this.administratorOffices = this.eidAdministratorService.getCurrentAdministratorOffices();

        if (this.isAdminPage) {
            this.filterForm.controls.eidAdministratorFrontOfficeId.patchValue([
                this.eidAdministratorService.currentAdministratorOfficeId,
            ]);
        }

        this.setFilteredValues();
        this.filteredData.emit(this.extractValues());
    }

    ngOnDestroy() {
        this.formChangeSubscription.unsubscribe();
        this.formChangeSubscription1.unsubscribe();
    }

    openFilter() {
        this.visible = true;
        // Save state
        this.filterFormUnSavedState = this.filterForm.getRawValue();
        const bodyTag = document.body;
        bodyTag.classList.add('overflow-hidden');
    }

    closeFilter() {
        this.visible = false;
        // Use state
        this.filterForm.patchValue(this.filterFormUnSavedState);
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
        Object.entries(this.filterForm.getRawValue()).forEach((formValue: any) => {
            // Check if value is string|array|boolean|date|number and not null
            if (this.performDataTypeCheck(formValue[1])) {
                // We don't want to add administratorId when the logged user has role different than APP_ADMINISTRATOR
                if (!this.userService.hasRole(RoleType.APP_ADMINISTRATOR) && formValue[0] === 'administratorId') {
                    return;
                }
                const preparedObject = {
                    key: formValue[0],
                    value: formValue[1],
                    label: `modules.eidManagement.tableFilter.filterLabels.applications.${formValue[0]}`,
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
                        // We don't want to add administratorId when the logged user has role OPERATOR
                        if (this.userService.hasRole(RoleType.OPERATOR) && formValue[0] === 'administratorId') {
                            return;
                        }
                        const preparedObject = {
                            key: formValue[0],
                            value: formValue[1],
                            label: `modules.eidManagement.tableFilter.filterLabels.applications.${formValue[0]}`,
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
        if (filteredValue.key === 'createdDateFrom' || filteredValue.key === 'createdDateTo') {
            this.chosenFilters = this.chosenFilters.filter(
                fV => fV.key !== 'createdDateFrom' && fV.key !== 'createdDateTo'
            );
            (this.filterForm.controls as any)['createdDateFrom'].setValue(null);
            (this.filterForm.controls as any)['createdDateTo'].setValue(null);
        } else if (filteredValue.key === 'administratorId') {
            (this.filterForm.controls as any)['administratorId'].setValue(null);
            (this.filterForm.controls as any)['eidAdministratorFrontOfficeId'].setValue([]);
            this.chosenFilters = this.chosenFilters.filter(
                fV => fV.key !== 'administratorId' && fV.key !== 'eidAdministratorFrontOfficeId'
            );
        } else {
            (this.filterForm.controls as any)[filteredValue.key].setValue(null);
            this.chosenFilters = this.chosenFilters.filter(fV => fV.key !== filteredValue.key);
        }

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
        this.filterForm.controls.administratorId.patchValue(this.eidAdministratorService.currentAdministratorId);
        this.setFilteredValues();
        this.filterFormUnSavedState = this.filterForm.getRawValue();
        this.filteredData.emit(this.extractValues());
    }

    prepareFilteredValueLabel(filteredValue: any): string {
        if (filteredValue.key === 'createdDateFrom' || filteredValue.key === 'createdDateTo') {
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
            Object.entries(this.filterForm.getRawValue()).filter(([, v]: any) => this.performDataTypeCheck(v))
        );

        if (extractedValues && extractedValues.createdDateFrom) {
            const localizedDate = this.translocoLocaleService.localizeDate(
                extractedValues.createdDateFrom,
                this.translocoLocaleService.getLocale()
            );
            extractedValues.createdDateFrom = new Date(`${localizedDate}T00:00:00`).toISOString();
        }
        if (extractedValues && extractedValues.createdDateTo) {
            const localizedDate = this.translocoLocaleService.localizeDate(
                extractedValues.createdDateTo,
                this.translocoLocaleService.getLocale()
            );
            extractedValues.createdDateTo = new Date(`${localizedDate}T23:59:59`).toISOString();
        }
        if (extractedValues && extractedValues.administratorId === 0) {
            delete extractedValues.administratorId;
        }

        delete extractedValues.createDate;
        return extractedValues;
    }

    loadOffices(administrator: { originalEvent?: any; value: string }) {
        if (administrator.value) {
            this.filterForm.controls.eidAdministratorFrontOfficeId.patchValue(null);
            this.raeiceiClientService.getAdministratorOffices(administrator.value).subscribe({
                next: response => {
                    this.administratorOffices = response;
                },
                error: error => {
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        default:
                            this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                            break;
                    }
                },
            });
        }
    }

    refreshData() {
        this.filteredData.emit(this.extractValues());
    }

    exportData() {
        this.exportDataEvent.emit(this.extractValues());
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }
}
