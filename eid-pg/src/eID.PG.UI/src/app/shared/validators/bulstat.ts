import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { PinOrAdultEGNValidator } from './egn';

let _control: AbstractControl;

export function BulstatValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        _control = control;
        const controlValue = control.value?.toString();
        if (controlValue && Bulstat.validate(controlValue)) {
            return null;
        } else {
            return { bulstatError: controlValue };
        }
    };
}

function isValidCheckSum(checkSum: number, bulstat: string) {
    if (bulstat.length === 9) {
        return checkSum === Number(bulstat.charAt(8));
    }
    if (bulstat.length === 10) {
        // If the BULSTAT is 10 digits it must be valid EGN or LNCH
        return PinOrAdultEGNValidator()(_control) === null;
    } else {
        return checkSum === Number(bulstat.charAt(12));
    }
}

function isValidLength(pin: string) {
    return pin.length === 9 || pin.length === 10 || pin.length === 13;
}

function containsIntegerOnly(pin: string) {
    return /[0-9]/.test(pin);
}

export const Bulstat = (function () {
    function validate(bulstat: string) {
        const checkSum = computeCheckSum(bulstat);

        return isValidLength(bulstat) && containsIntegerOnly(bulstat) && isValidCheckSum(checkSum, bulstat);
    }

    function computeCheckSum(value: string) {
        const controlNumbers9_1 = [1, 2, 3, 4, 5, 6, 7, 8];
        const controlNumbers9_2 = [3, 4, 5, 6, 7, 8, 9, 10];
        const controlNumbers13_1 = [2, 7, 3, 5];
        const controlNumbers13_2 = [4, 9, 5, 7];

        const val = [];

        for (let i = 0; i < value.length; i++) {
            val[i] = ~~value.charAt(i);
        }

        let sum = 0;
        for (let i = 0; i < 8; i++) {
            sum += val[i] * controlNumbers9_1[i];
        }

        let checkSum = sum % 11;
        if (checkSum > 9) {
            sum = 0;
            for (let i = 0; i < 8; i++) {
                sum += val[i] * controlNumbers9_2[i];
            }

            checkSum = sum % 11;
            checkSum = checkSum > 9 ? 0 : checkSum;
        }

        if (value.length === 9) {
            return checkSum;
        }

        // use 9-digit BULSTAT
        val[8] = checkSum;

        sum = 0;
        for (let i = 8; i < 12; i++) {
            sum += val[i] * controlNumbers13_1[i - 8];
        }

        checkSum = sum % 11;
        if (checkSum > 9) {
            sum = 0;
            for (let i = 8; i < 12; i++) {
                sum += val[i] * controlNumbers13_2[i - 8];
            }

            checkSum = sum % 11;
            checkSum = checkSum > 9 ? 0 : checkSum;
        }

        return checkSum;
    }

    return {
        validate: validate,
    };
})();
