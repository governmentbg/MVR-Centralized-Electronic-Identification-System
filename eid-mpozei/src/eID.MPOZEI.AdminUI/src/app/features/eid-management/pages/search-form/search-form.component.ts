import { Component, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { PersonalIdTypes } from '../../enums/eid-management.enum';
import { Router } from '@angular/router';
import { EGNValidator, PinValidator } from '../../../../shared/validators/egn';

@Component({
    selector: 'app-search-form',
    templateUrl: './search-form.component.html',
    styleUrls: ['./search-form.component.scss'],
})
export class SearchFormComponent implements OnDestroy {
    constructor(public translateService: TranslocoService, private router: Router) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.personalIdTypes = [
                {
                    name: this.translateService.translate('modules.eidManagement.txtEGN'),
                    id: PersonalIdTypes.EGN,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.txtLNCH'),
                    id: PersonalIdTypes.LNCH,
                },
            ];
        });
    }

    languageChangeSubscription: Subscription;
    requestInProgress = false;
    personalIdTypes = [
        {
            name: this.translateService.translate('modules.eidManagement.txtEGN'),
            id: PersonalIdTypes.EGN,
        },
        {
            name: this.translateService.translate('modules.eidManagement.txtLNCH'),
            id: PersonalIdTypes.LNCH,
        },
    ];
    submitted = false;
    form = new FormGroup({
        type: new FormControl<PersonalIdTypes | null>(this.personalIdTypes[0].id, Validators.required),
        personalId: new FormControl<string | null>(null, [Validators.required, EGNValidator()]),
        personalDocumentNumber: new FormControl<string | null>(null, [Validators.required]),
    });
    PersonalIdTypes = PersonalIdTypes;

    onSubmit() {
        this.submitted = true;
        this.form.markAllAsTouched();
        Object.values(this.form.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    innerControl.markAsDirty();
                });
            } else {
                control.markAsDirty();
            }
        });
        if (this.form.valid) {
            this.requestInProgress = true;
            this.router.navigate(['/eid-management/search/results'], {
                state: {
                    personalId: this.form.controls.personalId.value,
                    personalIdType: this.form.controls.type.value,
                    personalDocumentNumber: this.form.controls.personalDocumentNumber.value,
                },
            });
            if (this.form.controls.personalDocumentNumber.value) {
                sessionStorage.setItem('personalDocumentNumber', this.form.controls.personalDocumentNumber.value);
            } else {
                sessionStorage.removeItem('personalDocumentNumber');
            }
        }
    }

    personalIdTypeChange() {
        this.form.controls.personalId.reset();
        this.form.controls.personalDocumentNumber.reset();
        switch (this.form.controls.type.value) {
            case PersonalIdTypes.EGN:
                this.form.controls.personalId.setValidators([Validators.required, EGNValidator()]);
                this.form.controls.personalId.updateValueAndValidity();
                this.form.controls.personalDocumentNumber.setValidators([Validators.required]);
                this.form.controls.personalDocumentNumber.updateValueAndValidity();
                break;
            case PersonalIdTypes.LNCH:
                this.form.controls.personalId.setValidators([Validators.required, PinValidator()]);
                this.form.controls.personalId.updateValueAndValidity();
                this.form.controls.personalDocumentNumber.setValidators([]);
                this.form.controls.personalDocumentNumber.updateValueAndValidity();
                break;
        }
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }
}
