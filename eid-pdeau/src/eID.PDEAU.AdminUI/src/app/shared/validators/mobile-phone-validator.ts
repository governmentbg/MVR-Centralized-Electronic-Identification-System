import { AbstractControl, ValidatorFn } from '@angular/forms';
const phoneNumber = /^\+\d{6,15}$/;

export function mobilePhoneValidator(): ValidatorFn {
    return (control: AbstractControl) => {
        const isValid = phoneNumber.test(control.value);

        if (isValid) return null;

        return { phone: true };
    };
}
