import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ContentService } from '../../search/services/content.service';
import { first, forkJoin } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { MessageService } from 'primeng/api';
import { Language } from '../../search/enums/search-enums';
@Component({
    selector: 'app-create-content',
    templateUrl: './create-content.component.html',
    styleUrls: ['./create-content.component.scss'],
})
export class CreateContentComponent implements OnInit {
    form!: FormGroup;
    requestInProgress!: boolean;
    editorInstanceBulgarian: any;
    editorInstanceEnglish: any;

    constructor(
        private fb: FormBuilder,
        private contentService: ContentService,
        private translocoService: TranslocoService,
        private messageService: MessageService
    ) {}

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
        this.form = this.fb.group({
            pageName: [null, Validators.required],
            bulgarianTitle: [null, Validators.required],
            englishTitle: [null, Validators.required],
            contentWithHtml: [null],
            language: [null],
        });
    }

    setupBulgarianEditor(editor: any) {
        this.editorInstanceBulgarian = editor;
    }

    setupEnglishEditor(editor: any) {
        this.editorInstanceEnglish = editor;
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

        if (this.form.valid) {
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
                this.contentService.postContent(bulgarianPayload).pipe(first()),
                this.contentService.postContent(englishPayload).pipe(first()),
            ]).subscribe({
                next: () => {
                    this.messageService.add({
                        severity: 'success',
                        summary: this.translocoService.translate('global.txtSuccessTitle'),
                        detail: this.translocoService.translate('global.txtContentUploadedSuccessfully'),
                    });
                    this.requestInProgress = false;
                },
                error: () => {
                    this.messageService.add({
                        severity: 'error',
                        summary: this.translocoService.translate('global.txtErrorTitle'),
                        detail: this.translocoService.translate('global.txtErrorUploadingContent'),
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
