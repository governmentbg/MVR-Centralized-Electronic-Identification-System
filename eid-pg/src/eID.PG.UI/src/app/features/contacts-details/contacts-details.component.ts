import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { MessageService } from 'primeng/api';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Subscription, first } from 'rxjs';
import { HelpPages } from 'src/app/shared/enums/help-pages';
import { EidManagementService } from '../eid-management/services/eid-management.service';
import { IHelpPage } from '../useful-information/components/interfaces/useful-info';

@Component({
    selector: 'app-contacts-details',
    templateUrl: './contacts-details.component.html',
    styleUrls: ['./contacts-details.component.scss'],
})
export class ContactsDetailsComponent implements OnInit, OnDestroy {
    constructor(
        private eidManagementService: EidManagementService,
        private translocoLocaleService: TranslocoLocaleService,
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private sanitizer: DomSanitizer
    ) {}

    requestInProgress = false;
    language!: string;
    localeSubscription!: Subscription;
    pagesList!: any;

    ngOnInit(): void {
        this.localeSubscription = this.translocoLocaleService.localeChanges$.subscribe((locale: string) => {
            const formLanguage = locale === 'bg-BG' ? 'bg' : 'en';
            this.language = formLanguage;
            this.getHelpPage();
        });
    }

    getHelpPage() {
        this.eidManagementService
            .getHelpPageByPageName(HelpPages.CONTACT_DETAILS)
            .pipe(first())
            .subscribe({
                next: (pages: IHelpPage[]) => {
                    this.pagesList = pages.map((page: IHelpPage) => ({
                        ...page,
                        contentWithHtml: this.sanitizeHtml(page.contentWithHtml),
                    }));
                    this.requestInProgress = false;
                },
                error: () => {
                    this.messageService.add({
                        severity: 'error',
                        summary: this.translocoService.translate('global.txtErrorTitle'),
                        detail: this.translocoService.translate('global.txtNoRecordsFound'),
                    });
                    this.requestInProgress = false;
                },
            });
    }

    getPageForLanguage(pages: IHelpPage[]) {
        const page = pages?.find(page => page.language === this.language);
        return page ? page : null;
    }

    sanitizeHtml(content: string | null): SafeHtml {
        return content ? this.sanitizer.bypassSecurityTrustHtml(content) : '';
    }

    ngOnDestroy(): void {
        this.localeSubscription.unsubscribe();
    }
}
