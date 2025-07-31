import { Directive, ElementRef, forwardRef, HostListener, Renderer2 } from '@angular/core';
import { DefaultValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Directive({
    selector: 'input[appUppercase]',
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: forwardRef(() => UppercaseDirective),
        },
    ],
})
export class UppercaseDirective extends DefaultValueAccessor {
    @HostListener('input', ['$event']) input($event: InputEvent) {
        const target = $event.target as HTMLInputElement;
        const start = target.selectionStart;

        target.value = target.value.toUpperCase();
        target.setSelectionRange(start, start);

        this.onChange(target.value);
    }

    constructor(renderer: Renderer2, elementRef: ElementRef) {
        super(renderer, elementRef, false);
    }
}
