import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
const cyrillicPattern = /^[аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ \-']+$/;

export function cyrillicValidator(): ValidatorFn {
    return (control: AbstractControl) => {
        const isValid = cyrillicPattern.test(control.value);

        if (isValid) return null;

        return { cyrillic: true };
    };
}

export function twoCyrillicNamesWithUppercaseValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const value = control.value;
        if (!value) {
            return null; // Don't validate empty values to allow required validator to handle them
        }

        const names = value.trim().split(/\s+/);
        const uppercaseCyrillicNamesCount = names.filter((name: string) => cyrillicPattern.test(name)).length;

        return uppercaseCyrillicNamesCount >= 2 ? null : { twoCyrillicNamesWithUppercase: true };
    };
}
