import { Component, EventEmitter, Output, OnInit, Input } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { EmpowermentStatementStatus, IdentifierType, OnBehalfOf } from '../../enums/authorization-register.enum';
import { Subscription } from 'rxjs';
import { EGNAdultValidator, EGNValidator, PinValidator } from '../../../../shared/validators/egn';
import { formArrayFormGroupDuplicateValidator } from '../../../../shared/validators/duplicate';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { BulstatValidator } from '../../../../shared/validators/bulstat';

@Component({
    selector: 'app-empowerments-filter',
    templateUrl: './empowerments-filter.component.html',
    styleUrls: ['./empowerments-filter.component.scss'],
})
export class EmpowermentsFilterComponent implements OnInit {
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

    @Input() isFromMePage = false;
    @Input() isLegalEntityReferencePage = false;
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
    form: any = {
        status: new FormControl<number | null>(null),
        serviceName: new FormControl<string | null>(null),
        providerName: new FormControl<string | null>(null),
        showOnlyNoExpiryDate: new FormControl<boolean | null>(null),
        validToDate: new FormControl<Date | null>(null),
        number: new FormControl<string | null>(null),
    };
    filterForm: FormGroup = new FormGroup({});
    filterFormUnSavedState: any;
    maxEmpoweredUids = 10;
    legalEntityNamePattern = /^[аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ -.&\d]+$/;

    ngOnInit() {
        if (this.isFromMePage) {
            this.form.onBehalfOf = new FormControl<number | null>(null);
            this.form.legalEntityName = new FormControl<string | null>(null, [
                Validators.pattern(this.legalEntityNamePattern),
            ]);
            this.form.eik = new FormControl<string | null>(null);
            this.form.empoweredUids = new FormArray(
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
            );
        } else {
            this.form.authorizer = new FormControl<string | null>(null);
        }

        this.filterForm = new FormGroup(this.form);
        if (this.isFromMePage) {
            this.filterForm.get('legalEntityName')?.disable();
            this.filterForm.get('eik')?.disable();

            this.filterForm.controls['eik'].valueChanges.subscribe(value => {
                if (value) {
                    this.filterForm.controls['eik'].setValidators(BulstatValidator());
                } else {
                    this.filterForm.controls['eik'].clearValidators();
                }
                this.filterForm.controls['eik'].updateValueAndValidity({ emitEvent: false });
            });
        }

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
                    } else if (this.empoweredUids.controls[index].controls['uidType'].value === IdentifierType.LNCh) {
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
        });
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
            this.filterForm.get('eik')?.enable();
        } else {
            this.filterForm.get('legalEntityName')?.disable();
            this.filterForm.get('legalEntityName')?.setValue(null);
            this.filterForm.get('eik')?.disable();
            this.filterForm.get('eik')?.setValue(null);
        }
    }

    openFilter() {
        this.visible = true;
        // Save state
        this.filterFormUnSavedState = this.filterForm.value;
        if (this.filterForm.get('legalEntityName')) {
            this.filterFormUnSavedState.legalEntityName = this.filterForm.get('legalEntityName')?.value;
        }
        if (this.filterForm.get('eik')) {
            this.filterFormUnSavedState.eik = this.filterForm.get('eik')?.value;
        }
    }

    closeFilter() {
        this.visible = false;
        // Use state
        if (
            this.isFromMePage &&
            !Object.prototype.hasOwnProperty.call(this.filterFormUnSavedState, 'legalEntityName')
        ) {
            this.filterFormUnSavedState.legalEntityName = null;
        }
        if (this.isFromMePage && !Object.prototype.hasOwnProperty.call(this.filterFormUnSavedState, 'eik')) {
            this.filterFormUnSavedState.eik = null;
        }
        this.filterForm.setValue(this.filterFormUnSavedState);
    }

    onApplicantClear() {
        this.filterForm.get('legalEntityName')?.setValue(null);
        this.filterForm.get('eik')?.setValue(null);
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
                this.chosenFilters.filter((chosenFilter, index, chosenFilters) => {
                    // If filter is preselected
                    if (formValue[0] === chosenFilter.key) {
                        // Update the value
                        chosenFilter.value = formValue[1];

                        // If user choose individual option in onBehalfOf we should ensure
                        // that legalEntityName is cleaned if it was populated before
                        if (formValue[0] === 'onBehalfOf' && formValue[1] === OnBehalfOf.Individual) {
                            this.removeLegalEntityName();
                            this.removeLegalEntityEik();
                        }

                        // If current value is null | empty string | boolean false, then remove it from the chosenFilters array
                        if (!this.performDataTypeCheck(formValue[1])) {
                            chosenFilters.splice(index, 1);
                            // If onBehalfOf is null then remove legalEntityName if it is not empty
                            if (formValue[0] === 'onBehalfOf') {
                                this.removeLegalEntityName();
                                this.removeLegalEntityEik();
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

                        // If current filter is not preselected in the chosenFilters array but its value is not null it should be added
                    } else if (
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
                        chosenFilters.push(preparedObject);
                    }
                });
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

    removeLegalEntityEik() {
        const legalEntityEikIndex = this.chosenFilters.findIndex((cF: any) => cF.key === 'eik');
        if (
            this.chosenFilters &&
            this.chosenFilters[legalEntityEikIndex] &&
            this.chosenFilters[legalEntityEikIndex].value &&
            this.chosenFilters[legalEntityEikIndex].value.length > 0
        ) {
            this.chosenFilters.splice(legalEntityEikIndex, 1);
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
        if (Array.isArray(filteredValue.value)) {
            (this.filterForm.controls[filteredValue.key] as FormArray).clear();
            this.addEmpoweredUid();
            if (this.empoweredUids) {
                this.empoweredUids.markAsPristine();
            }
        } else {
            (this.filterForm.controls as any)[filteredValue.key].setValue(null);

            // Hide the legalEntityName control when onBehalfOf control is cleared
            if (filteredValue.key === 'onBehalfOf' && filteredValue.value === OnBehalfOf.LegalEntity) {
                this.filterForm.get('legalEntityName')?.setValue(null);
                this.filterForm.get('legalEntityName')?.disable();
                this.removeLegalEntityName();

                this.filterForm.get('eik')?.setValue(null);
                this.filterForm.get('eik')?.disable();
                this.removeLegalEntityEik();
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
        if (this.isFromMePage) {
            this.filterForm.get('legalEntityName')?.disable();
            this.filterForm.get('eik')?.disable();
        }
        this.filterFormUnSavedState = this.filterForm.value;
        this.filteredData.emit(null);
    }

    refreshData() {
        this.filteredData.emit(this.extractValues());
    }

    prepareFilteredValueLabel(filteredValue: any): string {
        if (filteredValue.key === 'validToDate') {
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
}
