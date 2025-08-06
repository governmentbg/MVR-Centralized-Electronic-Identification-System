import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ContentService } from '../../../search/services/content.service';
import { first, forkJoin, Subscription } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { MessageService } from 'primeng/api';
import { Language } from 'src/app/features/search/enums/search-enums';
import { IBreadCrumbItems, IHideEditPageEmitter } from '../interfaces/interfaces';
import { IHelpPage } from 'src/app/features/create-content/interfaces/editor-html-target';
@Component({
    selector: 'app-edit-content',
    templateUrl: './edit-content.component.html',
    styleUrls: ['./edit-content.component.scss'],
})
export class EditContentComponent implements OnInit, OnDestroy {
    //bg and en version array
    @Input() contentData!: [IHelpPage, IHelpPage];
    @Output() hide: EventEmitter<IHideEditPageEmitter> = new EventEmitter<IHideEditPageEmitter>();
    form!: FormGroup;
    requestInProgress!: boolean;
    editorInstanceBulgarian: any;
    editorInstanceEnglish: any;
    englishContent!: any;
    bulgarianContent!: any;
    updateContent = false;
    breadcrumbItems: IBreadCrumbItems[] = [];
    subscriptions: Subscription[] = [];
    constructor(
        private fb: FormBuilder,
        private contentService: ContentService,
        private translocoService: TranslocoService,
        private messageService: MessageService
    ) {
        const languageChangeSubscription = translocoService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translocoService.translate('searchPage.txtContents'),
                    onClick: this.hidePreview.bind(this),
                },
                { label: this.translocoService.translate('searchPage.txtEditContent') },
            ];
        });
        this.subscriptions.push(languageChangeSubscription);
    }

    init = {
        plugins: 'lists link image table help wordcount searchreplace preview code save media lists advlist lists',
        language_url: '../../../../assets/tinymceLang/bg_BG.js',
        language: 'bg_BG',
        toolbar:
            'undo redo | bold italic | alignleft aligncenter alignright | numlist bullist | code advlist | autolink | link | image | preview | code | save | media| lists advlist',
        menubar: true,
        branding: false,
    };

    ngOnInit(): void {
        const bulgarianContent = this.contentData.find((el: any) => el.language === Language.BG);
        const englishContent = this.contentData.find((el: any) => el.language === Language.EN);
        this.form = this.fb.group({
            id: [null],
            pageName: [bulgarianContent?.pageName, Validators.required],
            bulgarianTitle: [bulgarianContent?.title, Validators.required],
            englishTitle: [englishContent?.title, Validators.required],
            contentWithHtml: [null],
            language: [null],
        });
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    hidePreview() {
        this.hide.emit({ editContent: false, updateContent: this.updateContent });
    }

    setupBulgarianEditor(editor: any) {
        this.editorInstanceBulgarian = editor;
        this.bulgarianContent = this.contentData.find((el: any) => el.language === Language.BG);
        if (this.bulgarianContent) {
            this.editorInstanceBulgarian.setContent(this.bulgarianContent.contentWithHtml);
        }
    }

    setupEnglishEditor(editor: any) {
        this.editorInstanceEnglish = editor;
        this.englishContent = this.contentData.find((el: any) => el.language === Language.EN);
        if (this.englishContent) {
            this.editorInstanceEnglish.setContent(this.englishContent.contentWithHtml);
        }
    }

    submit(event: any) {
        event.preventDefault();
        const bulgarianContent = this.editorInstanceBulgarian.getContent();
        const englishContent = this.editorInstanceEnglish.getContent();
        if (!englishContent || !bulgarianContent) {
            this.messageService.add({
                severity: 'error',
                summary: this.translocoService.translate('global.txtErrorTitle'),
                detail: this.translocoService.translate('global.txtMissingLanguageContent'),
            });
            return;
        }

        if (this.form.valid && this.bulgarianContent && this.englishContent) {
            const bulgarianPayload = {
                pageName: this.form.get('pageName')?.value,
                title: this.form.get('bulgarianTitle')?.value,
                contentWithHtml: bulgarianContent,
                language: Language.BG,
            };

            const englishPayload = {
                pageName: this.form.get('pageName')?.value,
                title: this.form.get('englishTitle')?.value,
                contentWithHtml: englishContent,
                language: Language.EN,
            };

            this.requestInProgress = true;
            forkJoin([
                this.contentService.updateContent(bulgarianPayload).pipe(first()),
                this.contentService.updateContent(englishPayload).pipe(first()),
            ]).subscribe({
                next: () => {
                    this.messageService.add({
                        severity: 'success',
                        summary: this.translocoService.translate('global.txtSuccessTitle'),
                        detail: this.translocoService.translate('content.txtContentUpdatedSuccessfully'),
                    });
                    this.updateContent = true;
                    this.requestInProgress = false;
                },
                error: () => {
                    this.messageService.add({
                        severity: 'error',
                        summary: this.translocoService.translate('global.txtErrorTitle'),
                        detail: this.translocoService.translate('content.txtErrorUpdatingContent'),
                    });
                    this.requestInProgress = false;
                },
            });
        } else {
            this.markFormGroupTouched(this.form);
            this.requestInProgress = false;
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
}
