import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { Subscription, first } from 'rxjs';
import { IHidePreviewEventEmitter } from 'src/app/features/authorization-register/interfaces/authorization-register.interfaces';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CertificateStatus, PermittedUser, ReasonNamesByStatus } from '../../enums/eid-management';
import { ICertificate, IHistoryItem, IReason } from '../../interfaces/eid-management.interface';
import { EidManagementService } from '../../services/eid-management.service';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';

@Component({
    selector: 'app-certificates-history',
    templateUrl: './certificates-history.component.html',
    styleUrls: ['./certificates-history.component.scss'],
})
export class CertificatesHistoryComponent implements OnInit, OnDestroy, OnChanges {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private eidManagement: EidManagementService,
        private toastService: ToastService,
        private translocoLocaleService: TranslocoLocaleService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            if (this.certificate) {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.eidManagement.txtTitle'),
                        onClick: this.navigateToSearch.bind(this),
                    },
                    {
                        label: this.translateService.translate('modules.eidManagement.txtCertificates'),
                        onClick: this.navigateToSearchResults.bind(this),
                    },
                    {
                        label: `${this.translateService.translate(
                            'modules.eidManagement.tableFilter.txtCertificate'
                        )} ${this.certificate.serialNumber}`,
                        onClick: () => this.closePreview(),
                    },
                    {
                        label: this.translateService.translate('modules.eidManagement.txtCertificateHistory'),
                    },
                ];
            }
        });
    }

    @Output() hide: EventEmitter<IHidePreviewEventEmitter> = new EventEmitter<IHidePreviewEventEmitter>();
    @Input() certificate!: ICertificate;
    @Input() fullBreadcrumb = true;
    @Input() certificateId!: string | any;
    @Input() originalReasons!: IReason[];
    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading = false;
    certificateHistory!: IHistoryItem[];
    originalReasonName: string | undefined;
    CertificateStatus = CertificateStatus;

    ngOnInit() {
        this.loading = true;
        if (!this.fullBreadcrumb) {
            this.breadcrumbItems.splice(1, 1);
        }
        this.getCertificateHistory();
    }

    ngOnChanges() {
        if (this.certificate && this.certificate.id) {
            this.loading = true;
            this.getCertificateHistory();
        }
        if (this.certificate && this.certificate.id) {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtTitle'),
                    onClick: this.navigateToSearch.bind(this),
                },
                {
                    label: this.translateService.translate('modules.eidManagement.txtCertificates'),
                    onClick: this.navigateToSearchResults.bind(this),
                },
                {
                    label: `${this.translateService.translate('modules.eidManagement.tableFilter.txtCertificate')} ${
                        this.certificate.serialNumber
                    }`,
                    onClick: () => this.closePreview(),
                },
                {
                    label: this.translateService.translate('modules.eidManagement.txtCertificateHistory'),
                },
            ];
        }
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    formatDate(date: string | null): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
            timeStyle: 'medium',
        });
    }

    navigateToSearch() {
        this.router.navigate(['/eid-management']);
    }

    navigateToSearchResults() {
        this.hide.emit({ showPreview: false, refreshTable: true });
    }

    closePreview() {
        this.hide.emit({ showPreview: false, refreshTable: false });
    }

    openApplication(id: string) {
        const url = this.router.serializeUrl(this.router.createUrlTree([`/eid-management/applications/${id}`]));
        window.open(url, '_blank');
    }

    getIssuedTextByStatus(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtTerminationApplicationIssued'
                );
            case CertificateStatus.ACTIVE:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtStartApplicationIssued'
                );
            case CertificateStatus.STOPPED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtStopApplicationIssued'
                );
            case CertificateStatus.CREATED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtCreateApplicationIssued'
                );
            case CertificateStatus.EXPIRED:
                return '';
        }
    }

    getApplicationNumberText(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtTerminationApplicationNumber'
                );
            case CertificateStatus.ACTIVE:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtStartApplicationNumber'
                );
            case CertificateStatus.STOPPED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtStopApplicationNumber'
                );
            case CertificateStatus.CREATED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtCreateApplicationNumber'
                );
            case CertificateStatus.EXPIRED:
                return '';
        }
    }

    getApplicationDateText(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtTerminationApplicationDate'
                );
            case CertificateStatus.ACTIVE:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtStartApplicationDate'
                );
            case CertificateStatus.STOPPED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtStopApplicationDate'
                );
            case CertificateStatus.CREATED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtCreateApplicationDate'
                );
            case CertificateStatus.EXPIRED:
                return '';
        }
    }

    getApplicationReasonText(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtTerminationApplicationReason'
                );
            case CertificateStatus.ACTIVE:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtStartApplicationReason'
                );
            case CertificateStatus.STOPPED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtStopApplicationReason'
                );
            case CertificateStatus.CREATED:
                return this.translateService.translate(
                    'modules.eidManagement.certificateHistory.txtCreateApplicationReason'
                );
            case CertificateStatus.EXPIRED:
                return '';
        }
    }

    getStatusClass(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return 'text-danger';
            case CertificateStatus.ACTIVE:
                return 'text-success';
            case CertificateStatus.STOPPED:
                return 'text-warning';
            default:
                return 'text-theme-primary-dark';
        }
    }

    certificateStatusTranslation(status: CertificateStatus) {
        return this.translateService.translate(
            'modules.eidManagement.certificateStatus.txt' + status[0] + status.substring(1).toLocaleLowerCase()
        );
    }

    getReasonTranslation(reasonId: string | null, status: CertificateStatus, reasonText?: string | null) {
        const foundReason = this.originalReasons.find(
            reason => reason.name === ReasonNamesByStatus[status as keyof typeof ReasonNamesByStatus]
        );
        const reasons = foundReason?.nomenclatures.filter(nomenclature => {
            return (
                nomenclature.language === this.translateService.getActiveLang() &&
                nomenclature.permittedUser === PermittedUser.PUBLIC
            );
        });
        if (reasonId && reasons) {
            const foundReason = reasons.find(reason => reason.id === reasonId);

            if (foundReason) {
                return reasonText || foundReason.description;
            } else {
                const nomenclatures = this.originalReasons.flatMap(({ nomenclatures }) => nomenclatures);
                const foundReasonByName = nomenclatures.find(
                    nom =>
                        nom.name === this.originalReasonName && nom.language === this.translateService.getActiveLang()
                );
                return reasonText || foundReasonByName?.description;
            }
        } else {
            return this.translateService.translate('modules.eidManagement.txtNoData');
        }
    }

    getCertificateHistory() {
        this.eidManagement
            .getCertificateHistory(this.certificate.id)
            .pipe(first())
            .subscribe({
                next: (history: any) => {
                    this.certificateHistory = history;
                },
                error: err => {
                    let showDefaultError = true;
                    err.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.toastService.showErrorToast(
                                this.translateService.translate('global.txtErrorTitle'),
                                this.translateService.translate('errors.' + formatError(el))
                            );
                            // if backend has an error/s, turn off the default message
                            showDefaultError = false;
                        }
                    });
                    // if not error was recognized by the storage, throw default message once
                    if (showDefaultError) {
                        this.toastService.showErrorToast(
                            this.translateService.translate('global.txtErrorTitle'),
                            this.translateService.translate('global.txtUnexpectedError')
                        );
                    }
                },
            });
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }
}
