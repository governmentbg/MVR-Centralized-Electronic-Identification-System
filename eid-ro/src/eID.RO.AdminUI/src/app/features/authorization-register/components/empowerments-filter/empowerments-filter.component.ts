import { Component, EventEmitter, Output, OnInit, OnDestroy } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { EmpowermentStatementStatus, IdentifierType, OnBehalfOf } from '../../enums/authorization-register.enum';
import { Subscription } from 'rxjs';
import { EGNAdultValidator, EGNValidator, PinValidator } from '../../../../shared/validators/egn';
import { formArrayFormGroupDuplicateValidator } from '../../../../shared/validators/duplicate';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { BulstatValidator } from '../../../../shared/validators/bulstat';
import { dateMoreThanOrEqualValidate } from '../../../../shared/validators/date';

@Component({
    selector: 'app-empowerments-filter',
    templateUrl: './empowerments-filter.component.html',
    styleUrls: ['./empowerments-filter.component.scss'],
})
export class EmpowermentsFilterComponent implements OnInit, OnDestroy {
    constructor(public translateService: TranslocoService, private translocoLocaleService: TranslocoLocaleService) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.statuses = [
                {
                    name: this.translateService.translate('modules.authorizationRegister.tableFilter.statuses.Active'),
                    code: EmpowermentStatementStatus.Active,
                },
                {
                    name: this.translateService.translate('modules.authorizationRegister.tableFilter.statuses.Expired'),
                    code: EmpowermentStatementStatus.Expired,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationRegister.tableFilter.statuses.DisagreementDeclared'
                    ),
                    code: EmpowermentStatementStatus.DisagreementDeclared,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationRegister.tableFilter.statuses.Withdrawn'
                    ),
                    code: EmpowermentStatementStatus.Withdrawn,
                },
                {
                    name: this.translateService.translate('modules.authorizationRegister.tableFilter.statuses.Denied'),
                    code: EmpowermentStatementStatus.Denied,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationRegister.tableFilter.statuses.Unconfirmed'
                    ),
                    code: EmpowermentStatementStatus.Unconfirmed,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationRegister.tableFilter.statuses.AwaitingSignature'
                    ),
                    code: EmpowermentStatementStatus.CollectingAuthorizerSignatures,
                },
                {
                    name: this.translateService.translate('modules.authorizationRegister.tableFilter.statuses.Created'),
                    code: EmpowermentStatementStatus.Created,
                },
            ];

            this.applicants = [
                {
                    name: this.translateService.translate(
                        'modules.authorizationRegister.tableFilter.applicants.LegalEntity'
                    ),
                    code: OnBehalfOf.LegalEntity,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationRegister.tableFilter.applicants.Individual'
                    ),
                    code: OnBehalfOf.Individual,
                },
            ];
        });
    }

    @Output() filteredData: EventEmitter<any> = new EventEmitter();
    languageChangeSubscription: Subscription;
    onBehalfOf = OnBehalfOf;
    statuses: any[] = [];
    applicants = [
        {
            name: this.translateService.translate('modules.authorizationRegister.tableFilter.applicants.LegalEntity'),
            code: OnBehalfOf.LegalEntity,
        },
        {
            name: this.translateService.translate('modules.authorizationRegister.tableFilter.applicants.Individual'),
            code: OnBehalfOf.Individual,
        },
    ];
    identifierTypes = [
        {
            name: this.translateService.translate('modules.authorizationStatement.statementForm.EGN'),
            id: IdentifierType.EGN,
        },
        {
            name: this.translateService.translate('modules.authorizationStatement.statementForm.LNCh'),
            id: IdentifierType.LNCh,
        },
    ];
    submitted = false;
    visible = false;
    chosenFilters: any[] = [];
    legalEntityNamePattern = /^[аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ -.&\d]+$/;
    form = new FormGroup({
        status: new FormControl<number | null>(null),
        serviceName: new FormControl<string | null>(null),
        providerName: new FormControl<string | null>(null),
        showOnlyNoExpiryDate: new FormControl<boolean | null>(null),
        validToDate: new FormControl<Date | null>(null),
        createdOnFrom: new FormControl<Date | null>(null, { updateOn: 'change' }),
        createdOnTo: new FormControl<Date | null>(null, { updateOn: 'change' }),
        number: new FormControl<string | null>(null),
        onBehalfOf: new FormControl<OnBehalfOf | null>(null),
        legalEntityName: new FormControl<string | null>(null, [Validators.pattern(this.legalEntityNamePattern)]),
        authorizer: new FormControl<string | null>(null),
        empowermentUid: new FormControl<string | null>(null, {
            validators: [],
            updateOn: 'change',
        }),
        empoweredUids: new FormArray(
            [
                new FormGroup({
                    uidType: new FormControl(IdentifierType.EGN, [Validators.required]),
                    uid: new FormControl('', {
                        validators: [],
                        updateOn: 'change',
                    }),
                }),
            ],
            [formArrayFormGroupDuplicateValidator()]
        ),
    });
    filterForm = this.form;
    filterFormUnSavedState: any;
    maxEmpoweredUids = 10;
    maxDate = new Date();
    formChangeSubscription: Subscription = new Subscription();

    ngOnInit() {
        this.filterForm.get('legalEntityName')?.disable();

        this.formChangeSubscription.add(
            this.empoweredUids?.valueChanges.subscribe(controlValues => {
                if (
                    controlValues.length > 1 ||
                    (controlValues.length === 1 && controlValues[0].uid !== '' && controlValues[0].uid !== null)
                ) {
                    controlValues.forEach((controlValue: any, index: number) => {
                        if (this.empoweredUids.controls[index].controls['uidType'].value === IdentifierType.EGN) {
                            this.empoweredUids.controls[index].controls['uid'].setValidators([
                                Validators.pattern('[0-9]+'),
                                EGNAdultValidator(),
                                EGNValidator(),
                                Validators.required,
                            ]);
                        } else if (
                            this.empoweredUids.controls[index].controls['uidType'].value === IdentifierType.LNCh
                        ) {
                            this.empoweredUids.controls[index].controls['uid'].setValidators([
                                Validators.pattern('[0-9]+'),
                                PinValidator(),
                                Validators.required,
                            ]);
                        }
                        this.empoweredUids.controls[index].controls['uid'].updateValueAndValidity({ emitEvent: false });
                    });
                } else if (controlValues.length === 1) {
                    this.empoweredUids.controls[0].controls['uid'].setValidators([]);
                    this.empoweredUids.controls[0].controls['uid'].updateValueAndValidity({ emitEvent: false });
                }
            })
        );

        this.formChangeSubscription.add(
            this.filterForm.controls.empowermentUid.valueChanges.subscribe(controlValue => {
                if (controlValue) {
                    this.filterForm.controls.empowermentUid.setValidators([BulstatValidator()]);
                } else {
                    this.filterForm.controls.empowermentUid.setValidators([]);
                }
                this.filterForm.controls.empowermentUid.updateValueAndValidity({ emitEvent: false });
            })
        );

        this.formChangeSubscription.add(
            this.filterForm.valueChanges.subscribe(controlValue => {
                const field1 = this.filterForm.controls.createdOnFrom;
                const field2 = this.filterForm.controls.createdOnTo;

                const isField1Filled = field1.value != null;
                const isField2Filled = field2.value != null;

                if (isField1Filled && !isField2Filled) {
                    field2.setValidators([Validators.required, dateMoreThanOrEqualValidate('createdOnFrom')]);
                    field2.markAsDirty();
                } else if (isField2Filled && !isField1Filled) {
                    field1.setValidators([Validators.required]);
                    field1.markAsDirty();
                } else if (isField1Filled && isField2Filled) {
                    field2.setValidators([dateMoreThanOrEqualValidate('createdOnFrom')]);
                } else {
                    field1.clearValidators();
                    field2.clearValidators();
                }

                field1.updateValueAndValidity({ emitEvent: false });
                field2.updateValueAndValidity({ emitEvent: false });
            })
        );
    }

    ngOnDestroy() {
        this.formChangeSubscription.unsubscribe();
    }

    onFocusCalendar() {
        this.filterForm.controls['showOnlyNoExpiryDate'].reset();
    }

    onCheckboxChange() {
        this.filterForm.controls['validToDate'].reset();
    }

    onOnBehalfOfChange(event: any) {
        if (event.value === OnBehalfOf.LegalEntity) {
            this.filterForm.get('legalEntityName')?.enable();
        } else {
            this.filterForm.get('legalEntityName')?.disable();
            this.filterForm.get('legalEntityName')?.setValue(null);
        }
    }

    openFilter() {
        this.visible = true;
        // Save state
        this.filterFormUnSavedState = this.filterForm.value;
        if (this.filterForm.get('legalEntityName')) {
            this.filterFormUnSavedState.legalEntityName = this.filterForm.get('legalEntityName')?.value;
        }
    }

    closeFilter() {
        this.visible = false;
        // Use state
        if (!Object.prototype.hasOwnProperty.call(this.filterFormUnSavedState, 'legalEntityName')) {
            this.filterFormUnSavedState.legalEntityName = null;
        }
        this.filterForm.setValue(this.filterFormUnSavedState);
    }

    onApplicantClear() {
        this.filterForm.get('legalEntityName')?.setValue(null);
    }

    onStatusChange(event: any) {
        if (event.value === EmpowermentStatementStatus.Expired) {
            this.filterForm.get('showOnlyNoExpiryDate')?.disable();
        } else {
            this.filterForm.get('showOnlyNoExpiryDate')?.enable();
        }
    }

    onStatusClear() {
        this.filterForm.get('showOnlyNoExpiryDate')?.enable();
    }

    onSubmit() {
        this.filterForm.markAllAsTouched();
        this.submitted = true;
        if (this.empoweredUids && this.empoweredUids.length > 1) {
            Object.values(this.empoweredUids.controls).forEach((control: any) => {
                control.markAsDirty();
            });
        }
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
                // We don't want to add empty empoweredUids to the chosenFilters
                if (
                    formValue[0] === 'empoweredUids' &&
                    formValue[1].length === 1 &&
                    (formValue[1][0]?.uid === null || formValue[1][0]?.uid === '')
                ) {
                    return;
                }
                const preparedObject = {
                    key: formValue[0],
                    value: formValue[1],
                    label: `modules.authorizationRegister.tableFilter.filterLabels.${formValue[0]}`,
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
                this.chosenFilters.forEach((chosenFilter, index, chosenFilters) => {
                    // If filter is preselected
                    if (formValue[0] === chosenFilter.key) {
                        // Update the value
                        chosenFilter.value = formValue[1];

                        // If user choose individual option in onBehalfOf we should ensure
                        // that legalEntityName is cleaned if it was populated before
                        if (formValue[0] === 'onBehalfOf' && formValue[1] === OnBehalfOf.Individual) {
                            this.removeLegalEntityName();
                        }

                        // If current value is null | empty string | boolean false, then remove it from the chosenFilters array
                        if (!this.performDataTypeCheck(formValue[1])) {
                            chosenFilters.splice(index, 1);
                            // If onBehalfOf is null then remove legalEntityName if it is not empty
                            if (formValue[0] === 'onBehalfOf') {
                                this.removeLegalEntityName();
                            }

                            // When user already clicked on 'showOnlyNoExpiryDate' checkbox control and after that click on calendar control,
                            // system disable 'showOnlyNoExpiryDate' checkbox it programmatically and it is removed from chosenFilters array,
                            // then current filter loop exits and calendar value is not added. This is the program way to fix this issue
                            if (
                                formValue[0] === 'showOnlyNoExpiryDate' &&
                                this.chosenFilters.length === 0 &&
                                this.filterForm.value.validToDate !== null
                            ) {
                                chosenFilters.push({
                                    key: 'validToDate',
                                    value: formValue[1],
                                    label: `modules.authorizationRegister.tableFilter.filterLabels.validDate`,
                                });
                            }
                        }

                        if (formValue[0] === 'empoweredUids') {
                            const uids = formValue[1].filter((controlValue: any) => {
                                return controlValue.uid !== '' && controlValue.uid !== null;
                            });

                            if (uids.length === 0) {
                                chosenFilters.splice(index, 1);
                            }
                        }
                    }
                });
                // If current filter is not preselected in the chosenFilters array but its value is not null it should be added
                if (
                    this.performDataTypeCheck(formValue[1]) &&
                    !this.chosenFilters.find(fV => fV.key === formValue[0])
                ) {
                    // We don't want to add empty empoweredUids to the chosenFilters
                    if (
                        formValue[0] === 'empoweredUids' &&
                        formValue[1].length === 1 &&
                        (formValue[1][0]?.uid === null || formValue[1][0]?.uid === '')
                    ) {
                        return;
                    }
                    const preparedObject = {
                        key: formValue[0],
                        value: formValue[1],
                        label: `modules.authorizationRegister.tableFilter.filterLabels.${formValue[0]}`,
                    };
                    this.chosenFilters.push(preparedObject);
                }
            });
        }
    }

    removeLegalEntityName() {
        const legalEntityNameIndex = this.chosenFilters.findIndex((cF: any) => cF.key === 'legalEntityName');
        if (
            this.chosenFilters &&
            this.chosenFilters[legalEntityNameIndex] &&
            this.chosenFilters[legalEntityNameIndex].value &&
            this.chosenFilters[legalEntityNameIndex].value.length > 0
        ) {
            this.chosenFilters.splice(legalEntityNameIndex, 1);
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
        if (Array.isArray(filteredValue.value) && this.filterForm.get(filteredValue.key) instanceof FormArray) {
            (this.filterForm.get(filteredValue.key) as FormArray).clear();
            this.addEmpoweredUid();
            if (this.empoweredUids) {
                this.empoweredUids.markAsPristine();
            }
        } else if (filteredValue.key === 'createdOnFrom' || filteredValue.key === 'createdOnTo') {
            this.chosenFilters = this.chosenFilters.filter(
                fV => fV.key !== 'createdOnFrom' && fV.key !== 'createdOnTo'
            );
            (this.filterForm.controls as any)['createdOnFrom'].setValue(null);
            (this.filterForm.controls as any)['createdOnTo'].setValue(null);
        } else {
            (this.filterForm.controls as any)[filteredValue.key].setValue(null);

            // Hide the legalEntityName control when onBehalfOf control is cleared
            if (filteredValue.key === 'onBehalfOf' && filteredValue.value === OnBehalfOf.LegalEntity) {
                this.filterForm.get('legalEntityName')?.setValue(null);
                this.filterForm.get('legalEntityName')?.disable();
                this.removeLegalEntityName();
            }
        }
        this.filterForm.markAllAsTouched();
        this.chosenFilters = this.chosenFilters.filter(fV => fV.key !== filteredValue.key);
        this.filteredData.emit(this.extractValues());
    }

    clearAllFilteredValues() {
        this.chosenFilters = [];
        this.filterForm.reset();
        if (this.empoweredUids) {
            this.empoweredUids.clear();
            this.addEmpoweredUid();
            this.empoweredUids.markAsPristine();
        }
        this.filterForm.get('legalEntityName')?.disable();
        this.filterFormUnSavedState = this.filterForm.value;
        this.filteredData.emit({});
    }

    refreshData() {
        this.filteredData.emit(this.extractValues());
    }

    prepareFilteredValueLabel(filteredValue: any): string {
        if (
            filteredValue.key === 'validToDate' ||
            filteredValue.key === 'createdOnFrom' ||
            filteredValue.key === 'createdOnTo'
        ) {
            return (
                this.translateService.translate(filteredValue.label) +
                ': ' +
                this.translocoLocaleService.localizeDate(filteredValue.value, this.translocoLocaleService.getLocale())
            );
        } else if (filteredValue.key === 'legalEntityName') {
            return this.translateService.translate(filteredValue.label) + ': ' + filteredValue.value;
        } else {
            return this.translateService.translate(filteredValue.label);
        }
    }

    extractValues(): any {
        const extractedValues: any = Object.fromEntries(
            Object.entries(this.filterForm.value).filter(([, v]: any) => this.performDataTypeCheck(v))
        );

        if (extractedValues && extractedValues.validToDate) {
            const validToDate = new Date(extractedValues.validToDate);
            validToDate.setHours(23, 59, 59, 999);
            extractedValues.validToDate = validToDate.toISOString();
        }

        if (extractedValues && extractedValues.createdOnFrom) {
            const localizedDate = this.translocoLocaleService.localizeDate(
                extractedValues.createdOnFrom,
                this.translocoLocaleService.getLocale()
            );
            extractedValues.createdOnFrom = new Date(`${localizedDate}T00:00:00`).toISOString();
        }
        if (extractedValues && extractedValues.createdOnTo) {
            const localizedDate = this.translocoLocaleService.localizeDate(
                extractedValues.createdOnTo,
                this.translocoLocaleService.getLocale()
            );
            extractedValues.createdOnTo = new Date(`${localizedDate}T23:59:59`).toISOString();
        }

        if (extractedValues && extractedValues.empoweredUids) {
            extractedValues.empoweredUids = extractedValues.empoweredUids.filter((controlValue: any) => {
                return controlValue.uid !== '' && controlValue.uid !== null;
            });

            if (extractedValues.empoweredUids.length === 0) {
                delete extractedValues.empoweredUids;
            }
        }

        // Quick fix because on the BE there isn't "legalEntityName" property
        // For both the authorizer (to me page) and legal entity name (from me page) is called authorizer
        // So to not break the FE logic and in the same time communicate with the BE with the right property this fix is needed
        if (extractedValues && extractedValues.legalEntityName) {
            extractedValues.authorizer = extractedValues.legalEntityName;
            delete extractedValues.legalEntityName;
        }

        if (extractedValues && extractedValues.createdOn) {
            extractedValues.createdOnFrom = extractedValues.createdOn[0];
            extractedValues.createdOnTo = extractedValues.createdOn[1];
            delete extractedValues.createdOn;
        }

        return extractedValues;
    }

    get empoweredUids() {
        return this.filterForm.get('empoweredUids') as FormArray<FormGroup>;
    }

    addEmpoweredUid(identifierValue = null, uidType = IdentifierType.EGN) {
        if (this.empoweredUids) {
            const validators = [Validators.required, Validators.pattern('[0-9]+')];
            if (uidType === IdentifierType.EGN) {
                validators.push(EGNAdultValidator(), EGNValidator());
            } else if (uidType === IdentifierType.LNCh) {
                validators.push(PinValidator());
            }
            this.empoweredUids.push(
                new FormGroup({
                    uidType: new FormControl(uidType, [Validators.required]),
                    uid: new FormControl(identifierValue, { validators, updateOn: 'change' }),
                })
            );
            Object.values(this.empoweredUids.controls).forEach((control: any) => {
                Object.values(control.controls).forEach((fg: any) => {
                    fg.markAsDirty();
                    fg.markAllAsTouched();
                });
            });
        }
    }

    removeEmpoweredUid(i: number) {
        this.empoweredUids.removeAt(i);
        this.empoweredUids.markAsDirty();
    }

    onIdentifierChange(event: any, i: number) {
        if (event.value === IdentifierType.EGN) {
            this.empoweredUids.controls[i].controls['uid'].setValidators([
                Validators.pattern('[0-9]+'),
                EGNAdultValidator(),
                EGNValidator(),
            ]);
        } else if (event.value === IdentifierType.LNCh) {
            this.empoweredUids.controls[i].controls['uid'].setValidators([
                Validators.pattern('[0-9]+'),
                PinValidator(),
            ]);
        }
        this.empoweredUids.controls[i].controls['uid'].updateValueAndValidity();
    }

    get minDate() {
        return this.form.controls.createdOnFrom.value || new Date();
    }
}
