import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService, LazyLoadEvent } from 'primeng/api';
import { AuthorizationRegisterService } from '../../services/authorization-register.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { EmpowermentXmlParserService } from '../../services/empowerment-xml-parser.service';
import { Subscription } from 'rxjs';
import { EmpowermentStatementStatus, SortOrder } from '../../enums/authorization-register.enum';
import {
    IEmpowerment,
    IEmpowermentsPaginatedData,
    IHidePreviewEventEmitter,
} from '../../interfaces/authorization-register.interfaces';
import { Table } from 'primeng/table';
import { BulstatValidator } from '../../../../shared/validators/bulstat';
import { EmpowermentsFilterComponent } from '../../components/empowerments-filter/empowerments-filter.component';

@Component({
    selector: 'app-authorization-for-legal-entity',
    templateUrl: './authorization-for-legal-entity.component.html',
    styleUrls: ['./authorization-for-legal-entity.component.scss'],
    providers: [ConfirmationService],
})
export class AuthorizationForLegalEntityComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private confirmationService: ConfirmationService,
        private authorizationRegisterService: AuthorizationRegisterService,
        private toastService: ToastService,
        private empowermentXmlParserService: EmpowermentXmlParserService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                    routerLink: '/authorization-register',
                },
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtFromMe'),
                    routerLink: '/authorization-register/from-me',
                },
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtLegalEntityCheck'),
                },
            ];
        });
    }

    @ViewChild('dt') table!: Table;
    @ViewChild('empowermentsFilter') empowermentsFilter!: EmpowermentsFilterComponent;
    breadcrumbItems: IBreadCrumbItems[] = [];
    empowerments: any[] = [];
    totalRecords = 0;
    pageSize = 10;
    loading!: boolean;
    lazyLoadEventState: LazyLoadEvent = {};
    empowermentsFilterState: any = {};
    languageChangeSubscription: Subscription = new Subscription();
    getEmpowermentsSubscription: Subscription = new Subscription();
    submitted = false;
    selectedEmpowerment: any;
    showPreview = false;
    showWithdrawalMode = false;
    showEmpowermentResults = false;
    requestInProgress = false;
    form = new FormGroup({
        eik: new FormControl<string | null>(null, [Validators.required, BulstatValidator()]),
    });
    currentEik = '';

    ngOnInit() {
        this.breadcrumbItems = [
            {
                label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                routerLink: '/authorization-register',
            },
            {
                label: this.translateService.translate('modules.authorizationRegister.txtFromMe'),
                routerLink: '/authorization-register/from-me',
            },
            {
                label: this.translateService.translate('modules.authorizationRegister.txtLegalEntityCheck'),
            },
        ];
        this.loading = true;
    }

    loadEmpowerments(event: LazyLoadEvent, empowermentsFilterData?: any): void {
        this.lazyLoadEventState = event;
        this.loading = true;
        this.totalRecords = 0;

        let payload: any = { eik: this.form.controls.eik.value };
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
            this.authorizationRegisterService.getEmpowermentsForLegalEntity(payload).subscribe({
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
                    this.requestInProgress = false;
                },
                error: error => {
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        case 403:
                            this.showErrorToast(this.translateService.translate('global.txtNoPermissionsMessage'));
                            break;
                        default:
                            this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                            break;
                    }
                    this.loading = false;
                    this.requestInProgress = false;
                },
            })
        );
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
        this.getEmpowermentsSubscription.unsubscribe();
    }

    onSubmit() {
        this.submitted = true;
        this.form.markAllAsTouched();
        Object.values(this.form.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    innerControl.markAsDirty();
                });
            } else {
                control.markAsDirty();
            }
        });
        if (this.form.valid) {
            this.currentEik = this.form.controls.eik.value as string;
            this.requestInProgress = true;
            // When the table with results is already visible, the filters should be reset and the new results should be loaded
            if (this.showEmpowermentResults) {
                this.empowermentsFilter.clearAllFilteredValues();
            } else {
                this.showEmpowermentResults = true;
            }
        }
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

    withdrawalEmpowerment(empowerment: any) {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            message: this.translateService.translate('modules.authorizationRegister.txtAuthorizationWithdrawalWarning'),
            header: this.translateService.translate('modules.authorizationRegister.txAuthorizationWithdrawal'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.selectedEmpowerment = empowerment;
                this.showPreview = true;
                this.showWithdrawalMode = true;
            },
        });
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

    showWithdrawalButton(empowerment: any) {
        return (
            empowerment &&
            (empowerment.status === EmpowermentStatementStatus.Active ||
                empowerment.status === EmpowermentStatementStatus.Unconfirmed) &&
            !this.ifDateIsExpired(empowerment.expiryDate)
        );
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
