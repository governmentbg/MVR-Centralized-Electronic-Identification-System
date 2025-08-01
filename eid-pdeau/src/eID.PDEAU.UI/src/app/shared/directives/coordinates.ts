import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: '[appCoordinates]',
})
export class AppCoordinatesDirective {
    private regex = new RegExp(/^[0-9]*\.?[0-9]*$/g);
    private specialKeys: Array<string> = ['Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Delete'];

    constructor(private el: ElementRef) {}

    @HostListener('keydown', ['$event'])
    onKeyDown(event: KeyboardEvent) {
        // Allow special keys
        if (this.specialKeys.indexOf(event.key) !== -1) {
            return;
        }

        // Allow only one dot
        if (event.key === '.' && this.el.nativeElement.value.includes('.')) {
            event.preventDefault();
            return;
        }

        // Allow numbers and dot
        const current: string = this.el.nativeElement.value;
        const next: string = current.concat(event.key);
        if (next && !String(next).match(this.regex)) {
            event.preventDefault();
        }
    }

    @HostListener('paste', ['$event'])
    onPaste(event: ClipboardEvent) {
        const clipboardData = event.clipboardData;
        const pastedText = clipboardData?.getData('text');
        if (pastedText && !String(pastedText).match(this.regex)) {
            event.preventDefault();
        }
    }
}
