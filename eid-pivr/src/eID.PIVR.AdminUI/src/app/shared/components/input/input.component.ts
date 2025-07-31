import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'app-input',
    templateUrl: './input.component.html',
    styleUrls: ['./input.component.scss'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: forwardRef(() => InputComponent),
        },
    ],
})
export class InputComponent implements ControlValueAccessor {
    @Input() id = '';
    @Input() label = '';
    @Input() placeholder = '';
    @Input() autoFocus = false;
    @Input() isDisabled = false;
    @Input() inputType = 'text';

    value: any = '';

    onValueChange(newValue?: object | string) {
        if (newValue === undefined) {
            this.onChange(this.value);
        } else {
            this.onChange(newValue);
            this.value = newValue;
        }
    }

    // eslint-disable-next-line @typescript-eslint/no-unused-vars,@typescript-eslint/no-empty-function
    onChange(value: any) {}

    // eslint-disable-next-line @typescript-eslint/no-unused-vars,@typescript-eslint/no-empty-function
    onTouch(value: any): void {}

    registerOnChange(fn: () => void): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: () => void): void {
        this.onTouch = fn;
    }

    writeValue(value: any): void {
        this.value = value;
    }
}
