import { AbstractControl, ValidationErrors } from '@angular/forms';
import * as defaultMoment from 'moment';

export function dateMoreThanValidate(): any {
    let moment: any;
    if ('default' in defaultMoment) {
        moment = defaultMoment['default'];
        return (control: AbstractControl): ValidationErrors | null => {
            if (!control.parent || control.value === null) {
                return null;
            }
            const toDateControl = control.parent.get(
                'toDate'
            ) as AbstractControl;
            const fromDateControl = control.parent.get(
                'fromDate'
            ) as AbstractControl;

            return toDateControl.value !== null &&
                fromDateControl.value !== null &&
                moment(fromDateControl.value).isBefore(toDateControl.value)
                ? null
                : { dateMoreThanValid: true };
        };
    }
}

export function dateIsBeforeOtherDate(): any {
    let moment: any;
    if ('default' in defaultMoment) {
        moment = defaultMoment['default'];
        return (control: AbstractControl): ValidationErrors | null => {
            if (!control.parent || control.value === null) {
                return null;
            }
            const toDateControl = control.parent.get(
                'toDate'
            ) as AbstractControl;
            const fromDateControl = control.parent.get(
                'fromDate'
            ) as AbstractControl;
            
            return moment(fromDateControl.value).isBefore(toDateControl.value)
                ? null
                : { dateIsBeforeOtherDate: true };
        };
    }
}

export function notMoreThanThreeMonths(): any {
    let moment: any;
    if ('default' in defaultMoment) {
        moment = defaultMoment['default'];
        return (control: AbstractControl): ValidationErrors | null => {
            if (!control.parent || control.value === null) {
                return null;
            }
            const toDateControl = control.parent.get(
                'toDate'
            ) as AbstractControl;
            const fromDateControl = control.parent.get(
                'fromDate'
            ) as AbstractControl;

            const checkDifferenceMonths = moment(toDateControl.value).diff(
                moment(fromDateControl.value),
                'months'
            );

            let threeMonthsDifference = true;
            if (checkDifferenceMonths > 0) {
                threeMonthsDifference =
                    checkDifferenceMonths >= 0 && checkDifferenceMonths <= 2;
            }
            return toDateControl.value !== null &&
                fromDateControl.value !== null &&
                threeMonthsDifference
                ? null
                : { dateNotMoreThanThreeMonths: true };
        };
    }
}
