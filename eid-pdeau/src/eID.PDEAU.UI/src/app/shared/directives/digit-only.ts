import { Directive } from '@angular/core';
import { DigitOnlyDirective } from '@uiowa/digit-only';
// This directive is used because when DigitOnlyModule is used in combinations with PrimeNG Mask Input, there is a namespace collosion with the MaskDirective
@Directive({
    selector: '[appDigitOnly]',
})
export class AppDigitOnlyDirective extends DigitOnlyDirective {}
