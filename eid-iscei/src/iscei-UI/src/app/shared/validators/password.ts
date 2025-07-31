import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

export function passwordValidator() {
    return Validators.pattern(
        /^(?=(?:.*[A-Za-z]){2})(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()\-_=+~`\\|[\]{};:'",.<>/?]).{8,}$/
    );
}

export function matchPassword(formGroup: FormGroup) {
    const password = formGroup.get('baseProfilePassword')?.value;
    const confirm = formGroup.get('matchingPassword')?.value;

    if (password !== confirm) {
        formGroup.get('matchingPassword')?.setErrors({ passwordMismatch: true });
        return { passwordMismatch: true };
    }

    return null;
}

export function matchPasswordUpdate(formGroup: FormGroup) {
    const password = formGroup.get('newPassword')?.value;
    const confirm = formGroup.get('confirmPassword')?.value;

    if (password !== confirm) {
        formGroup.get('confirmPassword')?.setErrors({ passwordMismatch: true });
        return { passwordMismatch: true };
    }

    return null;
}

export function matchPasswordLandingPage(formGroup: FormGroup) {
    const password = formGroup.get('password')?.value;
    const confirm = formGroup.get('confirmPassword')?.value;

    if (password !== confirm) {
        formGroup.get('confirmPassword')?.setErrors({ passwordMismatch: true });
        return { passwordMismatch: true };
    }

    return null;
}

export function newPasswordValidator(oldPasswordControlName: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const formGroup = control.parent as FormGroup;
        if (!formGroup) {
            return null;
        }

        const oldPassword = formGroup.get(oldPasswordControlName)?.value;
        const newPassword = control.value;

        if (oldPassword && newPassword && oldPassword === newPassword) {
            return { sameAsOldPassword: true };
        }
        return null;
    };
}
