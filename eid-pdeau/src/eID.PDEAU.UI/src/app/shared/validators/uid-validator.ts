import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { isBefore, isValid, startOfDay, subYears } from 'date-fns';

let _control: AbstractControl;

export function PinOrEGNOrBulstatValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        _control = control;
        const controlValue = control.value?.toString();
        if (
            (controlValue && ValidatorEGN.validate(controlValue)) ||
            (controlValue && PersonalIdentificationNumber.validate(controlValue)) ||
            (controlValue && Bulstat.validate(controlValue))
        ) {
            return null;
        } else {
            return { pinOrEgnOrBulstatError: controlValue };
        }
    };
}

export function PinOrEGNValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (
            (controlValue && ValidatorEGN.validate(controlValue)) ||
            (controlValue && PersonalIdentificationNumber.validate(controlValue))
        ) {
            return null;
        } else {
            return { pinOrEgnError: controlValue };
        }
    };
}

export function PinOrAdultEGNValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (
            (controlValue && ValidatorEGN.validate(controlValue) && ValidatorEGN.isAdult(controlValue)) ||
            (controlValue && PersonalIdentificationNumber.validate(controlValue))
        ) {
            return null;
        } else {
            if (controlValue && ValidatorEGN.validate(controlValue) && !ValidatorEGN.isAdult(controlValue)) {
                return { EgnAdultError: controlValue };
            } else {
                return { pinOrEgnError: controlValue };
            }
        }
    };
}

export function EGNValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (controlValue && ValidatorEGN.validate(controlValue)) {
            return null;
        } else {
            return { EgnError: controlValue };
        }
    };
}

export function EGNAdultValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (controlValue && ValidatorEGN.isAdult(controlValue)) {
            return null;
        } else {
            return { EgnAdultError: controlValue };
        }
    };
}

function isValidCheckSum(checkSum: number, pin: string) {
    return checkSum === Number(pin.charAt(9));
}

function isValidLength(pin: string) {
    return pin.length === 10;
}

function containsIntegerOnly(pin: string) {
    return /[0-9]/.test(pin);
}

export function getDateByEGN(egn: string): Date | null {
    if (egn.length !== 10) {
        return null;
    }
    const controlValueYear = parseInt(egn.slice(0, 2), 10);
    const controlValueMonth = parseInt(egn.slice(2, 4), 10);
    const controlValueDate = egn.slice(4, 6);

    let month = controlValueMonth;
    let baseYear = 1900;
    if (controlValueMonth > 40) {
        month -= 40;
        baseYear = 2000;
    } else if (controlValueMonth > 20) {
        baseYear = 1800;
        month -= 20;
    }
    const year = baseYear + controlValueYear;
    const egnToDate = startOfDay(new Date(`${year}/${month}/${controlValueDate}`));

    if (isValid(egnToDate)) {
        return egnToDate;
    } else {
        return null;
    }
}

export function isAdult(egn: string) {
    return ValidatorEGN.isAdult(egn);
}

export function isLNCH(value: string) {
    return PersonalIdentificationNumber.validate(value);
}

export function PinValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (controlValue && PersonalIdentificationNumber.validate(controlValue)) {
            return null;
        } else {
            return { PinError: controlValue };
        }
    };
}

const ValidatorEGN = (function () {
    function validate(egn: string) {
        const checkSum = computeCheckSum(egn);

        return isValidLength(egn) && containsIntegerOnly(egn) && getDateByEGN(egn) && isValidCheckSum(checkSum, egn);
    }

    function isAdult(egn: string) {
        const eighteenYearsAgo = startOfDay(subYears(new Date(), 18));
        const egnToDate = getDateByEGN(egn);

        if (egnToDate) {
            return isBefore(egnToDate, eighteenYearsAgo);
        } else {
            return false;
        }
    }

    function computeCheckSum(egn: string) {
        let checkSum = 0;
        const weights = [2, 4, 8, 5, 10, 9, 7, 3, 6];

        for (let i = 0; i < weights.length; ++i) {
            checkSum += weights[i] * Number(egn.charAt(i));
        }

        checkSum %= 11;
        checkSum %= 10;

        return checkSum;
    }

    return {
        validate: validate,
        isAdult: isAdult,
    };
})();

const PersonalIdentificationNumber = (function () {
    function validate(pin: string) {
        const checkSum = computeCheckSum(pin);

        return isValidLength(pin) && containsIntegerOnly(pin) && isValidCheckSum(checkSum, pin);
    }

    function computeCheckSum(pin: string) {
        let checkSum = 0;
        const weights = [21, 19, 17, 13, 11, 9, 7, 3, 1];

        for (let i = 0; i < weights.length; ++i) {
            checkSum += weights[i] * Number(pin.charAt(i));
        }

        // Ако полученият остатък от checkSum е число по-малко от 10, то става контролно число.
        // Ако е равно на 10, контролното число е 0
        checkSum %= 10;
        return checkSum;
    }

    return {
        validate: validate,
    };
})();

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

function isValidBulstatCheckSum(checkSum: number, bulstat: string) {
    if (bulstat.length === 9) {
        return checkSum === Number(bulstat.charAt(8));
    }
    if (bulstat.length === 10) {
        // If the BULSTAT is 10 digits it is identical to EGN
        return EGNValidator()(_control) === null && EGNAdultValidator()(_control) === null;
    } else {
        return checkSum === Number(bulstat.charAt(12));
    }
}

function isValidBulstatLength(pin: string) {
    return pin.length === 9 || pin.length === 10 || pin.length === 13;
}

export const Bulstat = (function () {
    function validate(bulstat: string) {
        const checkSum = computeCheckSum(bulstat);

        return (
            isValidBulstatLength(bulstat) && containsIntegerOnly(bulstat) && isValidBulstatCheckSum(checkSum, bulstat)
        );
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
