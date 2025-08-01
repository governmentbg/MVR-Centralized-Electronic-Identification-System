import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import { MpozeiClientService } from '../../services/mpozei-client.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { Subscription } from 'rxjs';
import { IBreadCrumbItems } from '../../interfaces/breadcrumb.interfaces';
import {
    ICertificate,
    IHidePreviewEventEmitter,
    IHistoryItem,
    INomenclature,
    IReason,
} from '../../interfaces/eid-management.interfaces';
import { CertificateStatus } from '../../enums/eid-management.enum';
import { NomenclatureService } from '../../services/nomenclature.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';

@Component({
    selector: 'app-certificate-history',
    templateUrl: './certificate-history.component.html',
    styleUrls: ['./certificate-history.component.scss'],
})
export class CertificateHistoryComponent implements OnInit, OnDestroy, OnChanges {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private mpozeiClientService: MpozeiClientService,
        private toastService: ToastService,
        private nomenclatureService: NomenclatureService,
        private translocoLocaleService: TranslocoLocaleService
    ) {}

    @Output() hide: EventEmitter<IHidePreviewEventEmitter> = new EventEmitter<IHidePreviewEventEmitter>();
    @Input() certificate!: ICertificate;
    @Input() fullBreadcrumb = true;

    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    loading = false;
    certificateHistory: IHistoryItem[] | null = null;
    originalReasons: IReason[] = [];
    reasons: INomenclature[] = [];
    originalReasonName: string | undefined;
    CertificateStatus = CertificateStatus;

    ngOnInit() {
        this.loading = true;
        this.languageChangeSubscription = this.translateService.langChanges$.subscribe(() => {
            let breadcrumbs: IBreadCrumbItems[] = [];

            if (this.router.url === '/eid-management/search/results') {
                breadcrumbs = [
                    {
                        label: this.translateService.translate('modules.eidManagement.txtSearchSubtitle'),
                        onClick: this.navigateToSearch.bind(this),
                    },
                    {
                        label: this.translateService.translate('modules.eidManagement.txtSearchResults'),
                        onClick: this.navigateToSearchResults.bind(this),
                    },
                ];
            }
            this.breadcrumbItems = [
                ...breadcrumbs,
                {
                    label: this.translateService.translate('modules.eidManagement.details.txtPreviewCertificate'),
                    onClick: () => this.closePreview(),
                },
                {
                    label: this.translateService.translate(
                        'modules.eidManagement.details.txtCertificateHistoryBreadcrumb'
                    ),
                },
            ];
        });
        this.nomenclatureService.getReasons().subscribe({
            next: response => {
                this.originalReasons = response;
                const nomenclatures = response.flatMap(({ nomenclatures }) => nomenclatures);
                this.reasons = nomenclatures.filter(nomenclature => {
                    return nomenclature.language === this.translateService.getActiveLang();
                });
            },
        });
    }

    ngOnChanges() {
        if (this.certificate && this.certificate.id) {
            this.loading = true;
            this.mpozeiClientService.getCertificateHistory(this.certificate.id).subscribe({
                next: response => {
                    this.certificateHistory = response;
                    this.loading = false;
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
        const url = this.router.serializeUrl(this.router.createUrlTree([`/eid-management/application/${id}`]));
        window.open(url, '_blank');
    }

    getIssuedTextByStatus(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtTerminationApplicationIssued'
                );
            case CertificateStatus.ACTIVE:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtStartApplicationIssued'
                );
            case CertificateStatus.STOPPED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtStopApplicationIssued'
                );
            case CertificateStatus.CREATED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtCreateApplicationIssued'
                );
            case CertificateStatus.EXPIRED:
            case CertificateStatus.FAILED:
                return '';
        }
    }

    getApplicationNumberText(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtTerminationApplicationNumber'
                );
            case CertificateStatus.ACTIVE:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtStartApplicationNumber'
                );
            case CertificateStatus.STOPPED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtStopApplicationNumber'
                );
            case CertificateStatus.CREATED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtCreateApplicationNumber'
                );
            case CertificateStatus.EXPIRED:
            case CertificateStatus.FAILED:
                return '';
        }
    }

    getApplicationDateText(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtTerminationApplicationDate'
                );
            case CertificateStatus.ACTIVE:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtStartApplicationDate'
                );
            case CertificateStatus.STOPPED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtStopApplicationDate'
                );
            case CertificateStatus.CREATED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtCreateApplicationDate'
                );
            case CertificateStatus.EXPIRED:
            case CertificateStatus.FAILED:
                return '';
        }
    }

    getApplicationReasonText(status: CertificateStatus) {
        switch (status) {
            case CertificateStatus.REVOKED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtTerminationApplicationReason'
                );
            case CertificateStatus.ACTIVE:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtStartApplicationReason'
                );
            case CertificateStatus.STOPPED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtStopApplicationReason'
                );
            case CertificateStatus.CREATED:
                return this.translateService.translate(
                    'modules.eidManagement.details.certificateHistory.txtCreateApplicationReason'
                );
            case CertificateStatus.EXPIRED:
            case CertificateStatus.FAILED:
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
        switch (status) {
            case CertificateStatus.ACTIVE:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Active');
            case CertificateStatus.STOPPED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Stopped');
            case CertificateStatus.REVOKED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Revoked');
            case CertificateStatus.CREATED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Created');
            case CertificateStatus.EXPIRED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Expired');
            case CertificateStatus.FAILED:
                return this.translateService.translate('modules.eidManagement.tableFilter.certificateStatuses.Failed');
        }
    }

    getReasonTranslation(reasonId: string | null, reasonText: string | null) {
        if (reasonId) {
            const foundReason = this.reasons.find(reason => reason.id === reasonId);
            const nomenclatures = this.originalReasons.flatMap(({ nomenclatures }) => nomenclatures);
            this.originalReasonName = nomenclatures.find(nomenclature => {
                return nomenclature.id === reasonId;
            })?.name;

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

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }
}
