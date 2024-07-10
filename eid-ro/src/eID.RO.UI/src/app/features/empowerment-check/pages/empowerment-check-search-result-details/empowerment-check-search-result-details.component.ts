import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { TranslocoService } from '@ngneat/transloco';
import { EmpowermentStatementStatus, OnBehalfOf } from '../../enums/empowerment-check.enum';
import { Subscription } from 'rxjs';
import { EmpowermentsDenialReason } from '../../../authorization-register/enums/authorization-register.enum';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';

@Component({
    selector: 'app-empowerment-check-search-result-details',
    templateUrl: './empowerment-check-search-result-details.component.html',
    styleUrls: ['./empowerment-check-search-result-details.component.scss'],
})
export class EmpowermentCheckSearchResultDetailsComponent implements OnInit {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private translocoLocaleService: TranslocoLocaleService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('empowermentCheckModule.txtReferences'),
                    onClick: this.navigateToSearch.bind(this),
                },
                {
                    label: this.translateService.translate('empowermentCheckModule.txtEmpowerments'),
                    onClick: this.navigateToSearchResults.bind(this),
                },
                { label: this.translateService.translate('empowermentCheckModule.txtPreview') },
            ];
        });
        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            const state = navigation.extras.state as any;
            if (state) {
                this.selectedEmpowerment = state.selectedEmpowerment;
                this.searchedEmpowerment = state.searchedEmpowerment;
            }
        }
    }

    searchedEmpowerment: any = {};
    selectedEmpowerment: any = {};
    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    onBehalfOf = OnBehalfOf;
    empowermentsDenialReason = EmpowermentsDenialReason;
    empowermentStatementStatus = EmpowermentStatementStatus;

    get empowermentCreatedOnDate() {
        if (this.selectedEmpowerment && this.selectedEmpowerment.createdOn) {
            return this.translocoLocaleService.localizeDate(
                this.selectedEmpowerment.createdOn,
                this.translocoLocaleService.getLocale(),
                { timeStyle: 'medium' }
            );
        }
        return;
    }

    get empowermentStartDate() {
        if (this.selectedEmpowerment && this.selectedEmpowerment.startDate) {
            return this.translocoLocaleService.localizeDate(
                this.selectedEmpowerment.startDate,
                this.translocoLocaleService.getLocale(),
                { timeStyle: 'medium' }
            );
        }
        return;
    }

    get empowermentExpiryDate() {
        if (this.selectedEmpowerment.expiryDate) {
            return this.translocoLocaleService.localizeDate(
                this.selectedEmpowerment.expiryDate,
                this.translocoLocaleService.getLocale(),
                { timeStyle: 'medium' }
            );
        }
        return this.translateService.translate('empowermentCheckModule.txtIndefinitely');
    }

    get empowermentWithdrawalsLastDate() {
        if (
            this.selectedEmpowerment.empowermentWithdrawals &&
            this.selectedEmpowerment.empowermentWithdrawals.length > 0 &&
            this.selectedEmpowerment.empowermentWithdrawals[0].activeDateTime
        ) {
            return this.translocoLocaleService.localizeDate(
                this.selectedEmpowerment.empowermentWithdrawals[0].activeDateTime,
                this.translocoLocaleService.getLocale(),
                { timeStyle: 'medium' }
            );
        }
        return;
    }

    get empowermentDisagreementsLastDate() {
        if (
            this.selectedEmpowerment.empowermentDisagreements &&
            this.selectedEmpowerment.empowermentDisagreements.length > 0 &&
            this.selectedEmpowerment.empowermentDisagreements[0].activeDateTime
        ) {
            return this.translocoLocaleService.localizeDate(
                this.selectedEmpowerment.empowermentDisagreements[0].activeDateTime,
                this.translocoLocaleService.getLocale(),
                { timeStyle: 'medium' }
            );
        }
        return;
    }

    get serviceNameAndNumber() {
        if (this.selectedEmpowerment.serviceId && this.selectedEmpowerment.serviceName) {
            return `${this.selectedEmpowerment.serviceId} - ${this.selectedEmpowerment.serviceName}`;
        }
        return;
    }

    ngOnInit() {
        this.breadcrumbItems = [
            {
                label: this.translateService.translate('empowermentCheckModule.txtReferences'),
                onClick: this.navigateToSearch.bind(this),
            },
            {
                label: this.translateService.translate('empowermentCheckModule.txtEmpowerments'),
                onClick: this.navigateToSearchResults.bind(this),
            },
            { label: this.translateService.translate('empowermentCheckModule.txtPreview') },
        ];
    }

    navigateToSearch() {
        this.router.navigate(['/empowerment-check/search'], {
            state: this.searchedEmpowerment,
        });
    }

    navigateToSearchResults() {
        this.router.navigate(['/empowerment-check/search-results'], {
            skipLocationChange: true,
            state: this.searchedEmpowerment,
        });
    }

    ifDateIsExpired(date: Date): boolean {
        if (!date) {
            return false;
        }
        const todayTimestamp = new Date().getTime();
        const expireDateUtc = new Date(date).getTime();
        return expireDateUtc < todayTimestamp;
    }

    getWithdrawalsReason(selectedEmpowerment: any) {
        if (
            selectedEmpowerment &&
            selectedEmpowerment.empowermentWithdrawals &&
            selectedEmpowerment.empowermentWithdrawals.length > 0
        ) {
            return selectedEmpowerment.empowermentWithdrawals[0].reason;
        } else {
            return '';
        }
    }

    getDisagreementsDeclaredReason(selectedEmpowerment: any) {
        if (
            selectedEmpowerment &&
            selectedEmpowerment.empowermentDisagreements &&
            selectedEmpowerment.empowermentDisagreements.length > 0
        ) {
            return selectedEmpowerment.empowermentDisagreements[0].reason;
        } else {
            return '';
        }
    }

    ifUpcoming(): boolean {
        let result = false;
        this.selectedEmpowerment.statusHistory.find((sH: any) => {
            if (sH.status === EmpowermentStatementStatus.Active) {
                const startDate = new Date(this.selectedEmpowerment.startDate).getTime();
                const today = new Date(new Date().setHours(0, 0, 0, 0)).getTime();
                const dateOfSigning = new Date(sH.dateTime).getTime();
                result = dateOfSigning < startDate && startDate > today;
            }
        });
        return result;
    }

    getEmpowermentStatusTranslation(status: any) {
        return this.translateService.translate('empowermentCheckModule.statuses.' + status);
    }

    statusTranslation(status: EmpowermentStatementStatus) {
        switch (status) {
            case EmpowermentStatementStatus.Active:
                if (this.ifUpcoming()) {
                    return this.translateService.translate('empowermentCheckModule.detailsPage.txtSignedOn');
                } else {
                    return this.translateService.translate('empowermentCheckModule.detailsPage.txtEffectiveOn');
                }
            case EmpowermentStatementStatus.Created:
                return this.translateService.translate('empowermentCheckModule.detailsPage.txtSubmittedTo');
            case EmpowermentStatementStatus.Denied:
                return this.translateService.translate('empowermentCheckModule.detailsPage.txtDeniedOn');
            case EmpowermentStatementStatus.Expired:
                return this.translateService.translate('empowermentCheckModule.detailsPage.txtExpiresOn');
            case EmpowermentStatementStatus.Withdrawn:
                return this.translateService.translate('empowermentCheckModule.detailsPage.txtWithdrawnOn');
            case EmpowermentStatementStatus.DisagreementDeclared:
                return this.translateService.translate('empowermentCheckModule.detailsPage.txtDeclaredDisagreementOf');
            default:
                return;
        }
    }
}
