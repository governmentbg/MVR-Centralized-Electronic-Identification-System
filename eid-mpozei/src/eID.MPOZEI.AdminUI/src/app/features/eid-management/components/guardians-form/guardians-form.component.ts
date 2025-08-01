import { Component, OnDestroy } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { dateMoreThanValidate } from '../../../../shared/validators/date';
import { DeviceType, PersonalIdTypes } from '../../enums/eid-management.enum';
import { TranslocoService } from '@ngneat/transloco';
import { ToastService } from '../../../../shared/services/toast.service';
import { EGNAdultValidator, EGNValidator, PinValidator } from '../../../../shared/validators/egn';
import { Subscription } from 'rxjs';
import { IGuardingKeysForm } from '../../interfaces/eid-management.interfaces';

@Component({
    selector: 'app-guardians-form',
    templateUrl: './guardians-form.component.html',
    styleUrls: ['./guardians-form.component.scss'],
})
export class GuardiansFormComponent implements OnDestroy {
    constructor(public translateService: TranslocoService, private toastService: ToastService) {
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
            this.identityDocumentTypes = [
                {
                    id: DeviceType.CHIP_CARD,
                    name: this.translateService.translate('modules.eidManagement.txtIdCard'),
                },
            ];
        });
    }

    maxGuardiansCount = 5;
    languageChangeSubscription: Subscription;
    identityDocumentTypes = [
        {
            id: DeviceType.CHIP_CARD,
            name: this.translateService.translate('modules.eidManagement.txtIdCard'),
        },
    ];
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
    cyrillicPattern = /^[аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ]+$/;
    form = new FormGroup({
        array: new FormArray<FormGroup<IGuardingKeysForm>>([]),
    });

    createItem(): FormGroup<IGuardingKeysForm> {
        const controls: IGuardingKeysForm = {
            firstName: new FormControl<string | null>(null, [
                Validators.required,
                Validators.pattern(this.cyrillicPattern),
            ]),
            secondName: new FormControl<string | null>(null, [
                Validators.required,
                Validators.pattern(this.cyrillicPattern),
            ]),
            lastName: new FormControl<string | null>(null, [
                Validators.required,
                Validators.pattern(this.cyrillicPattern),
            ]),
            citizenship: new FormControl<string | null>(null, Validators.required),
            citizenIdentifierType: new FormControl<PersonalIdTypes | null>(
                this.personalIdTypes[0].id,
                Validators.required
            ),
            citizenIdentifierNumber: new FormControl<string | null>(null, [
                Validators.required,
                EGNValidator(),
                EGNAdultValidator(),
            ]),
            personalIdentityDocument: new FormGroup({
                identityType: new FormControl<string | null>(this.identityDocumentTypes[0].id, Validators.required),
                identityNumber: new FormControl<string | null>(null, Validators.required),
                identityIssueDate: new FormControl<Date | null>(null, Validators.required),
                identityIssuer: new FormControl<string | null>(null, Validators.required),
                identityValidityToDate: new FormControl<Date | null>(null, [
                    Validators.required,
                    dateMoreThanValidate('identityIssueDate'),
                ]),
            }),
        };
        const formGroup = new FormGroup(controls);
        formGroup.controls.personalIdentityDocument.get('identityIssueDate')?.valueChanges.subscribe({
            next: () => {
                formGroup.controls.personalIdentityDocument.get('identityValidityToDate')?.updateValueAndValidity();
            },
        });
        formGroup.get('citizenIdentifierType')?.valueChanges.subscribe({
            next: (controlValue: PersonalIdTypes) => {
                switch (controlValue) {
                    case PersonalIdTypes.EGN:
                        formGroup.get('citizenIdentifierNumber')?.setValidators([EGNAdultValidator(), EGNValidator()]);
                        formGroup.get('citizenIdentifierNumber')?.updateValueAndValidity();
                        break;
                    case PersonalIdTypes.LNCH:
                        formGroup.get('citizenIdentifierNumber')?.setValidators([PinValidator()]);
                        formGroup.get('citizenIdentifierNumber')?.updateValueAndValidity();
                        break;
                }
            },
        });
        return formGroup;
    }

    addItem() {
        const items: FormArray = this.form.controls.array;
        if (items.length < this.maxGuardiansCount) {
            items.push(this.createItem());
        }
    }

    removeItem(index: number) {
        const items: FormArray = this.form.controls.array;
        items.removeAt(index);
    }

    personalIdTypeChange(formGroup: FormGroup) {
        switch (formGroup.controls['personalIdType'].value) {
            case PersonalIdTypes.EGN:
                formGroup.controls['personalId'].setValidators([EGNValidator()]);
                formGroup.controls['personalId'].updateValueAndValidity();
                break;
            case PersonalIdTypes.LNCH:
                formGroup.controls['personalId'].setValidators([PinValidator()]);
                formGroup.controls['personalId'].updateValueAndValidity();
                break;
        }
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }
}
