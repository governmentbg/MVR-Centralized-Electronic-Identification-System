import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject, retry, Subject, Subscription } from 'rxjs';
import { IBreadCrumbItems } from '../../interfaces/breadcrumb.interfaces';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import {
    ApplicationStatus,
    ApplicationType,
    CertificateStatus,
    PersonalIdTypes,
    SubmissionType,
} from '../../enums/eid-management.enum';
import { PivrClientService } from '../../services/pivr-client.service';
import { MpozeiClientService } from '../../services/mpozei-client.service';
import { LazyLoadEvent } from 'primeng/api';
import { ToastService } from '../../../../shared/services/toast.service';
import { Table } from 'primeng/table';
import {
    CHILD_AGE,
    IApplication,
    ICertificate,
    ICheckUidRestrictionsResponseData,
    IFindEidentityByNumberAndTypeResponseData,
    IForeignIdentity,
    IGetApplicationsByEidentityRequestData,
    IGetCertificatesByEidentityRequestData,
    IHidePreviewEventEmitter,
    IPersonalIdentity,
    IPersonRelation,
    LAWFUL_AGE,
} from '../../interfaces/eid-management.interfaces';
import * as moment from 'moment';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { EidDeviceService } from '../../services/eid-device.service';
import { ConfirmationDialogService } from '../../../../shared/services/confirmation-dialog.service';
import { EidAdministratorService } from '../../services/eid-administrator.service';
import { RoleType } from '../../../../core/enums/auth.enum';

@Component({
    selector: 'app-search-results',
    templateUrl: './search-results.component.html',
    styleUrls: ['./search-results.component.scss'],
})
export class SearchResultsComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private pivrClientService: PivrClientService,
        private mpozeiClientService: MpozeiClientService,
        private toastService: ToastService,
        private translocoLocaleService: TranslocoLocaleService,
        private eidDeviceService: EidDeviceService,
        private confirmationDialogService: ConfirmationDialogService,
        private eidAdministratorService: EidAdministratorService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.eidManagement.txtSearchSubtitle'),
                    onClick: this.navigateToSearch.bind(this),
                },
                { label: this.translateService.translate('modules.eidManagement.txtSearchResults') },
            ];
            this.personalIdTypes = [
                {
                    name: this.translateService.translate('modules.eidManagement.txtEGN'),
                    id: PersonalIdTypes.EGN,
                },
                {
                    name: this.translateService.translate('modules.eidManagement.txtLNCH'),
                    id: PersonalIdTypes.LNCH,
                },
            ];
            this.submissionTypes = [
                {
                    id: SubmissionType.DESK,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.DESK}`
                    ),
                },
                {
                    id: SubmissionType.EID,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.EID}`
                    ),
                },
                {
                    id: SubmissionType.BASE_PROFILE,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.BASE_PROFILE}`
                    ),
                },
                {
                    id: SubmissionType.PERSO_CENTRE,
                    name: this.translateService.translate(
                        `modules.eidManagement.submissionTypes.${SubmissionType.PERSO_CENTRE}`
                    ),
                },
            ];
            this.buildTabOptions();
        });

        this.errorMessages = [];
        this.personalDocumentNumber = sessionStorage.getItem('personalDocumentNumber') || null;
        const navigation = this.router.getCurrentNavigation();
        if (navigation || window.history.state) {
            const state = navigation ? (navigation.extras.state as any) : window.history.state;
            if (state) {
                this.personalId = state.personalId;
                this.personalIdType = state.personalIdType;
                this.personalDocumentNumber = state.personalDocumentNumber;
                sessionStorage.setItem('personalDocumentNumber', state.personalDocumentNumber);
                if (state.personalDocumentNumber === null || state.personalIdType === PersonalIdTypes.LNCH) {
                    sessionStorage.removeItem('personalDocumentNumber');
                }
            }
        }
    }

    ApplicationStatus = ApplicationStatus;
    PersonalIdTypes = PersonalIdTypes;
    languageChangeSubscription: Subscription = new Subscription();
    breadcrumbItems: IBreadCrumbItems[] = [];
    personalId;
    personalIdType!: PersonalIdTypes;
    personalIdTypes = [
        {
            name: this.translateService.translate('modules.eidManagement.txtEGN'),
            id: PersonalIdTypes.EGN,
        },
        {
            name: this.translateService.translate('modules.eidManagement.txtLNCH'),
            id: PersonalIdTypes.LNCH,
        },
    ];
    personalDocumentNumber: string | null = null;
    errorMessages: string[] = [];
    isNewApplicationButtonVisible = false;
    applicationsTotalRecords = 0;
    applicationsPageSize = 10;
    certificatesTotalRecords = 0;
    certificatesPageSize = 10;
    loadingApplications!: boolean;
    loadingCertificates!: boolean;
    lazyLoadApplicationsEventState: LazyLoadEvent = {};
    lazyLoadCertificatesEventState: LazyLoadEvent = {};
    applicationsFilterState: any = {};
    applications: IApplication[] = [];
    certificatesFilterState: any = {};
    certificates: ICertificate[] = [];
    value = 'applications';
    activeItem!: string;
    tabOptions: any[] = [];
    noEidForThisPerson = true;
    loadingEidStatus!: boolean;
    loadingProhibitionDateOfDeathStatus!: boolean;
    currentEid: IFindEidentityByNumberAndTypeResponseData | null = null;
    personalIdentity: IPersonalIdentity | IForeignIdentity | null = null;
    INTERVAL = 10000;
    retryCountIfServerError = 3;
    loadRegixDataSubscription: Subscription = new Subscription();
    loadingRegixData!: boolean;
    showApplicationDetails = false;
    selectedApplication!: IApplication;
    showCertificateDetailsModal = false;
    selectedCertificate!: string;
    dialogVisible = false;
    relatives: IPersonRelation[] = [];
    personIsBelowLawfulAge = false;
    action: ApplicationType | null = null;
    CertificateStatus = CertificateStatus;
    relativesSearchLoading = false;
    restrictionsStatus: ICheckUidRestrictionsResponseData = {
        response: { isProhibited: false, isDead: false, hasRevokedParentalRights: false },
        hasFailed: false,
        error: '',
    };
    ApplicationType = ApplicationType;
    prohibitionOrDateOfDeathSubject = new Subject<any>();
    actionApplicationId: string | null = null;
    noSuchPersonError = false;
    isPINChangeDialogVisible = false;
    personIsAMinor = false;
    protected readonly LAWFUL_AGE = LAWFUL_AGE;
    protected readonly CHILD_AGE = CHILD_AGE;
    noEidErrorMessage = this.translateService.translate('modules.eidManagement.txtPersonDoesntHaveExistingEid');
    submissionTypes: { id: SubmissionType; name: string }[] = [];
    buttonLoading: BehaviorSubject<boolean>[] = this.relatives.map(() => new BehaviorSubject<boolean>(false));

    @ViewChild('dt') table!: Table;

    ngOnInit() {
        this.loadingApplications = true;
        this.loadingCertificates = true;
        this.loadingEidStatus = true;
        this.loadingProhibitionDateOfDeathStatus = true;
        this.loadingRegixData = true;
        this.breadcrumbItems = [
            {
                label: this.translateService.translate('modules.eidManagement.txtSearchSubtitle'),
                onClick: this.navigateToSearch.bind(this),
            },
            { label: this.translateService.translate('modules.eidManagement.txtSearchResults') },
        ];
        this.buildTabOptions();
        this.activeItem = this.tabOptions[0].value;
        this.checkForProhibitionOrDateOfDeath();
        this.checkForEid();
        this.prohibitionOrDateOfDeathSubject.subscribe(response => {
            this.getPersonalIdentity();
        });
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    navigateToSearch() {
        this.router.navigate(['/eid-management']);
    }

    buildTabOptions() {
        this.tabOptions = [
            {
                label: this.translateService.translate('modules.eidManagement.txtApplicationsForEid'),
                value: 'applications',
            },
            {
                label: this.translateService.translate('modules.eidManagement.txtCertificatesForEid'),
                value: 'certificates',
            },
        ];
    }

    activeItemChange(event: string) {
        this.activeItem = event;
    }

    createNewApplication() {
        this.router.navigate(['/eid-management/search/new-application'], {
            state: {
                personalId: this.personalId,
                personalIdType: this.personalIdType,
                currentEid: this.currentEid,
                restrictionsStatus: this.restrictionsStatus,
                personalDocumentNumber: this.personalDocumentNumber,
            },
        });
    }

    checkForProhibitionOrDateOfDeath() {
        this.pivrClientService
            .checkUidRestrictions({
                uid: this.personalId,
                uidType: this.personalIdType,
            })
            .subscribe({
                next: response => {
                    this.loadingProhibitionDateOfDeathStatus = false;
                    if (response.response.isDead) {
                        this.errorMessages.push(
                            this.translateService.translate('modules.eidManagement.txtSearchErrorDateOfDeath')
                        );
                        this.isNewApplicationButtonVisible = false;
                    } else {
                        this.isNewApplicationButtonVisible = true;
                    }

                    if (response.response.isProhibited) {
                        this.errorMessages.push(
                            this.translateService.translate('modules.eidManagement.txtSearchErrorDateOfProhibition')
                        );
                    }
                    this.restrictionsStatus = response;
                    this.prohibitionOrDateOfDeathSubject.next(response);
                },
                error: error => {
                    this.loadingProhibitionDateOfDeathStatus = false;
                    this.isNewApplicationButtonVisible = true;
                    this.showErrorToast(this.translateService.translate('global.txtNoConnectionProhibition'));
                    this.prohibitionOrDateOfDeathSubject.next(error);
                },
            });
    }

    checkForEid() {
        this.mpozeiClientService
            .findEidentityByNumberAndType({
                type: this.personalIdType,
                number: this.personalId,
            })
            .subscribe({
                next: response => {
                    this.noEidForThisPerson = !response.eidentityId;
                    this.currentEid = response;
                    this.loadingEidStatus = false;
                },
                error: (error: any) => {
                    switch (error.status) {
                        case 404:
                            this.noEidErrorMessage = this.translateService.translate(
                                'modules.eidManagement.txtPersonDoesntHaveExistingEid'
                            );
                            this.noEidForThisPerson = true;
                            break;
                        case 400:
                            this.noEidErrorMessage = this.translateService.translate('global.txtNoConnectionEid');
                            this.noEidForThisPerson = true;
                            this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                            break;
                        default:
                            this.noEidErrorMessage = this.translateService.translate('global.txtNoConnectionEid');
                            this.noEidForThisPerson = true;
                            this.showErrorToast(this.translateService.translate('global.txtNoConnectionEid'));
                            break;
                    }
                    this.loadingEidStatus = false;
                },
            });
    }

    get translatedPersonalIdType() {
        return this.personalIdTypes.find(type => {
            return type.id === this.personalIdType;
        });
    }

    formatDate(date: string | null | undefined, includeHourAndMinutes = false): string {
        if (!date) {
            return '';
        }

        if (includeHourAndMinutes) {
            return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
                timeStyle: 'medium',
            });
        } else {
            return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale());
        }
    }

    loadApplications(event: LazyLoadEvent, applicationsFilterData?: any): void {
        this.loadingApplications = true;
        this.lazyLoadApplicationsEventState = event;
        this.applicationsTotalRecords = 0;

        const sortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
        let payload: IGetApplicationsByEidentityRequestData = {
            page: 0,
            eidentityId: '',
            size: event.rows || this.applicationsPageSize,
            sort: `${event.sortField},${sortOrder}`,
        };

        if (applicationsFilterData !== null && applicationsFilterData !== undefined) {
            payload = { ...payload, ...applicationsFilterData };
        } else if (this.applicationsFilterState !== null && Object.keys(this.applicationsFilterState).length > 0) {
            payload = { ...payload, ...this.applicationsFilterState };
        }

        if (event && event.first && event.rows) {
            payload.page = event.first / event.rows;
        }

        payload.eidentityId = this.currentEid?.eidentityId as string;

        this.mpozeiClientService.getApplicationsByEidentity(payload).subscribe({
            next: response => {
                this.applications = response.content;
                this.applicationsTotalRecords = response.totalElements;
                this.loadingApplications = false;
            },
            error: (error: any) => {
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtNoConnectionApplication'));
                        break;
                }
                this.loadingApplications = false;
            },
        });
    }

    loadCertificates(event: LazyLoadEvent, certificatesFilterData?: any): void {
        this.loadingCertificates = true;
        this.lazyLoadCertificatesEventState = event;
        this.certificatesTotalRecords = 0;

        const sortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
        let payload: IGetCertificatesByEidentityRequestData = {
            page: 0,
            eidentityId: '',
            size: event.rows || this.certificatesPageSize,
            sort: `${event.sortField},${sortOrder}`,
        };

        if (certificatesFilterData !== null && certificatesFilterData !== undefined) {
            payload = { ...payload, ...certificatesFilterData };
        } else if (this.certificatesFilterState !== null && Object.keys(this.certificatesFilterState).length > 0) {
            payload = { ...payload, ...this.certificatesFilterState };
        }

        if (event && event.first && event.rows) {
            payload.page = event.first / event.rows;
        }

        payload.eidentityId = this.currentEid?.eidentityId as string;

        this.mpozeiClientService.getCertificatesByEidentity(payload).subscribe({
            next: response => {
                this.certificates = response.content;
                this.certificatesTotalRecords = response.totalElements;
                this.loadingCertificates = false;
            },
            error: (error: any) => {
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtNoConnectionCertificate'));
                        break;
                }
                this.loadingCertificates = false;
            },
        });
    }

    showDetails(application: any) {
        this.selectedApplication = application;
        this.showApplicationDetails = true;
    }

    showCertificateDetails(certificate: string, action?: ApplicationType) {
        this.selectedCertificate = certificate;
        this.showCertificateDetailsModal = true;
        this.action = action || null;
    }

    isDateBelowAge(date: any, age: number) {
        return moment().diff(moment(new Date(date)), 'years') < age;
    }

    getPersonalIdentity() {
        if (this.personalIdType === PersonalIdTypes.EGN) {
            this.loadRegixDataSubscription = this.pivrClientService
                .getPersonalIdentityV2({
                    EGN: this.personalId,
                    IdentityDocumentNumber: this.personalDocumentNumber as string,
                })
                .pipe(
                    retry({
                        count: this.retryCountIfServerError,
                        delay: this.INTERVAL,
                    })
                )
                .subscribe({
                    next: response => {
                        this.loadingRegixData = false;
                        this.personalIdentity = response.response.PersonalIdentityInfoResponse;
                        this.personIsBelowLawfulAge = this.isDateBelowAge(this.personalIdentity.birthDate, LAWFUL_AGE);
                        this.personIsAMinor = this.isDateBelowAge(this.personalIdentity.birthDate, CHILD_AGE);

                        if (response.response.PersonalIdentityInfoResponse.returnInformations.returnCode === '0100') {
                            this.isNewApplicationButtonVisible = false;
                            this.noSuchPersonError = true;
                            this.errorMessages.push(
                                this.translateService.translate('modules.eidManagement.txtSearchErrorNoSuchPerson')
                            );
                        } else {
                            this.noSuchPersonError = false;
                            if (this.isPersonalDocumentExpired) {
                                this.errorMessages.push(
                                    this.translateService.translate(
                                        'modules.eidManagement.txtIdentityDocumentExpiredError'
                                    )
                                );
                            }
                        }
                    },
                    error: (error: any) => {
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            default:
                                this.showErrorToast(this.translateService.translate('global.txtNoConnectionWithRegix'));
                                break;
                        }

                        this.confirmationDialogService.showConfirmation({
                            message: this.translateService.translate(
                                'modules.eidManagement.txtFailedLoadingPersonalData'
                            ),
                            header: this.translateService.translate('global.txtConfirmation'),
                            icon: 'pi pi-exclamation-triangle',
                            rejectVisible: false,
                            acceptLabel: this.translateService.translate('global.txtOk'),
                            accept: () => {
                                this.loadingRegixData = false;
                            },
                            reject: () => {
                                this.loadingRegixData = false;
                            },
                        });
                    },
                });
        } else {
            this.loadRegixDataSubscription = this.pivrClientService
                .getForeignIdentityV2({ Identifier: this.personalId, IdentifierType: 'LNCh' })
                .pipe(
                    retry({
                        count: this.retryCountIfServerError,
                        delay: this.INTERVAL,
                    })
                )
                .subscribe({
                    next: response => {
                        this.loadingRegixData = false;
                        this.personalIdentity = response.response.ForeignIdentityInfoResponse;
                        this.personIsBelowLawfulAge = this.isDateBelowAge(this.personalIdentity.birthDate, LAWFUL_AGE);
                        this.personIsAMinor = this.isDateBelowAge(this.personalIdentity.birthDate, CHILD_AGE);
                        if (
                            this.personalIdentity.validDate &&
                            moment(this.personalIdentity.validDate).isBefore(moment())
                        ) {
                            this.errorMessages.push(
                                this.translateService.translate('modules.eidManagement.txtIdentityDocumentExpiredError')
                            );
                        }
                    },
                    error: (error: any) => {
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            default:
                                this.showErrorToast(this.translateService.translate('global.txtNoConnectionWithRegix'));
                                break;
                        }

                        this.confirmationDialogService.showConfirmation({
                            message: this.translateService.translate(
                                'modules.eidManagement.txtFailedLoadingPersonalData'
                            ),
                            header: this.translateService.translate('global.txtConfirmation'),
                            icon: 'pi pi-exclamation-triangle',
                            rejectVisible: false,
                            acceptLabel: this.translateService.translate('global.txtOk'),
                            accept: () => {
                                this.loadingRegixData = false;
                            },
                            reject: () => {
                                this.loadingRegixData = false;
                            },
                        });
                    },
                });
        }
    }

    onFilteredApplicationsDataChange(filteredData: any) {
        this.applicationsFilterState = filteredData;
        // On filter change we always set pagination to first page
        if (this.table) {
            if (this.table.first > 0) {
                this.table.first = 0;
                this.lazyLoadApplicationsEventState.first = 0;
            }
            this.loadApplications(this.lazyLoadApplicationsEventState, filteredData);
        }
    }

    onFilteredCertificatesDataChange(filteredData: any) {
        for (const key in filteredData) {
            if (Array.isArray(filteredData[key])) {
                filteredData[key] = filteredData[key].join(',');
            }
        }
        this.certificatesFilterState = filteredData;
        // On filter change we always set pagination to first page
        if (this.table.first > 0) {
            this.table.first = 0;
            this.lazyLoadCertificatesEventState.first = 0;
        }
        this.loadCertificates(this.lazyLoadCertificatesEventState, filteredData);
    }

    applicationDeviceTypeTranslation(deviceId: string) {
        return this.eidDeviceService.getDeviceTranslation(deviceId);
    }

    applicationStatusTranslation(status: ApplicationStatus) {
        switch (status) {
            case ApplicationStatus.COMPLETED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Completed');
            case ApplicationStatus.SUBMITTED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Submitted');
            case ApplicationStatus.SIGNED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Signed');
            case ApplicationStatus.APPROVED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Approved');
            case ApplicationStatus.DENIED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Denied');
            case ApplicationStatus.GENERATED_CERTIFICATE:
                return this.translateService.translate(
                    'modules.eidManagement.tableFilter.statuses.GeneratedCertificate'
                );
            case ApplicationStatus.PAID:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Paid');
            case ApplicationStatus.PENDING_PAYMENT:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.PendingPayment');
            case ApplicationStatus.PENDING_SIGNATURE:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.PendingSignature');
            case ApplicationStatus.PROCESSING:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.Processing');
            case ApplicationStatus.CERTIFICATE_STORED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.CertificateStored');
            case ApplicationStatus.PAYMENT_EXPIRED:
                return this.translateService.translate('modules.eidManagement.tableFilter.statuses.PaymentExpired');
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

    get nationality() {
        return this.personalIdentity && this.personalIdentity.nationalityList
            ? this.personalIdentity.nationalityList[0].nationalityName
            : this.translateService.translate('modules.eidManagement.txtNoData');
    }

    get permanentAddress() {
        if (this.personalIdentity && this.personalIdentity.permanentAddress) {
            const locationName = this.personalIdentity.permanentAddress.locationName;
            const buildingNumber = this.personalIdentity.permanentAddress.buildingNumber;
            const entrance = this.personalIdentity.permanentAddress.entrance;
            const floor = this.personalIdentity.permanentAddress.floor;
            const apartment = this.personalIdentity.permanentAddress.apartment;
            const settlementCode = this.personalIdentity.permanentAddress.settlementCode;
            const districtName = this.personalIdentity.permanentAddress.districtName;
            const settlementName = this.personalIdentity.permanentAddress.settlementName;
            const municipalityName = this.personalIdentity.permanentAddress.municipalityName;

            const addressLines = [];

            if (locationName && buildingNumber) {
                addressLines.push(locationName + ' №' + buildingNumber + ', ');
            } else if (locationName) {
                addressLines.push(locationName + ', ');
            }

            const additionalInfo = [];
            if (entrance) {
                additionalInfo.push(
                    `${this.translateService.translate('modules.eidManagement.txtBuilding')} ` + entrance
                );
            }
            if (floor) {
                additionalInfo.push(`${this.translateService.translate('modules.eidManagement.txtFloor')} ` + floor);
            }

            if (apartment) {
                additionalInfo.push(
                    `${this.translateService.translate('modules.eidManagement.txtApartment')} ` + apartment
                );
            }

            if (additionalInfo.length > 0) {
                addressLines.push(additionalInfo.join(', ') + ', ');
            }

            if (settlementCode && settlementName) {
                addressLines.push(settlementCode + ' ' + settlementName + ', ');
            } else if (settlementName) {
                addressLines.push(settlementName + ', ');
            }

            const locationInfo = [];
            if (districtName) {
                locationInfo.push(districtName);
            }
            if (municipalityName) {
                locationInfo.push(municipalityName);
            }
            if (locationInfo.length > 0) {
                addressLines.push(locationInfo.join(', '));
            }

            return addressLines.join('');
        } else {
            return this.translateService.translate('modules.eidManagement.txtNoData');
        }
    }

    onHideApplicationDetails(eventData: IHidePreviewEventEmitter) {
        if (eventData.refreshTable) {
            this.loadApplications(this.lazyLoadApplicationsEventState, this.applicationsFilterState);
            this.loadCertificates(this.lazyLoadCertificatesEventState, this.certificatesFilterState);
        }
        if (eventData.requestedAction) {
            this.actionApplicationId = eventData.requestedAction.context.applicationId;
            this.showCertificateDetails(
                eventData.requestedAction.context.certificateId,
                eventData.requestedAction.name
            );
        } else {
            this.actionApplicationId = null;
        }
        this.showApplicationDetails = eventData.showPreview;
    }

    onHideCertificateDetails(eventData: IHidePreviewEventEmitter) {
        if (eventData.refreshTable) {
            this.loadCertificates(this.lazyLoadCertificatesEventState, this.certificatesFilterState);
            this.loadApplications(this.lazyLoadApplicationsEventState, this.applicationsFilterState);
        }
        this.actionApplicationId = null;
        this.showCertificateDetailsModal = eventData.showPreview;
    }

    showRelationsSearch() {
        this.dialogVisible = true;
        this.relativesSearchLoading = true;
        this.relatives = [];
        this.pivrClientService.getPersonRelatives(this.personalId).subscribe({
            next: response => {
                this.relatives = response.response.relationsResponse.personRelations || [];
                this.buttonLoading = this.relatives.map(() => new BehaviorSubject<boolean>(false));
                this.relativesSearchLoading = false;
            },
            error: (error: any) => {
                this.relativesSearchLoading = false;
                switch (error.status) {
                    case 400:
                        this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                        break;
                    default:
                        this.showErrorToast(this.translateService.translate('global.txtNoConnectionRelation'));
                        break;
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

    getIssuerName() {
        if (this.personalIdentity) {
            if ('issuerPlace' in this.personalIdentity) {
                return `${this.personalIdentity.issuerName}-${this.personalIdentity.issuerPlace}`;
            } else if ('issuePlace' in this.personalIdentity) {
                return `${this.personalIdentity.issuerName}-${this.personalIdentity.issuePlace}`;
            } else {
                return `${this.personalIdentity.issuerName}`;
            }
        }
        return this.translateService.translate('modules.eidManagement.txtNoData');
    }

    setPIN() {
        this.confirmationDialogService.showConfirmation({
            message: this.translateService.translate('modules.eidManagement.txtSetPINConfirmationMessage'),
            header: this.translateService.translate('global.txtConfirmation'),
            icon: 'pi pi-exclamation-triangle',
            rejectVisible: false,
            acceptLabel: this.translateService.translate('global.txtOk'),
            accept: () => {
                this.isPINChangeDialogVisible = true;
            },
        });
    }

    onHidePINDialog() {
        this.isPINChangeDialogVisible = false;
    }

    isCertificateExpired(certificate: ICertificate) {
        const date = moment.utc(certificate.validityUntil);
        const now = moment.utc();

        return now > date;
    }

    applicationTypeText(type: ApplicationType) {
        switch (type) {
            case ApplicationType.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.txtApplicationTypeRevoke');
            case ApplicationType.STOP_EID:
                return this.translateService.translate('modules.eidManagement.txtApplicationTypeStop');
            case ApplicationType.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.txtApplicationTypeResume');
            case ApplicationType.ISSUE_EID:
                return this.translateService.translate('modules.eidManagement.txtApplicationTypeIssue');
        }
    }

    getOfficeName(officeId: string) {
        return this.eidAdministratorService.getOfficeNameById(officeId);
    }

    submissionTypeTranslation(submissionType: SubmissionType) {
        return this.submissionTypes.find(type => type.id === submissionType)?.name || '';
    }

    checkForRelativeRestrictions(relative: any, index: number) {
        this.buttonLoading[index].next(true);
        this.pivrClientService
            .checkUidRestrictions({
                uid: relative.egn,
                uidType: PersonalIdTypes.EGN,
            })
            .subscribe({
                next: response => {
                    relative.restrictions = response.response;
                    this.buttonLoading[index].next(false);
                },
                error: error => {
                    this.showErrorToast(this.translateService.translate('global.txtNoConnectionProhibition'));
                    this.buttonLoading[index].next(false);
                },
            });
    }

    get isPersonalDocumentExpired() {
        if (
            this.personalIdentity &&
            this.personalIdentity.validDate &&
            moment(this.personalIdentity.validDate).isBefore(moment())
        ) {
            return true;
        } else {
            return false;
        }
    }

    protected readonly RoleType = RoleType;
}
