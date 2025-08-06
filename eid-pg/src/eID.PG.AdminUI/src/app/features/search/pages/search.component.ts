import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first, Subscription } from 'rxjs';
import { ContentService } from '../services/content.service';
import { TranslocoService } from '@ngneat/transloco';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { SortOrder } from '../enums/search-enums';
import { IHelpPage } from '../../create-content/interfaces/editor-html-target';
@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss'],
})
export class SearchComponent implements OnInit, OnDestroy {
    constructor(
        private fb: FormBuilder,
        private contentService: ContentService,
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private sanitizer: DomSanitizer
    ) {}
    form!: FormGroup;
    requestInProgress = false;
    localeSubscription!: Subscription;
    pagesList!: any;
    totalRecords = 0;
    pageSize = 10;
    minKeywordLength = 3;
    displayPreviewModal = false;
    selectedTitle!: string;
    selectedContent!: SafeHtml;
    sortOrder = SortOrder;

    ngOnInit(): void {
        this.form = this.fb.group({
            keyword: [null, Validators.required],
            language: [this.translocoService.getActiveLang()],
            page: 0,
            size: this.pageSize,
            sort: ['asc'],
        });

        this.localeSubscription = this.translocoService.langChanges$.subscribe(() => {
            this.form.patchValue({ language: this.translocoService.getActiveLang() });
        });
    }

    submit() {
        const keywordControl = this.form.controls['keyword'];
        if (!keywordControl.value) {
            keywordControl.setErrors({ required: true });
            this.markFormGroupTouched(this.form);
            return;
        }
        if (keywordControl.value && keywordControl.value && keywordControl.value.length < this.minKeywordLength) {
            keywordControl.setErrors({ minlength: true });
            return;
        } else {
            keywordControl.setErrors(null);
        }
        if (this.form.valid) {
            this.requestInProgress = true;
            this.contentService
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
                            detail: this.translocoService.translate('global.txtUnexpectedError'),
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

    showPreview(pageItem: IHelpPage) {
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

    //runs through the html until it finds a word matching the search keyword
    //then cuts a slice of text starting from it for display
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
