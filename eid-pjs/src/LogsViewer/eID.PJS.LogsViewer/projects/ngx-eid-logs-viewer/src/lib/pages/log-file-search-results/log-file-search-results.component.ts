import {Component, OnDestroy, OnInit} from '@angular/core';
import {JournalsClientService} from '../../services/journals-client.service';
import {TranslocoService} from '@ngneat/transloco';
import {TranslocoLocaleService} from '@ngneat/transloco-locale';
import {MessageService} from 'primeng/api';
import {Router} from '@angular/router';
import {IBreadCrumbItems} from '../../interfaces/IBreadCrumbItems';
import {Subscription} from 'rxjs';
import * as defaultMoment from 'moment';

let moment: any;

@Component({
    selector: 'app-log-file-search-results',
    templateUrl: './log-file-search-results.component.html',
    styleUrls: ['./log-file-search-results.component.scss'],
    providers: [MessageService],
})
export class LogFileSearchResultsComponent implements OnInit, OnDestroy {
    constructor(
        private journalsClientService: JournalsClientService,
        private translocoService: TranslocoService,
        private messageService: MessageService,
        private router: Router,
        private translocoLocaleService: TranslocoLocaleService
    ) {
        if ('default' in defaultMoment) {
            moment = defaultMoment['default'];
        }

        this.languageChangeSubscription =
            translocoService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translocoService.translate(
                            'modules.journalLogs.txtJournal'
                        ),
                        routerLink: '/logs-viewer/journals',
                    },
                    {
                        label: this.translocoService.translate(
                            'modules.journalLogs.txtSearching'
                        ),
                        routerLink: '/logs-viewer/journals/search-files',
                    },
                    {
                        label: this.translocoService.translate(
                            'modules.journalLogs.txtResults'
                        ),
                    },
                ];
            });

        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            const state = navigation.extras.state as any;
            if (state) {
                if (state.payload) {
                    this.searchedCriteria = state.payload;
                    this.preparePayload(state.payload);
                }
                this.shouldRefreshData = state.shouldRefresh;
            }
        }
    }

    logs: any[] = [];
    loading = false;
    languageChangeSubscription: Subscription;
    breadcrumbItems: IBreadCrumbItems[] = this._initialBreadcrumbs;
    searchedCriteria: any = {};
    configPayload = {
        fileDateRangeFilter: {
            fromDate: '',
            toDate: '',
        },
        dataQuery: {
            type: 'logical',
            operator: 'and',
            nodes: [],
        },
    };
    recordsCount = 0;
    shouldRefreshData = false;

    get _initialBreadcrumbs(): IBreadCrumbItems[] {
        return ([] as IBreadCrumbItems[]).concat([
            {
                label: this.translocoService.translate(
                    'modules.journalLogs.txtJournal'
                ),
                routerLink: '/logs-viewer/journals',
            },
            {
                label: this.translocoService.translate(
                    'modules.journalLogs.txtSearching'
                ),
                routerLink: '/logs-viewer/journals/search-files',
            },
            {
                label: this.translocoService.translate(
                    'modules.journalLogs.txtResults'
                ),
            },
        ]);
    }

    ngOnInit() {
        this.loading = true;
        this.journalsClientService
            .getCachedLogs(this.configPayload, this.shouldRefreshData)
            .subscribe({
                next: (response: any) => {
                    const transformedToObjectData: any = {};
                    const preparedDate: any[] = [];
                    response.records.map((log: any) => {
                        if (
                            Object.prototype.hasOwnProperty.call(
                                transformedToObjectData,
                                log.sourceFile
                            )
                        ) {
                            transformedToObjectData[log.sourceFile].logs.push(
                                log
                            );
                        } else {
                            transformedToObjectData[log.sourceFile] = {
                                sourceFilePath: log.sourceFilePath,
                                logs: [],
                            };
                            transformedToObjectData[log.sourceFile].logs.push(
                                log
                            );
                        }
                    });

                    for (const [key, value] of Object.entries(
                        transformedToObjectData
                    )) {
                        preparedDate.push({
                            sourceFile: key,
                            sourceFilePath: (value as any).sourceFilePath,
                            logs: (value as any).logs,
                        });
                    }

                    this.logs = preparedDate;

                    this.recordsCount = response.recordsCount;
                    this.loading = false;
                    if (!response.validationErrors.isValid) {
                        response.validationErrors.errors.map((error: any) => {
                            if (error.errorCode === 'result-limit-reached') {
                                this.showErrorToast(
                                    this.translocoService.translate(
                                        'modules.journalLogs.errors.txtSpecifySearch'
                                    )
                                );
                            } else {
                                this.showErrorToast(
                                    this.translocoService.translate(
                                        'modules.journalLogs.errors.txtError'
                                    )
                                );
                            }
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

    preparePayload(state: any) {
        this.configPayload.fileDateRangeFilter = {
            fromDate: new Date(state.fromDate).toISOString(),
            toDate: new Date(state.toDate).toISOString(),
        };

        state.properties.map((prop: any) => {
            const isValidDate = moment(
                prop.value,
                'YYYY-MM-DD',
                true
            ).isValid();
            if (isValidDate) {
                prop.value = moment(prop.value).toISOString();
            }
            (this.configPayload.dataQuery.nodes as any).push({
                type: 'compare',
                operator: prop.operator,
                field: prop.field,
                value: prop.value,
            });
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

    formatWholeDate(startDate: Date, endDate: Date): string {
        if (!startDate && !endDate) {
            return '';
        }

        return `${this.translocoLocaleService.localizeDate(
            startDate,
            this.translocoLocaleService.getLocale(),
            {timeStyle: 'medium'}
        )} - ${this.translocoLocaleService.localizeDate(
            endDate,
            this.translocoLocaleService.getLocale(),
            {timeStyle: 'medium'}
        )}`;
    }

    navigateToSearchLogFilesForm() {
        this.router.navigate(['/logs-viewer/journals/search-files'], {
            state: this.searchedCriteria,
        });
    }

    openFileDetails(file: any) {
        this.router.navigate(['/logs-viewer/journals/file-details'], {
            skipLocationChange: true,
            state: {
                selectedFile: file,
                isSearch: true,
                searchedCriteria: this.searchedCriteria,
            },
        });
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    isObject(value: any): boolean {
        return (
            typeof value === 'object' && value !== null && !Array.isArray(value)
        );
    }
}
