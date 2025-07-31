import { Component, OnDestroy, OnInit } from '@angular/core';
import { JournalsClientService } from '../../services/journals-client.service';
import { Router } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { Subscription } from 'rxjs';
import { MessageService } from 'primeng/api';

@Component({
    selector: 'app-file-details',
    templateUrl: './file-details.component.html',
    styleUrls: ['./file-details.component.scss'],
    providers: [MessageService],
})
export class FileDetailsComponent implements OnInit, OnDestroy {
    constructor(
        private journalsClientService: JournalsClientService,
        private translocoService: TranslocoService,
        private router: Router,
        private messageService: MessageService
    ) {
        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            const state = navigation.extras.state as any;
            if (state) {
                this.selectedFile = state.selectedFile;
                this.isSearch = state.isSearch;
                if (state.searchedCriteria) {
                    this.searchedCriteria = state.searchedCriteria;
                }
            }
        }

        this.languageChangeSubscription = translocoService.langChanges$.subscribe(() => {
            if (this.isSearch) {
                this.breadcrumbItems = [
                    {
                        label: this.translocoService.translate('modules.journalLogs.txtJournal'),
                        routerLink: '/logs-viewer/journals',
                    },
                    {
                        label: this.translocoService.translate('modules.journalLogs.txtSearching'),
                        onClick: this.navigateToSearchLogFilesForm.bind(this),
                    },
                    {
                        label: this.translocoService.translate('modules.journalLogs.txtResults'),
                        onClick: this.navigateBack.bind(this),
                    },
                    { label: this.translocoService.translate('modules.journalLogs.txtLogFile') },
                ];
            } else {
                this.breadcrumbItems = [
                    {
                        label: this.translocoService.translate('modules.journalLogs.txtJournal'),
                        routerLink: '/logs-viewer/journals',
                    },
                    { label: this.translocoService.translate('modules.journalLogs.txtLogFile') },
                ];
            }
        });
    }

    selectedFile: any;
    loading = false;
    fileData: any[] = [];
    breadcrumbItems: IBreadCrumbItems[] = this._initialBreadcrumbs;
    languageChangeSubscription: Subscription;
    isSearch;
    searchedCriteria: any = {};

    get _initialBreadcrumbs(): IBreadCrumbItems[] {
        if (this.isSearch) {
            return ([] as IBreadCrumbItems[]).concat([
                {
                    label: this.translocoService.translate('modules.journalLogs.txtJournal'),
                    routerLink: '/logs-viewer/journals',
                },
                {
                    label: this.translocoService.translate('modules.journalLogs.txtSearching'),
                    routerLink: '/logs-viewer/journals/search-files',
                    onClick: this.navigateToSearchLogFilesForm.bind(this),
                },
                {
                    label: this.translocoService.translate('modules.journalLogs.txtResults'),
                    onClick: this.navigateBack.bind(this),
                },
                { label: this.translocoService.translate('modules.journalLogs.txtLogFile') },
            ]);
        } else {
            return ([] as IBreadCrumbItems[]).concat([
                {
                    label: this.translocoService.translate('modules.journalLogs.txtJournal'),
                    routerLink: '/logs-viewer/journals',
                },
                { label: this.translocoService.translate('modules.journalLogs.txtLogFile') },
            ]);
        }
    }

    ngOnInit() {
        if (this.selectedFile && this.selectedFile.sourceFilePath) {
            this.getFile();
        }
    }

    getFile() {
        const payload = {
            filePath: `${this.selectedFile.sourceFilePath}/${this.selectedFile.sourceFile}`,
        };
        this.loading = true;
        this.journalsClientService.getFile(payload).subscribe({
            next: (response: any) => {
                const jsonToArray = response.content.split(/\n\r|\n/g);
                this.fileData = jsonToArray.map((fileDataObject: any) => {
                    return JSON.parse(fileDataObject);
                });
                this.loading = false;
                if (!response.validationErrors.isValid) {
                    response.validationErrors.errors.map((error: any) => {
                        this.showErrorToast(this.translocoService.translate('modules.journalLogs.errors.txtError'));
                    });
                }
            },
            error: (error: any) => {
                console.log(error);
                this.showErrorToast(this.translocoService.translate('modules.journalLogs.errors.txtError'));
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

    isObject(value: any): boolean {
        return typeof value === 'object' && value !== null && !Array.isArray(value);
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    navigateBack() {
        if (this.isSearch) {
            this.router.navigate(['/logs-viewer/journals/log-file-results'], {
                skipLocationChange: true,
                state: { shouldRefresh: false, payload: this.searchedCriteria },
            });
        } else {
            this.router.navigate(['/logs-viewer/journals']);
        }
    }

    navigateToSearchLogFilesForm() {
        this.router.navigate(['/logs-viewer/journals/search-files'], {
            state: this.searchedCriteria,
        });
    }
}
