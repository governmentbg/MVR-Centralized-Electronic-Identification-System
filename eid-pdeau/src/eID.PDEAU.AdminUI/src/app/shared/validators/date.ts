import { AbstractControl, ValidationErrors } from '@angular/forms';

export function dateMoreThanValidate(control2Name: string): any {
    return (control: AbstractControl): ValidationErrors | null => {
        if (!control.parent || control.value === null) {
            return null;
        }
        const control1 = control;
        const control2 = control.parent.get(control2Name) as AbstractControl;
        return control1.value !== null && control2.value !== null && control2.value < control1.value
            ? null
            : { dateMoreThanValid: true };
    };
}

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
