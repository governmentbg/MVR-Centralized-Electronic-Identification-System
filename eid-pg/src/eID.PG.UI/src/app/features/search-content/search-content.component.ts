import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { MessageService, LazyLoadEvent } from 'primeng/api';
import { Subscription, first } from 'rxjs';
import { Language } from './enum/search';
import { EidManagementService } from '../eid-management/services/eid-management.service';
import { SortOrder } from '../authorization-register/enums/authorization-register.enum';

@Component({
    selector: 'app-search-content',
    templateUrl: './search-content.component.html',
    styleUrls: ['./search-content.component.scss'],
})
export class SearchContentComponent implements OnInit, OnDestroy {
    constructor(
        private fb: FormBuilder,
        private eidManagementService: EidManagementService,
        private translocoLocaleService: TranslocoLocaleService,
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private sanitizer: DomSanitizer
    ) {}
    form!: FormGroup;
    requestInProgress = false;
    language!: string;
    localeSubscription!: Subscription;
    pagesList!: any;
    totalRecords = 0;
    pageSize = 10;
    minKeywordLength = 3;
    lazyLoadEventState: LazyLoadEvent = {};
    displayPreviewModal = false;
    selectedTitle!: string;
    selectedContent!: SafeHtml;
    submitted = false;
    sortOrder = SortOrder;

    ngOnInit(): void {
        this.form = this.fb.group(
            {
                keyword: [null, [Validators.required, Validators.minLength(this.minKeywordLength)]],
                language: [null],
                page: 0,
                size: this.pageSize,
                sort: ['asc'],
            },
            { updateOn: 'submit' }
        );

        this.localeSubscription = this.translocoLocaleService.localeChanges$.subscribe((locale: string) => {
            const formLanguage = locale === 'bg-BG' ? Language.BG : Language.EN;
            this.form.patchValue({ language: formLanguage });
        });
    }

    submit() {
        if (this.form.valid) {
            this.requestInProgress = true;
            this.eidManagementService
                .getHelpPagesList(this.form.value)
                .pipe(first())
                .subscribe({
                    next: (pages: any) => {
                        this.pagesList = pages.content;
                        this.totalRecords = pages.totalElements;
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
        } else {
            this.requestInProgress = false;
            this.markFormGroupTouched(this.form);
        }
    }

    markFormGroupTouched(formGroup: FormGroup) {
        Object.values(formGroup.controls).forEach(control => {
            if (control instanceof FormGroup) {
                this.markFormGroupTouched(control);
            } else {
                control.markAsTouched();
                control.markAsDirty();
            }
        });
    }

    ngOnDestroy() {
        this.localeSubscription.unsubscribe();
    }

    showPreview(pageItem: any) {
        this.selectedTitle = pageItem.title;
        this.selectedContent = this.sanitizer.bypassSecurityTrustHtml(pageItem.contentWithHtml);
        this.displayPreviewModal = true;
    }

    loadApplications(event: LazyLoadEvent) {
        const pageNumber = event.first && event.rows ? event.first / event.rows : 0;
        const pageSize = event.rows || this.pageSize;
        this.form.patchValue({
            page: pageNumber,
            size: pageSize,
        });
        this.submit();
        this.requestInProgress = false;
    }

    highlightKeyword(contentWithHtml: string, keyword: string): SafeHtml {
        if (!contentWithHtml) return '';
        const strippedContent = contentWithHtml.replace(/<\/?[^>]+(>|$)/g, '');

        if (!keyword) return strippedContent;
        const escapedKeyword = keyword.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
        const regex = new RegExp(`(${escapedKeyword})`, 'gi');
        const firstMatchIndex = strippedContent.search(regex);
        if (firstMatchIndex === -1) {
            return strippedContent.length > 300 ? strippedContent.substring(0, 300) + '...' : strippedContent;
        }
        const contentAfterMatch = strippedContent.substring(firstMatchIndex);
        let truncatedContent = contentAfterMatch.substring(0, 300) + '...';
        const lastSpaceIndex = truncatedContent.lastIndexOf(' ');
        if (lastSpaceIndex !== -1 && lastSpaceIndex > 150) {
            truncatedContent = truncatedContent.substring(0, lastSpaceIndex) + '...';
        }
        const highlightedContent = truncatedContent.replace(regex, '<strong>$1</strong>');
        if (contentAfterMatch.length > 300) {
            truncatedContent += '...';
        }

        return this.sanitizer.bypassSecurityTrustHtml(highlightedContent);
    }
}
