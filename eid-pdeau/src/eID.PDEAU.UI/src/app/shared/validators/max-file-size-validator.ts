import { AbstractControl, ValidationErrors } from '@angular/forms';
/**
    @param maxSize - Maximum file size in megabytes.
*/
export function maxFileSizeValidator(maxSize: number) {
    const maxSizeInBytes = maxSize * 1024 * 1024; // Convert megabytes to bytes
    return (control: AbstractControl): ValidationErrors | null => {
        const files = control.value as FileList;

        for (let i = 0; i < files?.length; i++) {
            if (files[i].size > maxSizeInBytes) {
                return {
                    maxFileSize: {
                        maxFileSize: maxSize,
                    },
                };
            }
        }

        return null; // Valid if file size is within the limit
    };
}
