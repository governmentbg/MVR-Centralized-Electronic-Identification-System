import { Component } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-empowerment-check-language-selector',
    templateUrl: './empowerment-check-language-selector.component.html',
    styleUrls: ['./empowerment-check-language-selector.component.scss'],
})
export class EmpowermentCheckLanguageSelectorComponent {
    constructor(private translocoService: TranslocoService) {}

    activeLanguage(): string {
        return this.translocoService.getActiveLang();
    }

    changeLanguage(language: string): void {
        this.translocoService.setActiveLang(language);
    }
}
