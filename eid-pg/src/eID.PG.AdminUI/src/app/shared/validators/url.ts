import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
const urlRegExPattern =
    '(https?://(?:www.|(?!www))[a-zA-Z0-9]|www.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9].[^s]{2,}|https?://(?:www.|(?!www))[a-zA-Z0-9]|www.*)';

export function URLValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (controlValue && controlValue.length > 0) {
            if (IsValidUrl(controlValue)) {
                return null;
            } else {
                return { urlError: true };
            }
        } else {
            return null;
        }
    };
}

function IsValidUrl(urlString: string) {
    try {
        const pattern = new RegExp(urlRegExPattern, 'mg');
        const valid = pattern.test(urlString);
        return valid;
    } catch (TypeError) {
        return false;
    }
}
