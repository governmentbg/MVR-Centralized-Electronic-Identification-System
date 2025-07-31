import {
    Component,
    ViewChild,
    ApplicationRef,
    ComponentRef,
    ComponentFactoryResolver,
    Injector,
    OnDestroy,
    ChangeDetectorRef,
    OnInit,
} from '@angular/core';
import { Subscription } from 'rxjs';
import { LazyLoadEvent } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { EmpowermentXmlParserService } from '../../services/empowerment-xml-parser.service';
import { AuthorizationRegisterService } from '../../services/authorization-register.service';
import { Table } from 'primeng/table';
import {
    IEmpowerment,
    IEmpowermentsPaginatedData,
    IHidePreviewEventEmitter,
} from '../../interfaces/authorization-register.interfaces';
import { EmpowermentStatementStatus, SortOrder } from '../../enums/authorization-register.enum';
import { ToastService } from '../../../../shared/services/toast.service';
import { EmpowermentCertificatePrintableComponent } from '../../components/empowerment-certificate-printable/empowerment-certificate-printable.component';

@Component({
    selector: 'app-authorizations-list',
    templateUrl: './authorizations-list.component.html',
    styleUrls: ['./authorizations-list.component.scss'],
})
export class AuthorizationsListComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private authorizationRegisterService: AuthorizationRegisterService,
        private empowermentXmlParserService: EmpowermentXmlParserService,
        private toastService: ToastService,
        private injector: Injector,
        private appRef: ApplicationRef,
        private componentFactoryResolver: ComponentFactoryResolver
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                    routerLink: '/authorization-register',
                },
            ];
        });
    }

    @ViewChild('dt') table!: Table;
    breadcrumbItems: any[] = [];
    empowerments: any[] = [];
    totalRecords = 0;
    pageSize = 10;
    loading!: boolean;
    lazyLoadEventState: LazyLoadEvent = {};
    empowermentsFilterState: any = null;
    languageChangeSubscription: Subscription = new Subscription();
    getEmpowermentsSubscription: Subscription = new Subscription();
    selectedEmpowerment: any;
    showPreview = false;
    showWithdrawalMode = false;

    loadEmpowerments(event: LazyLoadEvent, empowermentsFilterData?: any): void {
        // When we first load the page we dont want to load any empowerments until a filter is applied
        if (!empowermentsFilterData && !this.empowermentsFilterState) {
            return;
        }

        this.lazyLoadEventState = event;
        this.loading = true;
        this.totalRecords = 0;

        let payload: any = {};
        payload.sortby = event.sortField || 'status';
        payload.sortDirection = (event.sortOrder !== undefined && SortOrder[event.sortOrder]) || SortOrder.asc;
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
        this.getEmpowermentsSubscription.add(
            this.authorizationRegisterService.getEmpowerments(payload).subscribe({
                next: (response: IEmpowermentsPaginatedData) => {
                    response.data.forEach((item: IEmpowerment) => {
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

                    this.empowerments = response.data;
                    this.totalRecords = response.totalItems;
                    this.loading = false;
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
            })
        );
    }

    ngOnInit() {
        this.loading = false;
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    onFilteredDataChange(filteredData: any) {
        this.empowermentsFilterState = filteredData;
        // On filter change we always set pagination to first page
        if (this.table.first > 0) {
            this.table.first = 0;
            this.lazyLoadEventState.first = 0;
        }
        this.loadEmpowerments(this.lazyLoadEventState, filteredData);
    }

    ifDateIsExpired(date: Date): boolean {
        if (!date) {
            return false;
        }
        const todayTimestamp = new Date().getTime();
        const expireDateUtc = new Date(date).getTime();
        return expireDateUtc < todayTimestamp;
    }

    showDetails(empower: any) {
        this.selectedEmpowerment = empower;
        this.showWithdrawalMode = false;
        this.showPreview = true;
    }

    private dynamicComponentRef!: ComponentRef<EmpowermentCertificatePrintableComponent>;
    exportEmpowermentCertificate(empower: any, showPrintScreen = false) {
        this.loading = true;
        const componentFactory = this.componentFactoryResolver.resolveComponentFactory(
            EmpowermentCertificatePrintableComponent
        );
        this.dynamicComponentRef = componentFactory.create(this.injector);
        this.dynamicComponentRef.instance.empowerment = empower;
        this.appRef.attachView(this.dynamicComponentRef.hostView);

        setTimeout(() => {
            const renderedHtml = this.dynamicComponentRef.location.nativeElement.innerHTML;
            this.appRef.detachView(this.dynamicComponentRef.hostView);
            this.dynamicComponentRef.destroy();
            this.authorizationRegisterService.exportHtml({ id: empower.id, html: renderedHtml }).subscribe(
                (data: Blob) => {
                    this.loading = false;
                    if (!showPrintScreen) {
                        const downloadUrl = window.URL.createObjectURL(data);
                        const anchor = document.createElement('a');
                        anchor.href = downloadUrl;
                        anchor.download = `${empower.id}.pdf`;
                        anchor.click();
                    } else {
                        const blob = new Blob([data], { type: 'application/pdf' });
                        const blobURL = URL.createObjectURL(blob);
                        let iframe = document.getElementById('printFrame') as HTMLIFrameElement;
                        if (!iframe) {
                            iframe = document.createElement('iframe');
                            iframe.setAttribute('id', 'printFrame');
                            iframe.style.display = 'none';
                            document.body.appendChild(iframe);
                        }
                        iframe.src = blobURL;
                        iframe.onload = function () {
                            setTimeout(function () {
                                iframe.focus();
                                iframe.contentWindow?.print();
                            }, 1);
                        };
                    }
                },
                (error: any) => {
                    this.loading = false;
                    console.error('Download error:', error);
                }
            );
        });
    }

    rowIsNotActive(empower: any): boolean {
        return (
            empower.status === EmpowermentStatementStatus.DisagreementDeclared ||
            empower.status === EmpowermentStatementStatus.Withdrawn ||
            empower.status === EmpowermentStatementStatus.Denied ||
            (empower.status === EmpowermentStatementStatus.Active && this.ifDateIsExpired(empower.expiryDate))
        );
    }

    onHideDetails(eventData: IHidePreviewEventEmitter) {
        if (eventData.refreshTable) {
            this.loadEmpowerments(this.lazyLoadEventState, this.empowermentsFilterState);
        }
        this.showPreview = eventData.showPreview;
        this.showWithdrawalMode = false;
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
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

    ifUpcoming(empowerment: any): boolean {
        let result = false;
        empowerment.statusHistory.find((sH: any) => {
            if (sH.status === EmpowermentStatementStatus.Active) {
                result =
                    new Date(sH.dateTime).getTime() < new Date(empowerment.startDate).getTime() &&
                    new Date().getTime() < new Date(empowerment.startDate).getTime();
            }
        });
        return result;
    }
}
