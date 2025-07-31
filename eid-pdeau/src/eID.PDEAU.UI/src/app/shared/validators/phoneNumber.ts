import { AbstractControl, ValidatorFn } from '@angular/forms';

export function bulgarianPhoneNumberValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
        const phoneNumber = control.value;
        if (!phoneNumber) {
            return null;
        }
        const isValid = /^(\+?359|0)(\d{9})$/.test(phoneNumber);

        return isValid ? null : { bulgarianPhoneNumber: { value: phoneNumber } };
    };
}
