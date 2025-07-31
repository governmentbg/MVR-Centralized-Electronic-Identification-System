import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import * as moment from 'moment';

export function dateMoreThanOrEqualValidate(control2Name: string): any {
    return (control: AbstractControl): ValidationErrors | null => {
        if (!control.parent || control.value === null) {
            return null;
        }
        const control1 = control;
        const control2 = control.parent.get(control2Name) as AbstractControl;
        return control1.value !== null && control2.value !== null && control2.value <= control1.value
            ? null
            : { dateMoreThanOrEqualValid: true };
    };
}

export function dateIsMinimumTodayValidator(options: { optional: boolean }): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();

        if (options && options.optional && control.value === null) {
            return null;
        } else {
            return new Date().setHours(0, 0, 0, 0) <= new Date(controlValue).setHours(0, 0, 0, 0)
                ? null
                : { dateIsMinimumToday: controlValue };
        }
    };
}

export function isDateBelowLawfulAge(control2Name: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        if (!control.parent) {
            return null;
        }

        const otherControl = control.parent.get(control2Name);

        if (!otherControl || otherControl.value === null) {
            return null;
        }

        const today = moment();
        const birthDate = moment(otherControl.value);

        if (!birthDate.isValid()) {
            return null;
        }

        const age = today.diff(birthDate, 'years');

        return age < 18 ? { belowLawfulAge: true } : null;
    };
}
