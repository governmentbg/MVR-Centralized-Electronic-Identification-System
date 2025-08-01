import { Component, Input, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { IdentifierType } from '@app/shared/enums';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { EGNAdultValidator, EGNValidator, PinValidator } from '@app/shared/validators/uid-validator';
import { cyrillicValidator } from '@app/shared/validators/cyrillic-validator';
import { createInputSanitizer } from '@app/shared/utils/create-input-sanitized';
import { EmployeeForm } from '@app/features/administrators/administrators.dto';

@Component({
    selector: 'app-employees-list',
    templateUrl: './employees-list.component.html',
    styleUrls: ['./employees-list.component.scss'],
})
export class EmployeesListComponent implements OnInit {
    constructor(private translateService: TranslocoService, private formBuilder: FormBuilder) {}

    @Input() form!: FormArray<FormGroup<EmployeeForm>>;

    showDialog = false;
    latinNamePattern = /^(?=.)[a-zA-Z\s']*$/;
    employeeForm = new FormGroup({
        citizenIdentifierType: new FormControl(IdentifierType.EGN, {
            validators: [Validators.required],
            nonNullable: true,
        }),
        citizenIdentifierNumber: new FormControl('', {
            validators: [Validators.required, EGNAdultValidator(), EGNValidator()],
            nonNullable: true,
        }),
        name: new FormControl('', {
            validators: [Validators.required, Validators.maxLength(200), cyrillicValidator()],
            nonNullable: true,
        }),
        nameLatin: new FormControl('', {
            validators: [Validators.required, Validators.maxLength(200), Validators.pattern(this.latinNamePattern)],
            nonNullable: true,
        }),
        email: new FormControl('', { validators: [Validators.required, Validators.email], nonNullable: true }),
        isAdministrator: new FormControl(false),
        phoneNumber: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
        roles: new FormControl<string[]>([], { validators: [Validators.required], nonNullable: true }),
    });
    identifierTypes: { name: string; id: string }[] = [];
    roles: { name: string; value: string }[] = [];
    private subscriptions = new Subscription();

    ngOnInit() {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.identifierTypes = [
                    {
                        name: this.translateService.translate('enums.identifierType.EGN'),
                        id: IdentifierType.EGN,
                    },
                    {
                        name: this.translateService.translate('enums.identifierType.LNCh'),
                        id: IdentifierType.LNCh,
                    },
                ];

                this.roles = [
                    {
                        name: this.translateService.translate('enums.administratorsAndCentersRoles.administrator'),
                        value: 'ADMINISTRATOR',
                    },
                    {
                        name: this.translateService.translate('enums.administratorsAndCentersRoles.user'),
                        value: 'USER',
                    },
                ];
            })
        );
        this.employeeForm.controls.citizenIdentifierType.valueChanges.subscribe(value => {
            const validators = [Validators.required];
            if (value === IdentifierType.EGN) {
                validators.push(EGNValidator(), EGNAdultValidator());
            } else if (value === IdentifierType.LNCh) {
                validators.push(PinValidator());
            }
            const control = this.employeeForm.controls.citizenIdentifierNumber;
            control.setValidators(validators);
            control.updateValueAndValidity();
        });
    }

    onSubmit() {
        this.employeeForm.markAllAsTouched();

        markFormGroupAsDirty(this.employeeForm);
        if (this.employeeForm.invalid) return;

        const fb = new FormGroup<EmployeeForm>({
            email: new FormControl(this.employeeForm.controls.email.value, { nonNullable: true }),
            name: new FormControl(this.employeeForm.controls.name.value, { nonNullable: true }),
            phoneNumber: new FormControl(this.employeeForm.controls.phoneNumber.value, { nonNullable: true }),
            citizenIdentifierNumber: new FormControl(this.employeeForm.controls.citizenIdentifierNumber.value, {
                nonNullable: true,
            }),
            citizenIdentifierType: new FormControl(this.employeeForm.controls.citizenIdentifierType.value, {
                nonNullable: true,
            }),
            nameLatin: new FormControl(this.employeeForm.controls.nameLatin.value, { nonNullable: true }),
            roles: new FormControl(this.employeeForm.controls.roles.value, { nonNullable: true }),
        });

        this.form.push(fb);
        this.employeeForm.reset();
        this.onClosePreview();
    }

    removeUser(index: number) {
        this.form.removeAt(index);
    }

    onClosePreview() {
        this.showDialog = false;
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    sanitizeNameInput = createInputSanitizer(this.employeeForm.controls.name);
}
