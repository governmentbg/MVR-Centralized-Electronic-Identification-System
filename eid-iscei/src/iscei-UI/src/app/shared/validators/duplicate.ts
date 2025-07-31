import { FormArray, ValidationErrors } from '@angular/forms';

export function duplicate(): any {
    return (formArray: FormArray): ValidationErrors | null => {
        const totalSelected = formArray.controls.map(control => control.value).filter(value => value);
        const hasDuplicate = totalSelected.some((value, index) => totalSelected.indexOf(value, index + 1) != -1);
        return hasDuplicate ? { duplicate: true } : null;
    };
}

export function formArrayFormGroupDuplicateValidator(): any {
    return (formArray: FormArray): ValidationErrors | null => {
        const totalSelected = formArray.controls
            .map((formGroup: any) => formGroup.controls.uid)
            .map(control => control.value)
            .filter(value => value);
        const hasDuplicate = totalSelected.some((value, index) => {
            return totalSelected.indexOf(value, index + 1) != -1;
        });
        return hasDuplicate ? { duplicateValidator: true } : null;
    };
}
