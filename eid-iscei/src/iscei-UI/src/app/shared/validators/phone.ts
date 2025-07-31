import { Validators } from '@angular/forms';

export function phoneValidator() {
    return Validators.pattern('^8[789]\\d\\s\\d{3}\\s\\d{3}$');
}
