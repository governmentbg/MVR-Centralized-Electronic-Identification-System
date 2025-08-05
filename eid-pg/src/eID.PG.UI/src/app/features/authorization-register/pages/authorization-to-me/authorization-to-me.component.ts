import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ConfirmationService, LazyLoadEvent, MenuItem } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { Table } from 'primeng/table';
import { EmpowermentStatementStatus, SortOrder } from '../../enums/authorization-register.enum';
import {
    IEmpowerment,
    IEmpowermentsPaginatedData,
    IHidePreviewEventEmitter,
} from '../../interfaces/authorization-register.interfaces';
import { AuthorizationRegisterService } from '../../services/authorization-register.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { EmpowermentXmlParserService } from '../../services/empowerment-xml-parser.service';
import * as moment from 'moment/moment';

@Component({
    selector: 'app-authorization-to-me',
    templateUrl: './authorization-to-me.component.html',
    styleUrls: ['./authorization-to-me.component.scss'],
    providers: [ConfirmationService],
})
export class AuthorizationToMeComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        public authorizationRegisterService: AuthorizationRegisterService,
        private toastService: ToastService,
        private confirmationService: ConfirmationService,
        private empowermentXmlParserService: EmpowermentXmlParserService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                    routerLink: '/authorization-register',
                },
                { label: this.translateService.translate('modules.authorizationRegister.txtControlAuthorization') },
            ];
        });
    }

    @ViewChild('dt') table!: Table;

    breadcrumbItems: MenuItem[] = [];
    empowerments: any[] = [];
    totalRecords = 0;
    pageSize = 10;
    loading!: boolean;
    lazyLoadEventState: LazyLoadEvent = {};
    empowermentsFilterState: any = {};
    languageChangeSubscription: Subscription = new Subscription();
    getEmpowermentsSubscription: Subscription = new Subscription();
    selectedEmpowerment: any;
    showPreview = false;
    showDisagreementMode = false;
    moment = moment;

    ngOnInit() {
        this.breadcrumbItems = [
            {
                label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                routerLink: '/authorization-register',
            },
            { label: this.translateService.translate('modules.authorizationRegister.txtControlAuthorization') },
        ];
        this.loading = true;
    }

    loadEmpowerments(event: LazyLoadEvent, empowermentsFilterData?: any): void {
        this.lazyLoadEventState = event;
        this.loading = true;
        this.totalRecords = 0;

        let payload: any = {};
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

        this.getEmpowermentsSubscription.add(
            this.authorizationRegisterService.getEmpowermentsTo(payload).subscribe({
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
        this.showDisagreementMode = false;
        this.showPreview = true;
    }

    declareDisagreement(empower: any) {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            message: this.translateService.translate(
                'modules.authorizationRegister.txtDeclareDisagreementConfirmMessage'
            ),
            header: this.translateService.translate('modules.authorizationRegister.txtDeclareDisagreementTitle'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.selectedEmpowerment = empower;
                this.showDisagreementMode = true;
                this.showPreview = true;
            },
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
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
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

    isDownloadButtonVisible(empower: any) {
        return (
            empower.status === EmpowermentStatementStatus.Active ||
            empower.status === EmpowermentStatementStatus.DisagreementDeclared ||
            empower.status === EmpowermentStatementStatus.Expired ||
            empower.status === EmpowermentStatementStatus.Withdrawn ||
            empower.status === EmpowermentStatementStatus.Unconfirmed
        );
    }

    downloadEmpowerment(empower: any) {
        this.loading = true;
        this.authorizationRegisterService
            .getEmpowermentToMe({
                empowermentId: empower.id,
            })
            .subscribe({
                next: response => {
                    this.loading = false;
                    this.downloadPDFBlob(response);
                },
                error: error => {
                    this.loading = false;
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        default:
                            this.showErrorToast(this.translateService.translate('global.txtPrintApplicationError'));
                            break;
                    }
                },
            });
    }

    downloadPDFBlob(data: Blob) {
        const url = window.URL.createObjectURL(data);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'empowerment.pdf';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }
}
