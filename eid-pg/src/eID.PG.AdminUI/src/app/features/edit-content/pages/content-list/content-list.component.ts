import { Component, OnDestroy } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService, MessageService } from 'primeng/api';
import { first, Subscription } from 'rxjs';
import { ContentService } from '../../../search/services/content.service';
import { EditContentComponent } from '../edit-content/edit-content.component';
import { IHideEditPageEmitter } from '../interfaces/interfaces';
import { IHelpPage } from 'src/app/features/create-content/interfaces/editor-html-target';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
@Component({
    selector: 'app-content-list',
    templateUrl: './content-list.component.html',
    styleUrls: ['./content-list.component.scss'],
    providers: [ConfirmationService, EditContentComponent],
})
export class ContentListComponent implements OnDestroy {
    constructor(
        private contentService: ContentService,
        private translocoService: TranslocoService,
        private translocoLocaleService: TranslocoLocaleService,
        private messageService: MessageService,
        private confirmationService: ConfirmationService
    ) {
        this.langSubscription = this.translocoLocaleService.localeChanges$.subscribe((locale: string) => {
            const formLanguage = locale === 'bg-BG' ? 'bg' : 'en';
            this.language = formLanguage;
            this.getHelppagesList();
        });
    }
    requestInProgress = false;
    langSubscription!: Subscription;
    pagesList!: any;
    editContent!: boolean;
    selectedContent!: any;
    skeletonArray = Array(8);
    language!: string;

    getHelppagesList() {
        this.contentService
            .getAllHelpPages()
            .pipe(first())
            .subscribe({
                next: (pages: IHelpPage[]) => {
                    this.pagesList = this.transformDataList(pages);
                    this.requestInProgress = false;
                },
                error: () => {
                    this.messageService.add({
                        severity: 'error',
                        summary: this.translocoService.translate('global.txtErrorTitle'),
                        detail: this.translocoService.translate('global.txtNoRecordsFound'),
                    });
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

    showPreview(selectedContent: IHelpPage[]) {
        this.selectedContent = selectedContent;
        this.editContent = true;
    }

    onHideDetails(eventData: IHideEditPageEmitter) {
        if (eventData.updateContent) {
            this.getHelppagesList();
        }
        this.editContent = eventData.editContent;
    }

    ngOnDestroy() {
        this.langSubscription.unsubscribe();
    }

    getIndexForLanguage(pages: IHelpPage[]) {
        const page = pages.find(page => page.language === this.language);
        return page ? page : null;
    }

    confirmDelete(helpPage: any) {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            message: this.translocoService.translate('content.txtDeleteHelpPageWarning'),
            header: this.translocoService.translate('content.txtHelpPageDeletion'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                for (const page of helpPage) {
                    this.deleteHelpPage(page);
                }
            },
        });
    }

    deleteHelpPage(helpPage: IHelpPage) {
        this.contentService
            .deleteContent(helpPage.id)
            .pipe(first())
            .subscribe({
                next: () => {
                    this.messageService.add({
                        severity: 'success',
                        summary: this.translocoService.translate('global.txtSuccessTitle'),
                        detail: this.translocoService.translate('content.txtContentDeletedSuccessfully'),
                    });
                    this.getHelppagesList();
                },
                error: () => {
                    this.messageService.add({
                        severity: 'error',
                        summary: this.translocoService.translate('global.txtErrorTitle'),
                        detail: this.translocoService.translate('content.txtContentDeleteError'),
                    });
                },
            });
    }
}
