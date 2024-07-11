import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { ToastService } from 'src/app/shared/services/toast.service';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { EmpowermentCheckClientService } from '../../services/empowerment-check-client.service';
import { OnBehalfOf } from '../../enums/empowerment-check.enum';
import { ICalculatedEmpowermentStatus } from '../../interfaces/ICalculatedEmpowermentStatus';
import * as moment from 'moment';
import { IEmpowerment } from '../../../authorization-register/interfaces/authorization-register.interfaces';
import { EmpowermentXmlParserService } from '../../../authorization-register/services/empowerment-xml-parser.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';

@Component({
    selector: 'app-empowerment-check-search-results',
    templateUrl: './empowerment-check-search-results.component.html',
    styleUrls: ['./empowerment-check-search-results.component.scss'],
})
export class EmpowermentCheckSearchResultsComponent implements OnInit {
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
        if (navigation) {
            const state = navigation.extras.state as any;
            if (state) {
                this.searchedEmpowerment = state;
            }
        }
    }

    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading = false;
    empowerments: any[] = [];
    searchedEmpowerment: any = {};
    empowermentStatus = ICalculatedEmpowermentStatus;

    ngOnInit() {
        this.breadcrumbItems = [
            {
                label: this.translateService.translate('empowermentCheckModule.txtReferences'),
                onClick: this.navigateToSearch.bind(this),
            },
            { label: this.translateService.translate('empowermentCheckModule.txtEmpowerment') },
        ];
        this.prepareRequest();
    }

    prepareRequest() {
        const payload: any = {
            serviceId: this.searchedEmpowerment.service?.serviceNumber,
            empoweredUid: this.searchedEmpowerment.empoweredUid,
            empoweredUidType: this.searchedEmpowerment.empoweredUidType,
            authorizerUidType: this.searchedEmpowerment.authorizerUidType,
            authorizerUid: this.searchedEmpowerment.authorizerUid,
            onBehalfOf: this.searchedEmpowerment.onBehalfOf,
            statusOn: moment(this.searchedEmpowerment.statusOn).endOf('day').toISOString(),
            volumeOfRepresentation: this.searchedEmpowerment.volumeOfRepresentation,
            pageIndex: 1,
            pageSize: 50,
        };
        if (
            this.searchedEmpowerment.volumeOfRepresentation !== null &&
            this.searchedEmpowerment.volumeOfRepresentation !== undefined &&
            this.searchedEmpowerment.volumeOfRepresentation.length > 0
        ) {
            payload.volumeOfRepresentation = payload.volumeOfRepresentation.map(({ code }: any) => code);
        }

        if (this.searchedEmpowerment.onBehalfOf === null) {
            payload.onBehalfOf = OnBehalfOf.Empty;
        }

        this.loadEmpowerments(payload);
    }

    loadEmpowerments(payload: any) {
        this.loading = true;
        this.empowermentCheckClientService.empowermentCheck(payload).subscribe({
            next: async (response: any) => {
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
                    item.supplierId = xmlRepresentation.supplierId;
                    item.supplierName = xmlRepresentation.supplierName;
                });
                this.empowerments = response.data;
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
        });
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    navigateToSearch() {
        this.router.navigate(['/empowerment-check/search'], {
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
            return this.translocoLocaleService.localizeDate(
                expiryDate,
                this.translocoLocaleService.getLocale()
            );
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
