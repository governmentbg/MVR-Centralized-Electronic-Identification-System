import { Component, Input, ViewEncapsulation } from '@angular/core';
import {
    EmpowermentsDenialReason,
    EmpowermentStatementStatus,
    OnBehalfOf,
} from '../../enums/authorization-register.enum';
import { IEmpowerment } from '../../interfaces/authorization-register.interfaces';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';

@Component({
    selector: 'app-empowerment-certificate-printable',
    templateUrl: './empowerment-certificate-printable.component.html',
    styleUrls: ['./empowerment-certificate-printable.component.scss'],
})
export class EmpowermentCertificatePrintableComponent {
    @Input() showWithdrawalMode = false;
    @Input() showDisagreementMode = false;
    @Input() empowerment: IEmpowerment = {
        id: '',
        uid: '',
        status: '',
        authorizer: '',
        providerName: '',
        serviceName: '',
        authorizerUids: [],
        empoweredUids: [],
        expiryDate: '',
        modifiedBy: '',
        modifiedOn: '',
        name: '',
        onBehalfOf: null,
        serviceId: 0,
        startDate: '',
        providerId: '0',
        volumeOfRepresentation: [],
        empowermentWithdrawals: [],
        issuerPosition: null,
        xmlRepresentation: '',
        denialReason: EmpowermentsDenialReason.None,
        createdOn: '',
        statusHistory: [],
        number: '',
        empowermentSignatures: [],
    };

    empowermentStatementStatus = EmpowermentStatementStatus;
    onBehalfOf = OnBehalfOf;
    EmpowermentsDenialReason = EmpowermentsDenialReason;

    constructor(public translateService: TranslocoService, private translocoLocaleService: TranslocoLocaleService) {}

    shouldShowWithdrawalReasonForm() {
        return this.empowerment.empowermentWithdrawals && this.empowerment.empowermentWithdrawals.length === 0;
    }

    ifDateIsExpired(date: string | null): boolean {
        if (!date) {
            return false;
        }
        const todayTimestamp = new Date().getTime();
        const expireDateUtc = new Date(date).getTime();
        return expireDateUtc < todayTimestamp;
    }

    computedStatus(status: any) {
        return this.translateService.translate('modules.authorizationRegister.tableFilter.statuses.' + status);
    }

    getHistoryItems(empowerment: IEmpowerment) {
        const historyItems = empowerment.statusHistory.map((historyItem: any) => {
            return {
                status: historyItem.status,
                value: this.translocoLocaleService.localizeDate(
                    historyItem.dateTime,
                    this.translocoLocaleService.getLocale(),
                    { timeStyle: 'medium' }
                ),
                statusTranslation: this.statusTranslation(historyItem.status),
                rawDate: historyItem.dateTime,
            };
        });

        const authorizerNameByUid: Record<string, string> = {};
        empowerment.authorizerUids.forEach((authorizer: any) => {
            if (!(authorizer.uid in authorizerNameByUid)) {
                authorizerNameByUid[authorizer.uid] = authorizer.name ?? '';
            }
        });
        let signatureItems: ConcatArray<{
            status: any;
            value: string;
            statusTranslation: string | undefined;
            rawDate: any;
        }> = [];
        if (empowerment.empowermentSignatures) {
            signatureItems = empowerment.empowermentSignatures.map((signature: any) => {
                let signatureValue = this.translocoLocaleService.localizeDate(
                    signature.dateTime,
                    this.translocoLocaleService.getLocale(),
                    { timeStyle: 'medium' }
                );
                if (authorizerNameByUid[signature.signerUid] !== '') {
                    signatureValue += `<br>${authorizerNameByUid[signature.signerUid]}`;
                }
                return {
                    status: signature.status,
                    value: signatureValue,
                    statusTranslation: this.translateService.translate(
                        'empowermentCheckModule.detailsPage.txtSignedOn'
                    ),
                    rawDate: signature.dateTime,
                };
            });
        }
        return historyItems
            .concat(signatureItems)
            .sort((a, b) => new Date(b.rawDate).getTime() - new Date(a.rawDate).getTime());
    }

    get empowermentStartDate() {
        if (this.empowerment && this.empowerment.startDate) {
            return this.translocoLocaleService.localizeDate(
                this.empowerment.startDate,
                this.translocoLocaleService.getLocale(),
                { timeStyle: 'medium' }
            );
        }
        return;
    }

    get empowermentExpiryDate() {
        if (this.empowerment.expiryDate) {
            return this.translocoLocaleService.localizeDate(
                this.empowerment.expiryDate,
                this.translocoLocaleService.getLocale(),
                { timeStyle: 'medium' }
            );
        }
        return this.translateService.translate('modules.authorizationRegister.tableFilter.txtIndefinitely');
    }

    ifUpcoming(): boolean {
        let result = false;
        this.empowerment.statusHistory.find((sH: any) => {
            if (sH.status === EmpowermentStatementStatus.Active) {
                result =
                    new Date(sH.dateTime).getTime() < new Date(this.empowerment.startDate).getTime() &&
                    new Date().getTime() < new Date(this.empowerment.startDate).getTime();
            }
        });
        return result;
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
            case EmpowermentStatementStatus.Unconfirmed:
                return this.translateService.translate('empowermentCheckModule.detailsPage.txtUnconfirmedOn');
            default:
                return;
        }
    }
}
