import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IdentifierType } from '@app/shared/enums';
import { EGNAdultValidator, EGNValidator, PinValidator } from '@app/shared/validators/uid-validator';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-administrator-form',
    templateUrl: './administrator-form.component.html',
    styleUrls: ['./administrator-form.component.scss'],
})
export class AdministratorFormComponent implements OnInit {
    @Input() form!: FormGroup<{
        uidType: FormControl<IdentifierType | null>;
        uid: FormControl<string | null>;
        name: FormControl<string | null>;
        email: FormControl<string | null>;
        phone: FormControl<string | null>;
    }>;

    constructor(private translateService: TranslocoService) {}

    identifierTypes: { name: string; id: string }[] = [];

    ngOnInit(): void {
        this.translateService.langChanges$.subscribe(() => {
            this.identifierTypes = [
                {
                    name: this.translateService.translate('modules.providers.register.EGN'),
                    id: IdentifierType.EGN,
                },
                {
                    name: this.translateService.translate('modules.providers.register.LNCh'),
                    id: IdentifierType.LNCh,
                },
            ];
        });
        this.form.controls.uidType.valueChanges.subscribe(value => {
            const validators = [Validators.required];
            if (value === IdentifierType.EGN) {
                validators.push(EGNValidator(), EGNAdultValidator());
            } else if (value === IdentifierType.LNCh) {
                validators.push(PinValidator());
            }
            const control = this.form.controls.uid;
            control.setValidators(validators);
            control.updateValueAndValidity();
        });
    }
}
