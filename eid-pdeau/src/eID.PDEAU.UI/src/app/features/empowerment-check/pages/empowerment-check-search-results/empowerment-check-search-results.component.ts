import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { Subject, Subscription, switchMap, takeUntil, timer } from 'rxjs';
import { ToastService } from 'src/app/shared/services/toast.service';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { EmpowermentCheckClientService } from '../../services/empowerment-check-client.service';
import { CalculatedEmpowermentStatus, OnBehalfOf, SortOrder } from '../../enums/empowerment-check.enum';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { endOfDay } from 'date-fns';
import { EmpowermentXmlParserService } from '@app/features/empowerment-check/services/empowerment-xml-parser.service';
import { IEmpowerment } from '@app/features/empowerment-check/interfaces/empowerment-check.interfaces';
import { LazyLoadEvent } from 'primeng/api';

@Component({
    selector: 'app-empowerment-check-search-results',
    templateUrl: './empowerment-check-search-results.component.html',
    styleUrls: ['./empowerment-check-search-results.component.scss'],
})
export class EmpowermentCheckSearchResultsComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private toastService: ToastService,
        private empowermentCheckClientService: EmpowermentCheckClientService,
        private empowermentXmlParserService: EmpowermentXmlParserService,
        private translocoLocaleService: TranslocoLocaleService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('empowermentCheckModule.txtReferences'),
                    onClick: this.navigateToSearch.bind(this),
                },
                { label: this.translateService.translate('empowermentCheckModule.txtEmpowerment') },
            ];
        });
        const navigation = this.router.getCurrentNavigation();
        if (navigation || window.history.state) {
            const state = navigation ? (navigation.extras.state as any) : window.history.state;
            if (state) {
                this.searchedEmpowerment = state;
            }
        }
    }

    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading!: boolean;
    empowerments: any[] = [];
    searchedEmpowerment: any = {};
    empowermentStatus = CalculatedEmpowermentStatus;
    totalRecords = 0;
    pageSize = 10;
    lazyLoadEventState: LazyLoadEvent = {};
    empowermentsFilterState: any = {};
    INTERVAL = 10000;
    closeTimer$ = new Subject<void>();

    ngOnInit() {
        this.breadcrumbItems = [
            {
                label: this.translateService.translate('empowermentCheckModule.txtReferences'),
                onClick: this.navigateToSearch.bind(this),
            },
            { label: this.translateService.translate('empowermentCheckModule.txtEmpowerment') },
        ];
        this.loading = true;
    }

    ngOnDestroy() {
        this.closeTimer$.next();
    }

    loadEmpowerments(event: LazyLoadEvent, empowermentsFilterData?: any): void {
        this.lazyLoadEventState = event;
        this.loading = true;
        this.totalRecords = 0;

        let payload: any = {
            serviceId: this.searchedEmpowerment.service?.serviceNumber,
            empoweredUid: this.searchedEmpowerment.empoweredUid,
            empoweredUidType: this.searchedEmpowerment.empoweredUidType,
            authorizerUidType: this.searchedEmpowerment.authorizerUidType,
            authorizerUid: this.searchedEmpowerment.authorizerUid,
            onBehalfOf: this.searchedEmpowerment.onBehalfOf,
            statusOn: endOfDay(new Date(this.searchedEmpowerment.statusOn)).toISOString(),
            volumeOfRepresentation: this.searchedEmpowerment.volumeOfRepresentation,
        };
        if (
            this.searchedEmpowerment.volumeOfRepresentation !== null &&
            this.searchedEmpowerment.volumeOfRepresentation !== undefined &&
            this.searchedEmpowerment.volumeOfRepresentation.length > 0
        ) {
            payload.volumeOfRepresentation = payload.volumeOfRepresentation.map(({ name }: any) => name);
        }

        if (this.searchedEmpowerment.onBehalfOf === null) {
            payload.onBehalfOf = OnBehalfOf.Empty;
        }
        payload.sortby = event.sortField;
        payload.sortDirection = event.sortOrder !== undefined && SortOrder[event.sortOrder];
        payload.pageIndex = 1;

        if (empowermentsFilterData !== null && empowermentsFilterData !== undefined) {
            payload = { ...payload, ...empowermentsFilterData };
        } else if (this.empowermentsFilterState !== null && Object.keys(this.empowermentsFilterState).length > 0) {
            payload = { ...payload, ...this.empowermentsFilterState };
        }

        if (event && event.first && event.rows) {
            payload.pageIndex = event.first / event.rows + 1;
        }

        payload.pageSize = event.rows || this.pageSize;

        timer(0, this.INTERVAL)
            .pipe(
                switchMap(() => this.empowermentCheckClientService.empowermentCheck(payload)),
                takeUntil(this.closeTimer$) // close the subscription when `closeTimer$` emits
            )
            .subscribe({
                next: response => {
                    if (response.status === 200) {
                        this.closeTimer$.next();
                        response.body.data.forEach((item: IEmpowerment) => {
                            const xmlRepresentation = this.empowermentXmlParserService.convertXmlToObject(
                                item.xmlRepresentation
                            );
                            item.authorizerUids = xmlRepresentation.authorizerUids;
                            item.empoweredUids = xmlRepresentation.empoweredUids;
                            item.volumeOfRepresentation = xmlRepresentation.volumeOfRepresentation;
                            item.onBehalfOf = xmlRepresentation.onBehalfOf;
                            item.id = xmlRepresentation.id;
                            item.name = xmlRepresentation.name;
                            item.uid = xmlRepresentation.uid;
                            item.startDate = xmlRepresentation.startDate;
                            item.expiryDate = xmlRepresentation.expiryDate;
                            item.serviceId = xmlRepresentation.serviceId;
                            item.serviceName = xmlRepresentation.serviceName;
                            item.providerId = xmlRepresentation.providerId;
                            item.providerName = xmlRepresentation.providerName;
                            item.number = xmlRepresentation.number;
                        });

                        this.empowerments = response.body.data;
                        this.totalRecords = response.body.totalItems;
                        this.loading = false;
                    }
                },
                error: error => {
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        default:
                            this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                            break;
                    }
                    this.loading = false;
                },
            });
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    navigateToSearch() {
        this.router.navigate(['/empowerment-check'], {
            state: this.searchedEmpowerment,
        });
    }

    navigateToDetails(selectedEmpowerment: any) {
        this.router.navigate(['/empowerment-check/search-result-details'], {
            skipLocationChange: true,
            state: { searchedEmpowerment: this.searchedEmpowerment, selectedEmpowerment: selectedEmpowerment },
        });
    }

    empowermentExpiryDate(expiryDate: any) {
        if (expiryDate) {
            return this.translocoLocaleService.localizeDate(expiryDate, this.translocoLocaleService.getLocale());
        }
        return this.translateService.translate('empowermentCheckModule.txtIndefinitely');
    }

    getHtmlTooltipForExtraUids(empoweredUids: any): string {
        if (empoweredUids === null || typeof empoweredUids === 'undefined') {
            return '';
        }

        return empoweredUids
            .slice(1) // Skip the first element from the array
            .map((empoweredUid: any) => `<span>${empoweredUid.uid}</span>`) // Format the Uid
            .join('<br />');
    }

    getTooltipForExtraUids(empoweredUids: any): string {
        if (empoweredUids === null || typeof empoweredUids === 'undefined') {
            return '';
        }

        return empoweredUids
            .slice(1) // Skip the first element from the array
            .map((empoweredUid: any) => empoweredUid.uid) // Take the Uid
            .join(' ');
    }
}
