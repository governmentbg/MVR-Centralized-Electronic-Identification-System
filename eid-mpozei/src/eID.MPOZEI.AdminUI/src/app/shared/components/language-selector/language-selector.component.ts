import { Component } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-language-selector',
    templateUrl: './language-selector.component.html',
    styleUrls: ['./language-selector.component.scss'],
})
export class LanguageSelectorComponent {
    constructor(private translocoService: TranslocoService) {}

    activeLanguage(): string {
        return this.translocoService.getActiveLang();
    }

    changeLanguage(language: string): void {
        this.translocoService.setActiveLang(language);
        localStorage.setItem('activeLang', language);
    }
}
