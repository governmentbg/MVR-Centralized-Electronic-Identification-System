import { Component, OnInit } from '@angular/core';
import { JournalsClientService } from '../../services/journals-client.service';
import { TranslocoService } from '@ngneat/transloco';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { dateMoreThanValidate } from '../../validators/date';

@Component({
    selector: 'app-journals',
    templateUrl: './files-journal.component.html',
    styleUrls: ['./files-journal.component.scss'],
    providers: [MessageService],
})
export class FilesJournalComponent implements OnInit {
    constructor(
        private journalsClientService: JournalsClientService,
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private router: Router
    ) {
        this.languageChangeSubscription =
            translocoService.langChanges$.subscribe(() => {
                this.cols = [
                    {
                        field: 'sourceFile',
                        header: this.translocoService.translate(
                            'modules.journalLogs.txtLogFiles'
                        ),
                    },
                    {
                        field: 'createdOn',
                        header: this.translocoService.translate(
                            'modules.journalLogs.txtDate'
                        ),
                    },
                ];
            });
        this.form.valueChanges.subscribe((changedFormValue) => {
            this.journalsClientService.fromDate = changedFormValue.fromDate;
            this.journalsClientService.toDate = changedFormValue.toDate;
        });
    }

    languageChangeSubscription: Subscription;
    files: any[] = [];
    loading = false;
    cols: any[] = [
        {
            field: 'sourceFile',
            header: this.translocoService.translate(
                'modules.journalLogs.txtLogFiles'
            ),
        },
        {
            field: 'createdOn',
            header: this.translocoService.translate(
                'modules.journalLogs.txtDate'
            ),
        },
    ];
    form = new FormGroup({
        fromDate: new FormControl<Date | null>(
            new Date(this.journalsClientService.fromDate),
            Validators.required
        ),
        toDate: new FormControl<Date | null>(
            new Date(this.journalsClientService.toDate),
            [Validators.required, dateMoreThanValidate()]
        ),
    });

    ngOnInit() {
        this.getFiles();
    }

    getFiles(shouldRefresh?: boolean) {
        this.loading = true;
        this.journalsClientService.getCachedFiles(shouldRefresh).subscribe({
            next: (response: any) => {
                this.files = response.records.sort(
                    (a: any, b: any) =>
                        new Date(b.createdOn).getTime() -
                        new Date(a.createdOn).getTime()
                );
                this.loading = false;
                if (!response.validationErrors.isValid) {
                    response.validationErrors.errors.map((error: any) => {
                        this.showErrorToast(
                            this.translocoService.translate(
                                'modules.journalLogs.errors.txtError'
                            )
                        );
                    });
                }
            },
            error: (error) => {
                this.showErrorToast(
                    this.translocoService.translate(
                        'modules.journalLogs.errors.txtError'
                    )
                );
                this.loading = false;
            },
        });
    }

    showErrorToast(message: string) {
        this.messageService.clear();
        this.messageService.add({
            key: 'toast',
            severity: 'error',
            summary: this.translocoService.translate('global.txtErrorTitle'),
            detail: message,
        });
    }

    navigateToSearchLogFilesForm() {
        this.router.navigate(['/logs-viewer/journals/search-files']);
    }

    openFileDetails(event: any) {
        this.router.navigate(['/logs-viewer/journals/file-details'], {
            skipLocationChange: true,
            state: { selectedFile: event.data },
        });
    }

    refreshFiles() {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.form.markAsPristine();
            this.getFiles(true);
        }
    }

    validateToDate() {
        this.form.controls['toDate'].updateValueAndValidity();
    }
}
