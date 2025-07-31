import { Directive, ElementRef, forwardRef, HostListener, Input, Renderer2 } from '@angular/core';
import { DefaultValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Directive({
    selector: 'input[appAllowedCharacters]',
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            multi: true,
            useExisting: forwardRef(() => AllowedCharactersDirective),
        },
    ],
})
export class AllowedCharactersDirective extends DefaultValueAccessor {
    @Input() appAllowedCharacters = '';
    @Input() appAllowedCharactersConvertUppercase = false;

    @HostListener('input', ['$event']) input($event: InputEvent) {
        const target = $event.target as HTMLInputElement;
        const start = target.selectionStart;
        const regex = new RegExp(`[^${this.appAllowedCharacters}]`, 'g');

        if (this.appAllowedCharactersConvertUppercase) {
            target.value = target.value.replace(regex, '').toUpperCase();
        } else {
            target.value = target.value.replace(regex, '');
        }
        target.setSelectionRange(start, start);

        this.onChange(target.value);
    }

    constructor(renderer: Renderer2, elementRef: ElementRef) {
        super(renderer, elementRef, false);
    }
}
