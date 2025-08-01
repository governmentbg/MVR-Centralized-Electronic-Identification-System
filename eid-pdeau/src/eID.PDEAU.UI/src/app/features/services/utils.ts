import { FormArray, FormControl, ValidationErrors } from '@angular/forms';

export function scopeDuplicateScopeValidator(): any {
    return (formControl: FormControl): ValidationErrors | null => {
        const value = formControl.value;

        const formArray = formControl.parent as FormArray;
        if (!formArray) return null;
        let result = null;
        formArray.controls.map(control => {
            if (control === formControl) return;
            if (control.value === value) {
                result = { duplicateScope: true };
            }
        });

        return result;
    };
}
