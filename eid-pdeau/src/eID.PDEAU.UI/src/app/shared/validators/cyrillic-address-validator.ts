import { AbstractControl, ValidatorFn } from '@angular/forms';

const cyrillicPattern = /^[аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ \-.,'"0-9]+$/;

export function cyrillicAddressValidator(): ValidatorFn {
    return (control: AbstractControl) => {
        const isValid = cyrillicPattern.test(control.value);

        if (isValid) return null;

        return { cyrillic: true };
    };
}
