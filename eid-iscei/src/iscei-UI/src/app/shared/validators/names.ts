import { AbstractControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
const patternLastName = /^(?=.{1,60}$)[а-яА-Я\s-']*$/;
const patternLastNameLatin = /^(?=.{1,60}$)[a-zA-Z\s-']*$/;
export function firstLetterCapitalValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
        const value: string = control.value as string;
        if (value && value.length > 0) {
            const firstLetter = value.charAt(0);
            if (firstLetter !== firstLetter.toUpperCase()) {
                return { firstLetterNotCapital: true };
            }
        }
        return null;
    };
}

export function requiredNamesValidator(formGroup: FormGroup) {
    const lastName = formGroup.get('lastName');
    const secondName = formGroup.get('secondName')?.value;
    if (!secondName && !lastName?.value) {
        lastName?.setValidators([
            Validators.required,
            Validators.pattern(patternLastName),
            firstLetterCapitalValidator(),
        ]);
        lastName?.updateValueAndValidity({ onlySelf: true });
        return null;
    } else if (secondName) {
        const secondNameValue = secondName.trim();
        if (secondNameValue) {
            lastName?.setValidators([Validators.pattern(patternLastName), firstLetterCapitalValidator()]);
            if (!lastName?.value) {
                lastName?.markAsPristine();
            }
            lastName?.updateValueAndValidity({ onlySelf: true });
        } else {
            lastName?.setValidators([
                Validators.required,
                Validators.pattern(patternLastName),
                firstLetterCapitalValidator(),
            ]);
            lastName?.updateValueAndValidity({ onlySelf: true });
        }
    }

    return null;
}

export function requiredLatinNamesValidator(formGroup: FormGroup) {
    const lastName = formGroup.get('lastNameLatin');
    const secondName = formGroup.get('secondNameLatin')?.value;
    if (!secondName && !lastName?.value) {
        lastName?.setValidators([
            Validators.required,
            Validators.pattern(patternLastNameLatin),
            firstLetterCapitalValidator(),
        ]);
        lastName?.updateValueAndValidity({ onlySelf: true });
        return null;
    } else if (secondName) {
        const secondNameValue = secondName.trim();
        if (secondNameValue) {
            lastName?.setValidators([Validators.pattern(patternLastNameLatin), firstLetterCapitalValidator()]);
            if (!lastName?.value) {
                lastName?.markAsPristine();
            }
            lastName?.updateValueAndValidity({ onlySelf: true });
        } else {
            lastName?.setValidators([
                Validators.required,
                Validators.pattern(patternLastNameLatin),
                firstLetterCapitalValidator(),
            ]);
            lastName?.updateValueAndValidity({ onlySelf: true });
        }
    }

    return null;
}
