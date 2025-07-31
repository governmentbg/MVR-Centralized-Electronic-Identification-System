import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import { TranslocoService } from '@ngneat/transloco';
import { EmpowermentsDenialReason, EmpowermentStatementStatus, OnBehalfOf } from '../../enums/empowerment-check.enum';
import { Subscription } from 'rxjs';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { EmpowermentCheckClientService } from '@app/features/empowerment-check/services/empowerment-check-client.service';
import { ToastService } from '@app/shared/services/toast.service';
import {
    IEmpowerment,
    IEmpowermentDenyRequestData,
    ILegalEntityInfoResponse,
    IReason,
} from '@app/features/empowerment-check/interfaces/empowerment-check.interfaces';
import { AuthorizationRegisterService } from '@app/features/empowerment-check/services/authorization-register.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';

@Component({
    selector: 'app-empowerment-check-search-result-details',
    templateUrl: './empowerment-check-search-result-details.component.html',
    styleUrls: ['./empowerment-check-search-result-details.component.scss'],
    providers: [ConfirmationService],
})
export class EmpowermentCheckSearchResultDetailsComponent implements OnInit {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private translocoLocaleService: TranslocoLocaleService,
        private empowermentCheckClientService: EmpowermentCheckClientService,
        private authorizationRegisterService: AuthorizationRegisterService,
        private toastService: ToastService,
        private confirmationService: ConfirmationService
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
        this.buildReasonOfDenyDropdownData();
    }

    searchedEmpowerment: any = {};
    selectedEmpowerment!: IEmpowerment;
    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    onBehalfOf = OnBehalfOf;
    empowermentsDenialReason = EmpowermentsDenialReason;
    empowermentStatementStatus = EmpowermentStatementStatus;
    additionalDataFromRegistries: ILegalEntityInfoResponse | null = null;
    loadingLegalEntityInfo = false;
    loading = false;
    showDenyDialog = false;
    customReasonId = 'OTHER_REASON';
    originalReasonsOfDisagreement: IReason[] = [
        {
            id: 'INVALID_DATA',
            translations: [
                {
                    language: 'bg',
                    name: 'Невалидни данни',
                },
                {
                    language: 'en',
                    name: 'Invalid data',
                },
            ],
        },
        {
            id: 'NO_LONGER_NEEDED',
            translations: [
                {
                    language: 'bg',
                    name: 'Вече не е необходимо',
                },
                {
                    language: 'en',
                    name: 'No longer needed',
                },
            ],
        },
        {
            id: 'OTHER_REASON',
            translations: [
                {
                    language: 'bg',
                    name: 'Друго',
                },
                {
                    language: 'en',
                    name: 'Other',
                },
            ],
        },
    ];
    denialReasons: any[] = [];
    form: FormGroup<{ reason: FormControl<string | null>; customReason: FormControl<string | null> }> = new FormGroup({
        reason: new FormControl<string | null>(null, Validators.required),
        customReason: new FormControl<string | null>(null, Validators.required),
    });

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
        if (this.selectedEmpowerment.onBehalfOf === OnBehalfOf.LegalEntity) {
            this.getLegalEntityData();
        }
    }

    navigateToSearch() {
        this.router.navigate(['/empowerment-check'], {
            state: this.searchedEmpowerment,
        });
    }

    navigateToSearchResults() {
        this.router.navigate(['/empowerment-check/search-results'], {
            skipLocationChange: true,
            state: this.searchedEmpowerment,
        });
    }

    ifDateIsExpired(date: string | null): boolean {
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
            case EmpowermentStatementStatus.Unconfirmed:
                return this.translateService.translate('empowermentCheckModule.detailsPage.txtUnconfirmedOn');
            default:
                return;
        }
    }

    getLegalEntityData() {
        this.loadingLegalEntityInfo = true;
        this.empowermentCheckClientService.getLegalEntityInfo({ uic: this.selectedEmpowerment.uid }).subscribe({
            next: response => {
                this.loadingLegalEntityInfo = false;
                this.additionalDataFromRegistries = response;
            },
            error: error => {
                switch (error.status) {
                    case 400:
                        this.toastService.showErrorToast(
                            this.translateService.translate('global.txtErrorTitle'),
                            this.translateService.translate('global.txtInvalidDataError')
                        );
                        break;
                    default:
                        this.toastService.showErrorToast(
                            this.translateService.translate('global.txtErrorTitle'),
                            this.translateService.translate('global.txtUnexpectedError')
                        );
                        break;
                }
                this.loadingLegalEntityInfo = false;
            },
        });
    }

    buildReasonOfDenyDropdownData(): void {
        this.denialReasons = this.originalReasonsOfDisagreement.map((reason: any) => {
            return {
                id: reason.id,
                name: reason.translations.find(
                    (translation: any) => translation.language === this.translateService.getActiveLang()
                )?.name,
            };
        });
    }

    onReasonChange(formName: FormGroup): void {
        if (formName.controls['reason'].value) {
            formName.controls['customReason'].setValidators(
                formName.controls['reason'].value === this.customReasonId
                    ? [Validators.required, Validators.maxLength(256)]
                    : null
            );
            formName.controls['customReason'].updateValueAndValidity();
        }
    }

    approveEmpowerment() {
        this.confirmationService.confirm({
            rejectButtonStyleClass: 'p-button-danger',
            message: this.translateService.translate(
                'empowermentCheckModule.detailsPage.txtConfirmApproveEmpowermentMessage'
            ),
            header: this.translateService.translate('empowermentCheckModule.detailsPage.txtConfirmApproveEmpowerment'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.loading = true;
                this.empowermentCheckClientService
                    .approveEmpowerment({ empowermentId: this.selectedEmpowerment.id })
                    .subscribe({
                        next: () => {
                            this.loading = false;
                            this.showSuccessToast(
                                this.translateService.translate(
                                    'empowermentCheckModule.detailsPage.txtApplicationApproveSuccess'
                                )
                            );
                            this.navigateToSearchResults();
                        },
                        error: (error: any) => {
                            this.loading = false;
                            switch (error.status) {
                                case 400:
                                    this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                    break;
                                default:
                                    this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                                    break;
                            }
                        },
                    });
            },
        });
    }
    denyEmpowerment() {
        this.showDenyDialog = true;
    }

    onDataSubmit() {
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
            this.loading = true;
            const denyData: IEmpowermentDenyRequestData = {
                empowermentId: this.selectedEmpowerment.id,
                denialReasonComment: this.form.controls.reason.value as string,
            };
            if (this.form.controls.customReason.value) {
                denyData['denialReasonComment'] = this.form.controls.customReason.value;
            }
            this.empowermentCheckClientService.denyEmpowerment(denyData).subscribe({
                next: () => {
                    this.showDenyDialog = false;
                    this.loading = false;
                    this.showSuccessToast(
                        this.translateService.translate('empowermentCheckModule.detailsPage.txtApplicationDenySuccess')
                    );
                    this.navigateToSearchResults();
                },
                error: (error: any) => {
                    this.loading = false;
                    switch (error.status) {
                        case 400:
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        default:
                            this.showErrorToast(this.translateService.translate('global.txtUnexpectedError'));
                            break;
                    }
                },
            });
        }
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string): void {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }
}
