import { AbstractControl } from '@angular/forms';

export function createInputSanitizer(control: AbstractControl) {
    return () => {
        const sanitizedValue = sanitizeNameInput(control.value);
        control.setValue(sanitizedValue, { emitEvent: false });
    };
}

function sanitizeNameInput(value: string): string {
    // Trim leading and trailing spaces
    let sanitizedValue = value.trim();
    if (!sanitizedValue) return '';

    // Capitalize the first letter of each word
    sanitizedValue = sanitizedValue
        .split(' ')
        .map(word => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase())
        .join(' ');

    // Replace multiple spaces between words with a single space
    sanitizedValue = sanitizedValue.replace(/\s+/g, ' ');

    return sanitizedValue;
}
