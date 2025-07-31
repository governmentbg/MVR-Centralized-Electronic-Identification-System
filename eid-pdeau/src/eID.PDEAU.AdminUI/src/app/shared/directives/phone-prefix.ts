import { Directive, HostListener, ElementRef, OnInit } from '@angular/core';

@Directive({
    selector: '[appPrefixPhoneInput]',
})
export class PrefixInputDirective implements OnInit {
    constructor(private el: ElementRef<HTMLInputElement>) {}

    ngOnInit(): void {
        if (!this.el.nativeElement.value) {
            this.el.nativeElement.value = '+';
        }
    }

    @HostListener('input', ['$event']) onInputChange() {
        const initialValue = this.el.nativeElement.value;

        if (initialValue.charAt(0) !== '+' && initialValue !== '') {
            this.el.nativeElement.value = '+' + initialValue;
        } else if (initialValue === '') {
            this.el.nativeElement.value = '+';
        }
    }
}
