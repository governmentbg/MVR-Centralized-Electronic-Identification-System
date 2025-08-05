import { Component, EventEmitter, Input, OnChanges, OnDestroy, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import {
    IEmpowermentWithSignature,
    IHidePreviewEventEmitter,
} from '../../interfaces/authorization-register.interfaces';
import {
    EmpowermentsDenialReason,
    EmpowermentStatementStatus,
    OnBehalfOf,
} from '../../enums/authorization-register.enum';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthorizationRegisterService } from '../../services/authorization-register.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { IUser } from 'src/app/core/interfaces/IUser';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { ConfirmationService } from 'primeng/api';
@Component({
    selector: 'app-empowerment-preview',
    templateUrl: './empowerment-preview.component.html',
    styleUrls: ['./empowerment-preview.component.scss'],
    providers: [ConfirmationService],
})
export class EmpowermentPreviewComponent implements OnChanges, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private authorizationRegisterService: AuthorizationRegisterService,
        private toastService: ToastService,
        private translocoLocaleService: TranslocoLocaleService,
        private confirmationService: ConfirmationService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                    routerLink: '/authorization-register',
                },
                {
                    label: this.isFromMePage
                        ? this.translateService.translate('modules.authorizationRegister.txtFromMe')
                        : this.translateService.translate('modules.authorizationRegister.txtToMe'),
                    onClick: this.hidePreview.bind(this),
                },
                { label: this.translateService.translate('modules.authorizationRegister.txtAuthorization') },
            ];
        });
    }

    @Input() isFromMePage = false;
    @Output() hide: EventEmitter<IHidePreviewEventEmitter> = new EventEmitter<IHidePreviewEventEmitter>();
    @Input() empowerment: IEmpowermentWithSignature = {
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
    @Input() showWithdrawalMode = false;
    @Input() showDisagreementMode = false;
    breadcrumbItems: IBreadCrumbItems[] = [];
    languageChangeSubscription: Subscription;
    withdrawalSubscription: Subscription | undefined;
    onBehalfOf = OnBehalfOf;
    requestInProgress = false;
    form = new FormGroup({
        reason: new FormControl<string>(
            this.translateService.translate('modules.authorizationStatement.statementForm.txtDefaultWithdrawalReason'),
            [Validators.required]
        ),
    });
    disagreementForm = new FormGroup({
        reason: new FormControl<string>(
            this.translateService.translate('modules.authorizationStatement.statementForm.txtDefaultWithdrawalReason'),
            [Validators.required]
        ),
    });
    EmpowermentsDenialReason = EmpowermentsDenialReason;
    empowermentStatementStatus = EmpowermentStatementStatus;
    user: IUser = {
        userId: '',
        locale: '',
        uid: '',
        uidType: '',
        name: '',
        acr: '',
    };
    redirectionPopupCountdownSeconds = 15;

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

    computedStatus(status: any) {
        return this.translateService.translate('modules.authorizationRegister.tableFilter.statuses.' + status);
    }

    ngOnChanges() {
        this.requestInProgress = false;
        if (this.empowerment && this.empowerment.number) {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                    routerLink: '/authorization-register',
                },
                {
                    label: this.isFromMePage
                        ? this.translateService.translate('modules.authorizationRegister.txtFromMe')
                        : this.translateService.translate('modules.authorizationRegister.txtToMe'),
                    onClick: this.hidePreview.bind(this),
                },
                {
                    label: `${this.translateService.translate('modules.authorizationRegister.txtAuthorization')} ${
                        this.empowerment.number
                    }`,
                },
            ];
        }
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    hidePreview(refreshTable = false): void {
        if (this.withdrawalSubscription) {
            this.withdrawalSubscription.unsubscribe();
            this.form.enable();
        }
        this.navigateBack(refreshTable);
    }

    showSignatureSection(): boolean {
        return (
            this.empowerment &&
            this.empowerment.status === EmpowermentStatementStatus.CollectingAuthorizerSignatures &&
            !this.showWithdrawalMode
        );
    }

    showErrorToast(message: string): void {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string): void {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    submitWithdrawal(): void {
        this.form.markAllAsTouched();
        this.form.controls.reason.markAsDirty();
        if (this.form.valid) {
            this.requestInProgress = true;
            this.form.disable();
            const reason = this.form.controls.reason.value as string;
            this.withdrawalSubscription = this.authorizationRegisterService
                .withdrawEmpowerment({
                    id: this.empowerment.id,
                    reason: reason.trim(),
                })
                .subscribe({
                    next: () => {
                        this.showSuccessToast(
                            this.translateService.translate(
                                'modules.authorizationStatement.statementForm.txtStatementInProgressMessage'
                            )
                        );
                        this.hidePreview(true);
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
                        this.requestInProgress = false;
                        this.form.enable();
                    },
                });
        }
    }

    submitDisagreement() {
        this.disagreementForm.markAllAsTouched();
        this.disagreementForm.controls.reason.markAsDirty();
        if (this.disagreementForm.valid) {
            this.requestInProgress = true;
            this.disagreementForm.disable();
            const reason = this.disagreementForm.controls.reason.value as string;
            this.withdrawalSubscription = this.authorizationRegisterService
                .declareDisagreementEmpowerment({
                    id: this.empowerment.id,
                    reason: reason.trim(),
                })
                .subscribe({
                    next: () => {
                        this.showSuccessToast(
                            this.translateService.translate(
                                'modules.authorizationStatement.statementForm.txtStatementInProgressMessage'
                            )
                        );
                        this.hidePreview();
                    },
                    error: error => {
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            case 403:
                                this.showErrorToast(
                                    this.translateService.translate(
                                        'modules.authorizationRegister.txtDeclareDisagreementPermissionMessage'
                                    )
                                );
                                break;
                            default:
                                this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                                break;
                        }
                        this.requestInProgress = false;
                        this.disagreementForm.enable();
                    },
                });
        }
    }

    shouldShowWithdrawalReasonForm() {
        return this.empowerment.empowermentWithdrawals && this.empowerment.empowermentWithdrawals.length === 0;
    }

    shouldShowDisagreementReasonForm() {
        return this.showDisagreementMode;
    }

    ifDateIsExpired(date: string | null): boolean {
        if (!date) {
            return false;
        }
        const todayTimestamp = new Date().getTime();
        const expireDateUtc = new Date(date).getTime();
        return expireDateUtc < todayTimestamp;
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

    getHeaderText() {
        if (this.showWithdrawalMode) {
            return this.translateService.translate('modules.authorizationRegister.txAuthorizationWithdrawal');
        } else if (this.showDisagreementMode) {
            return this.translateService.translate('modules.authorizationRegister.txtDeclareDisagreementTitle');
        } else {
            return this.translateService.translate(
                'modules.authorizationRegister.tableFilter.details.txtPreviewEmpowerment'
            );
        }
    }

    getHistoryItems(empowerment: IEmpowermentWithSignature) {
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

    showReloadDataInformationPopup(refreshTable: boolean) {
        this.confirmationService.confirm({
            message: this.translateService.translate('empowermentCheckModule.detailsPage.txtReloadDataPopupText'),
            accept: () => {
                this.navigateBack(refreshTable);
            },
        });

        const timerId = setInterval(() => {
            this.redirectionPopupCountdownSeconds--;
            if (this.redirectionPopupCountdownSeconds <= 0) {
                clearInterval(timerId);
                this.navigateBack(refreshTable);
            }
        }, 1000);
    }

    navigateBack(refreshTable: boolean): void {
        this.hide.emit({ showPreview: false, refreshTable: refreshTable });
    }
}
