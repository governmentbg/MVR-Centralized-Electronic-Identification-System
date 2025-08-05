import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { Subscription } from 'rxjs';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { IHideHelpPagePreviewEventEmitter } from 'src/app/features/eid-management/interfaces/eid-management.interface';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { IHelpPage } from '../interfaces/useful-info';

@Component({
    selector: 'app-page-preview',
    templateUrl: './page-preview.component.html',
    styleUrls: ['./page-preview.component.scss'],
})
export class PagePreviewComponent implements OnInit, OnDestroy {
    @Output() hide: EventEmitter<IHideHelpPagePreviewEventEmitter> =
        new EventEmitter<IHideHelpPagePreviewEventEmitter>();
    @Input() selectedPage!: IHelpPage[];
    language!: string;
    localeSubscription!: Subscription;
    displayedPage!: IHelpPage | undefined;
    breadcrumbItems: IBreadCrumbItems[] = [];
    safeContentWithHtml!: SafeHtml;

    constructor(
        private translocoLocaleService: TranslocoLocaleService,
        public translateService: TranslocoService,
        private sanitizer: DomSanitizer
    ) {}

    ngOnInit(): void {
        this.localeSubscription = this.translocoLocaleService.localeChanges$.subscribe((locale: string) => {
            const language = locale === 'bg-BG' ? 'bg' : 'en';
            this.displayedPage = this.selectedPage.find((el: IHelpPage) => el.language === language);
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('subNavbar.txtUsefulInformation'),
                    onClick: this.hidePreview.bind(this),
                },
                { label: this.displayedPage?.title },
            ];
            this.updateContentWithHtml();
        });
    }

    updateContentWithHtml(): void {
        if (this.displayedPage?.contentWithHtml) {
            this.safeContentWithHtml = this.sanitizer.bypassSecurityTrustHtml(this.displayedPage.contentWithHtml);
        }
    }

    hidePreview() {
        this.hide.emit({ showPreview: false });
    }

    ngOnDestroy(): void {
        this.localeSubscription.unsubscribe();
    }
}
