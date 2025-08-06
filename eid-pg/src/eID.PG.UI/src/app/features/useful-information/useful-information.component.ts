import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { MessageService } from 'primeng/api';
import { first, Subscription } from 'rxjs';
import { EidManagementService } from '../eid-management/services/eid-management.service';
import { IHideHelpPagePreviewEventEmitter } from '../eid-management/interfaces/eid-management.interface';
import { IHelpPage } from './components/interfaces/useful-info';
import { UsefulInformationService } from 'src/app/core/services/useful-information.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-useful-information',
    templateUrl: './useful-information.component.html',
    styleUrls: ['./useful-information.component.scss'],
})
export class UsefulInformationComponent implements OnInit, OnDestroy {
    constructor(
        private eidManagementService: EidManagementService,
        private translocoLocaleService: TranslocoLocaleService,
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private usefulInformationService: UsefulInformationService,
        private route: ActivatedRoute,
        private router: Router,
    ) {}
    requestInProgress = false;
    language!: string;
    localeSubscription!: Subscription;
    usefulInformationSubscription!: Subscription;
    pagesList!: any;
    showPreview!: boolean;
    selectedPage!: IHelpPage[];
    viewPage!: string;
    ngOnInit(): void {
        this.usefulInformationSubscription = this.usefulInformationService.informationTabSubject.subscribe(
            () => (this.showPreview = false)
        );
        this.route.queryParams.subscribe(params => {
            this.viewPage = params['viewPage'];
            if(this.viewPage) {
                this.getHelppagesList();
            }
        });
        this.localeSubscription = this.translocoLocaleService.localeChanges$.subscribe((locale: string) => {
            const formLanguage = locale === 'bg-BG' ? 'bg' : 'en';
            this.language = formLanguage;
            this.getHelppagesList();
        });
    }

    getHelppagesList() {
        this.requestInProgress = true;
        this.eidManagementService
            .getAllHelpPages()
            .pipe(first())
            .subscribe({
                next: (pages: IHelpPage[]) => {
                    this.pagesList = this.transformDataList(pages)
                    if (this.viewPage) {
                        const foundPageArray = this.pagesList.find((pageArray: any) =>
                            pageArray.some((page: any) => page.pageName === this.viewPage && page.language === this.language)
                        );

                        if (foundPageArray) {
                            this.previewPage(foundPageArray);
                        }
                    }

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

    //groups help pages in an array of arrays where each
    //element contains both language objects
    transformDataList(pagesList: IHelpPage[]) {
        const transformedList: any[] = [];
        const pagesMap: { [key: string]: any[] } = {};

        pagesList.forEach(item => {
            const pageName = item.pageName;
            if (pagesMap[pageName]) {
                pagesMap[pageName].push(item);
            } else {
                pagesMap[pageName] = [item];
            }
        });
        for (const helpPage in pagesMap) {
            transformedList.push(pagesMap[helpPage]);
        }
        return transformedList;
    }

    getPageForLanguage(pages: IHelpPage[]) {
        const page = pages.find(page => page.language === this.language);
        return page ? page : null;
    }

    previewPage(page: IHelpPage[]) {
        this.showPreview = true;
        this.selectedPage = page;
    }

    ngOnDestroy(): void {
        this.localeSubscription.unsubscribe();
        this.usefulInformationSubscription.unsubscribe();
    }

    onHideDetails(eventData: IHideHelpPagePreviewEventEmitter) {
        // clears query params
        this.router.navigate([]);
        this.showPreview = eventData.showPreview;
    }
}
