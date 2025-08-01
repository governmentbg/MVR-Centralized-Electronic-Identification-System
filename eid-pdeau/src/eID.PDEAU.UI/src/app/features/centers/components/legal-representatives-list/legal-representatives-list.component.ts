import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { IdentifierType } from '@app/shared/enums';
import { EGNAdultValidator, EGNValidator, PinValidator } from '@app/shared/validators/uid-validator';
import { cyrillicValidator } from '@app/shared/validators/cyrillic-validator';
import { createInputSanitizer } from '@app/shared/utils/create-input-sanitized';
import { Subscription } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { PersonForm } from '@app/features/centers/centers.dto';

@Component({
    selector: 'app-legal-representatives-list',
    templateUrl: './legal-representatives-list.component.html',
    styleUrls: ['./legal-representatives-list.component.scss'],
})
export class LegalRepresentativesListComponent implements OnInit {
    constructor(private translateService: TranslocoService, private formBuilder: FormBuilder) {}

    @Input() form!: FormArray<FormGroup<PersonForm>>;

    showDialog = false;
    latinNamePattern = /^(?=.)[a-zA-Z\s']*$/;
    userForm = new FormGroup<PersonForm>({
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
        phoneNumber: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
    });
    identifierTypes: { name: string; id: string }[] = [];
    private subscriptions = new Subscription();
    sanitizeNameInput = createInputSanitizer(this.userForm.controls.name);

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
            })
        );
        this.userForm.controls.citizenIdentifierType.valueChanges.subscribe(value => {
            const validators = [Validators.required];
            if (value === IdentifierType.EGN) {
                validators.push(EGNValidator(), EGNAdultValidator());
            } else if (value === IdentifierType.LNCh) {
                validators.push(PinValidator());
            }
            const control = this.userForm.controls.citizenIdentifierNumber;
            control.setValidators(validators);
            control.updateValueAndValidity();
        });
    }

    onSubmit() {
        this.userForm.markAllAsTouched();

        markFormGroupAsDirty(this.userForm);
        if (this.userForm.invalid) return;

        const fb = new FormGroup<PersonForm>({
            email: new FormControl(this.userForm.controls.email.value, { nonNullable: true }),
            name: new FormControl(this.userForm.controls.name.value, { nonNullable: true }),
            phoneNumber: new FormControl(this.userForm.controls.phoneNumber.value, { nonNullable: true }),
            citizenIdentifierNumber: new FormControl(this.userForm.controls.citizenIdentifierNumber.value, {
                nonNullable: true,
            }),
            citizenIdentifierType: new FormControl(this.userForm.controls.citizenIdentifierType.value, {
                nonNullable: true,
            }),
            nameLatin: new FormControl(this.userForm.controls.nameLatin.value, { nonNullable: true }),
        });

        this.form.push(fb);
        this.userForm.reset();
        this.onClosePreview();
    }

    removeUser(index: number) {
        this.form.removeAt(index);
    }

    onClosePreview() {
        this.showDialog = false;
    }
}
