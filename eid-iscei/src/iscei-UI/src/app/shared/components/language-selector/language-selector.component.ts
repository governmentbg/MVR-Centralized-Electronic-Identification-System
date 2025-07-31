import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-language-selector',
    templateUrl: './language-selector.component.html',
    styleUrls: ['./language-selector.component.scss'],
})
export class LanguageSelectorComponent implements OnInit {
    constructor(private translocoService: TranslocoService) {}

    languageOptions = [
        {
            name: 'EN',
            code: 'en',
        },
        {
            name: 'БГ',
            code: 'bg',
        },
    ];

    selectedLanguage = '';
    toggled = false;

    ngOnInit() {
        this.selectedLanguage = this.translocoService.getActiveLang();
    }
    onLanguageOptionsChange(language: any): void {
        this.translocoService.setActiveLang(language.value);
    }
}
