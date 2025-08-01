import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import * as moment from 'moment';

export function PinOrEGNValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (
            !controlValue ||
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
            !controlValue ||
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
        if (!controlValue || (controlValue && ValidatorEGN.validate(controlValue))) {
            return null;
        } else {
            return { EgnError: controlValue };
        }
    };
}

export function EGNAdultValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (!controlValue || (controlValue && ValidatorEGN.isAdult(controlValue))) {
            return null;
        } else {
            return { EgnAdultError: controlValue };
        }
    };
}

export function PinValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const controlValue = control.value?.toString();
        if (!controlValue || (controlValue && PersonalIdentificationNumber.validate(controlValue))) {
            return null;
        } else {
            return { PinError: controlValue };
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
    const egnToDate = moment(`${year}/${month}/${controlValueDate}`, 'YYYY/MM/DD').startOf('day');

    if (egnToDate.isValid()) {
        return egnToDate.toDate();
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

const ValidatorEGN = (function () {
    function validate(egn: string) {
        const checkSum = computeCheckSum(egn);

        return isValidLength(egn) && containsIntegerOnly(egn) && getDateByEGN(egn) && isValidCheckSum(checkSum, egn);
    }

    function isAdult(egn: string) {
        const eighteenYearsAgo = moment().subtract(18, 'years').startOf('day').toDate();
        const egnToDate = getDateByEGN(egn);

        if (egnToDate) {
            return egnToDate <= eighteenYearsAgo;
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
