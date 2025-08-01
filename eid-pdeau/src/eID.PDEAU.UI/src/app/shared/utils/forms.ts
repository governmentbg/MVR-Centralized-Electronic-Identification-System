import { FormArray, FormGroup } from '@angular/forms';

export function markFormGroupAsDirty(formGroup: FormGroup | FormArray) {
    Object.values(formGroup.controls).forEach(control => {
        if (control instanceof FormArray) {
            return markFormGroupAsDirty(control);
        }
        control.markAsDirty();
        control.updateValueAndValidity();
        if (control instanceof FormGroup) {
            markFormGroupAsDirty(control);
        }
    });
}
