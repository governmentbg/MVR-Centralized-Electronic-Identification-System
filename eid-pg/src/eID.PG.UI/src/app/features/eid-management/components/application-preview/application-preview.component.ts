import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import {
    Application,
    IApplication,
    IDevices,
    IHideCertificatePreviewEventEmitter,
    INomenclature,
    IReason,
} from '../../interfaces/eid-management.interface';
import { TranslocoService } from '@ngneat/transloco';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { ToastService } from 'src/app/shared/services/toast.service';
import { IBreadCrumbItems } from 'src/app/shared/interfaces/IBreadCrumbItems';
import { Subscription, first } from 'rxjs';
import { EidManagementService } from '../../services/eid-management.service';
import { ApplicationStatus, ApplicationTypes, SubmissionTypes } from '../../enums/eid-management';
import { ActivatedRoute, Router } from '@angular/router';
import { errorDataStorage, formatError } from 'src/app/shared/interfaces/ErrorHandlingTools';
import { RaeiceiClientService } from '../../services/raeicei-client.service';
import { NomenclatureService } from '../../services/nomenclature.service';
import { AppConfigService } from 'src/app/core/services/config.service';

@Component({
    selector: 'app-application-preview',
    templateUrl: './application-preview.component.html',
    styleUrls: ['./application-preview.component.scss'],
})
export class ApplicationPreviewComponent implements OnChanges, OnDestroy, OnInit {
    breadcrumbItems: IBreadCrumbItems[] = [];
    subscriptions: Subscription[] = [];
    requestInProgress!: boolean;
    applicationId!: string | null;
    application!: IApplication;
    originalReasonName: string | undefined;
    originalReasons: IReason[] = [];
    reasons: INomenclature[] = [];
    paymentUrl = this.appConfigService.config.externalLinks.paymentByAccessCodeUrl;
    ApplicationStatus = ApplicationStatus;
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private route: ActivatedRoute,
        private eidManagementService: EidManagementService,
        private toastService: ToastService,
        private translocoLocaleService: TranslocoLocaleService,
        private raeiceiService: RaeiceiClientService,
        private nomenclatureService: NomenclatureService,
        private appConfigService: AppConfigService
    ) {
        const languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.applicationId = this.route.snapshot.paramMap.get('id');
            this.updateBreadcrumbItems();
        });
        this.subscriptions.push(languageChangeSubscription);
    }

    ngOnInit(): void {
        // this.applicationId --> used when we were navigated from a specific url /eid-management/applications/:id
        // this.data --> used when we were navigated using preview button at certificates list /eid-management/applications
        if (this.applicationId) {
            const devices = this.raeiceiService.fetchDevices().subscribe((devices: IDevices[]) => {
                this.devicesList = devices;
            });
            this.subscriptions.push(devices);
            this.getApplicationData(this.applicationId);
        } else {
            this.getApplicationData(this.data.id);
        }
    }

    @Output() hide: EventEmitter<IHideCertificatePreviewEventEmitter> =
        new EventEmitter<IHideCertificatePreviewEventEmitter>();
    @Input() devicesList!: IDevices[];
    @Input() data: Application = {
        id: '',
        status: '',
        createDate: new Date(),
        eidentityId: '',
        eidAdministratorName: '',
        deviceId: '',
        applicationType: '',
        applicationNumber: '',
        serialNumber: '',
        reasonId: '',
        submissionType: SubmissionTypes.EID,
        action: ApplicationTypes.ISSUE_EID,
    };

    ngOnChanges(changes: SimpleChanges) {
        this.requestInProgress = false;
        if (changes['data'] || changes['application']) {
            this.updateBreadcrumbItems();
        }
    }

    updateBreadcrumbItems() {
        if ((this.data && this.data.id) || (this.applicationId && this.application)) {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtTitle'),
                    routerLink: '/eid-management',
                },
                {
                    label: this.translateService.translate('modules.eidManagement.txtApplications'),
                    onClick: this.hidePreview.bind(this),
                },
                {
                    label: `${this.translateService.translate('modules.eidManagement.tableFilter.txtApplication')} ${
                        this.data.applicationNumber ? this.data.applicationNumber : this.application?.applicationNumber
                    }`,
                },
            ];
        }
    }

    ngOnDestroy() {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    hidePreview(refreshTable = false): void {
        if (this.applicationId) {
            this.router.navigate(['/eid-management/applications']);
        } else {
            this.hide.emit({ showPreview: false, refreshTable: refreshTable, statusChangeRequest: false });
        }
    }

    computedStatus(status: string) {
        return this.translateService.translate('modules.eidManagement.applicationStatus.txt' + status);
    }

    openCertificate(id: string) {
        const url = this.router.serializeUrl(this.router.createUrlTree([`/eid-management/certificates/${id}`]));
        window.open(url, '_blank');
    }

    translateApplicationType(appType: string | null) {
        switch (appType) {
            case ApplicationTypes.ISSUE_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtIssueEID');
            case ApplicationTypes.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtResumeEID');
            case ApplicationTypes.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtRevokeEID');
            case ApplicationTypes.STOP_EID:
                return this.translateService.translate('modules.eidManagement.applicationTypes.txtStopEID');
            default:
                return appType;
        }
    }

    translateDeviceType(id: string) {
        const device = this.devicesList.find((device: any) => device.id === id);
        const language = this.translocoLocaleService.getLocale();
        if (language === 'bg-BG') {
            return device?.name;
        } else {
            return device?.description;
        }
    }

    translateSubmissionType(type: string | null) {
        switch (type) {
            case SubmissionTypes.EID:
                return this.translateService.translate('modules.eidManagement.submissionTypes.txtEID');
            case SubmissionTypes.BASE_PROFILE:
                return this.translateService.translate('modules.eidManagement.submissionTypes.txtBaseProfile');
            case SubmissionTypes.DESK:
                return this.translateService.translate('modules.eidManagement.submissionTypes.txtDesk');
            default:
                return type;
        }
    }

    formatDate(date: Date | null): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
            timeStyle: 'medium',
        });
    }

    getApplicationData(id: string) {
        this.eidManagementService
            .getApplicationById(id)
            .pipe(first())
            .subscribe({
                next: (application: any) => {
                    this.application = application;
                    this.updateBreadcrumbItems();
                    this.nomenclatureService.reasons().then(response => {
                        this.originalReasons = response;

                        const nomenclatures = this.originalReasons.flatMap(({ nomenclatures }) => nomenclatures);
                        this.originalReasonName = nomenclatures.find(nomenclature => {
                            return nomenclature.id === this.application.reasonId;
                        })?.name;
                    });
                },
                error: err => {
                    let showDefaultError = true;
                    err.errors?.forEach((el: string) => {
                        if (errorDataStorage.has(el)) {
                            this.toastService.showErrorToast(
                                this.translateService.translate('global.txtErrorTitle'),
                                this.translateService.translate('errors.' + formatError(el))
                            );
                            showDefaultError = false;
                        }
                    });
                    if (showDefaultError) {
                        this.toastService.showErrorToast(
                            this.translateService.translate('global.txtErrorTitle'),
                            this.translateService.translate('global.txtUnexpectedError')
                        );
                    }
                },
            });
    }

    redirectToPayment(accessCode: string): void {
        window.open(`${this.paymentUrl}${accessCode}`, '_blank');
    }

    getReasonTranslation(reasonId: string | null) {
        if (reasonId) {
            const foundReason = this.reasons.find(reason => reason.id === reasonId);

            if (foundReason) {
                return foundReason.description;
            } else {
                const nomenclatures = this.originalReasons.flatMap(({ nomenclatures }) => nomenclatures);
                const foundReasonByName = nomenclatures.find(
                    nom =>
                        nom.name === this.originalReasonName && nom.language === this.translateService.getActiveLang()
                );
                return foundReasonByName?.description;
            }
        } else {
            return this.translateService.translate('modules.eidManagement.txtNoData');
        }
    }

    showPaymentButton(application: IApplication) {
        return (
            application.status === ApplicationStatus.PENDING_PAYMENT &&
            application.submissionType !== SubmissionTypes.DESK
        );
    }
}
