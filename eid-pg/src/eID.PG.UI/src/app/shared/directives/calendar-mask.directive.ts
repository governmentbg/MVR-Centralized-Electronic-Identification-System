import { Directive, HostListener } from '@angular/core';

@Directive({
    selector: '[appDate]',
})
export class DateFormatDirective {
    @HostListener('input', ['$event'])
    onInput(event: any) {
        let value = event.target.value.replace(/\D/g, '');
        if (value.length > 4) {
            value = value.substring(0, 4) + '-' + value.substring(4);
        }
        if (value.length > 7) {
            value = value.substring(0, 7) + '-' + value.substring(7);
        }
        event.target.value = value.substring(0, 10);
    }
}
