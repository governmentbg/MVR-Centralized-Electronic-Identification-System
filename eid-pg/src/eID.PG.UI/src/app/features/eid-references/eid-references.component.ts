import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { MessageService, LazyLoadEvent } from 'primeng/api';
import { first, Subscription } from 'rxjs';
import { SortOrder } from '../authorization-register/enums/authorization-register.enum';
import { EidManagementService } from '../eid-management/services/eid-management.service';
import { Language } from '../search-content/enum/search';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';

@Component({
    selector: 'app-eid-references',
    templateUrl: './eid-references.component.html',
    styleUrls: ['./eid-references.component.scss'],
})
export class EidReferencesComponent implements OnInit, OnDestroy {
    constructor(
        private fb: FormBuilder,
        private eidManagementService: EidManagementService,
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private translocoLocaleService: TranslocoLocaleService
    ) {}
    form!: FormGroup;
    requestInProgress = false;
    language!: string;
    totalRecords = 0;
    pageSize = 10;
    minKeywordLength = 3;
    referencesList!: any;
    localeSubscription!: Subscription;
    lazyLoadEventState: LazyLoadEvent = {};
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
                .getEidReferences(this.form.value)
                .pipe(first())
                .subscribe({
                    next: (references: any) => {
                        this.referencesList = references.content;
                        this.totalRecords = references.totalElements;
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

    ngOnDestroy() {
        this.localeSubscription.unsubscribe();
    }

    computedStatus(status: any) {
        return this.translocoService.translate('modules.eidManagement.certificateStatus.txt' + status);
    }

    formatName(name: string) {
        return name.replace('CN=', '');
    }
}
