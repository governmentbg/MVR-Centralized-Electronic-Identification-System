import {
    Component,
    EventEmitter,
    HostListener,
    Input,
    OnChanges,
    OnDestroy,
    OnInit,
    Output,
    ViewChild,
} from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { MpozeiClientService } from '../../services/mpozei-client.service';
import { ToastService } from '../../../../shared/services/toast.service';
import {
    ICertificate,
    ICheckUidRestrictionsResponseData,
    IFindEidentityByNumberAndTypeResponseData,
    IForeignIdentity,
    INomenclature,
    IPersonalIdentity,
    IReason,
    ISignApplicationRequestData,
    IStep,
    LAWFUL_AGE,
} from '../../interfaces/eid-management.interfaces';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import {
    SubmissionType,
    ApplicationType,
    NomenclatureNameWithAdditionalReasonField,
    PermittedUser,
    PersonalIdTypes,
    ReasonNamesByAction,
} from '../../enums/eid-management.enum';
import { concatMap, of, Subscription } from 'rxjs';
import { NomenclatureService } from '../../services/nomenclature.service';
import { GuardiansFormComponent } from '../guardians-form/guardians-form.component';
import * as moment from 'moment/moment';
import { PivrClientService } from '../../services/pivr-client.service';
import { TranslocoLocaleService } from '@ngneat/transloco-locale';
import { RoleType } from '../../../../core/enums/auth.enum';
import { UserService } from '../../../../core/services/user.service';

@Component({
    selector: 'app-certificate-action-form',
    templateUrl: './certificate-action-form.component.html',
    styleUrls: ['./certificate-action-form.component.scss'],
})
export class CertificateActionFormComponent implements OnInit, OnChanges, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private mpozeiClientService: MpozeiClientService,
        private toastService: ToastService,
        private nomenclatureService: NomenclatureService,
        private pivrClientService: PivrClientService,
        private translocoLocaleService: TranslocoLocaleService,
        private userService: UserService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.steps = [
                {
                    id: 1,
                    name: this.translateService.translate('modules.eidManagement.txtPersonalDataStep'),
                    ref: 'personalData',
                },
            ];
            if (this.userService.hasRole(RoleType.AEID)) {
                this.steps.push({
                    id: 2,
                    name: this.translateService.translate('modules.eidManagement.details.txtConfirm'),
                    ref: 'confirmAndSign',
                });
            } else {
                this.steps.push({
                    id: 2,
                    name: this.translateService.translate('modules.eidManagement.details.txtConfirmAndSign'),
                    ref: 'confirmAndSign',
                });
            }
        });
    }

    @ViewChild('guardiansForm') guardiansForm!: GuardiansFormComponent;
    @Output() actionExecuted: EventEmitter<any> = new EventEmitter<any>();
    @Input() action!: ApplicationType;
    @Input() personalId!: string;
    @Input() personalIdType!: PersonalIdTypes;
    @Input() personalIdentity!: IPersonalIdentity | IForeignIdentity;
    @Input() eIdentity!: IFindEidentityByNumberAndTypeResponseData;
    @Input() certificate!: ICertificate;
    @Input() restrictionsStatus!: ICheckUidRestrictionsResponseData;
    @Input() applicationId: string | null = null;
    @HostListener('window:beforeunload', ['$event'])
    beforeUnloadHandler() {
        // returning true will navigate without confirmation
        // returning false will show a confirm dialog before navigating away
        return !this.unsavedFormDataExists;
    }
    unsavedFormDataExists = true;
    languageChangeSubscription: Subscription = new Subscription();
    currentStep = 0;
    form: FormGroup<{ reason: FormControl<INomenclature | null>; customReason: FormControl<string | null> }> =
        new FormGroup({
            reason: new FormControl<INomenclature | null>(null, []),
            customReason: new FormControl<string | null>(null, []),
        });
    steps: IStep[] = [];
    originalReasons: IReason[] = [];
    reasons: INomenclature[] = [];
    customReasonNames = Object.keys(NomenclatureNameWithAdditionalReasonField);
    requestInProgress = false;
    createdApplicationId: string | null = null;
    createdApplicationNumber: string | null = null;
    applicationType!: ApplicationType;
    applicationCreationInProgress = false;
    applicationCreationDate = new Date();
    showCustomReasonField = false;
    loadingProhibitionStatus = false;
    ApplicationType = ApplicationType;

    ngOnInit() {
        this.applicationType = this.action;
        if (this.personalIdentity) {
            this.requestInProgress = true;
            this.nomenclatureService.getReasons().subscribe({
                next: response => {
                    this.originalReasons = response;
                    this.buildReasonsDropdownData();
                    this.requestInProgress = false;
                },
            });
        }

        if (this.applicationId) {
            this.currentStep = 1;
        }

        if (this.action !== ApplicationType.RESUME_EID) {
            this.form.controls.reason.setValidators([Validators.required]);
            this.form.controls.customReason.setValidators([Validators.required]);
            this.form.controls.reason.updateValueAndValidity();
            this.form.controls.customReason.updateValueAndValidity();
        }
    }

    ngOnChanges() {
        if (!this.restrictionsStatus) {
            this.loadingProhibitionStatus = true;
            this.pivrClientService
                .checkUidRestrictions({
                    uid: this.personalId,
                    uidType: this.personalIdType,
                })
                .subscribe({
                    next: response => {
                        this.restrictionsStatus = response;
                        this.loadingProhibitionStatus = false;
                    },
                    error: (error: any) => {
                        this.loadingProhibitionStatus = false;
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            default:
                                this.showErrorToast(
                                    this.translateService.translate('global.txtNoConnectionProhibition')
                                );
                                break;
                        }
                    },
                });
        }
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    buildReasonsDropdownData(): void {
        const foundReason = this.originalReasons.find(reason => reason.name === ReasonNamesByAction[this.action]);
        if (foundReason) {
            this.reasons = foundReason.nomenclatures.filter(nomenclature => {
                return (
                    nomenclature.language === this.translateService.getActiveLang() &&
                    nomenclature.permittedUser === PermittedUser.ADMIN
                );
            });
        }
    }

    isDateBelowLawfulAge() {
        return moment().diff(moment(this.personalIdentity.birthDate), 'years') < LAWFUL_AGE;
    }

    onDataSubmit() {
        this.form.markAllAsTouched();
        this.form.controls.reason.markAsDirty();
        this.form.controls.customReason.markAsDirty();
        if (this.form.valid) {
            this.requestInProgress = true;
            this.requestInProgress = true;
            this.mpozeiClientService
                .createEidApplication({
                    eidentityId: this.eIdentity?.eidentityId as string,
                    firstName: this.personalIdentity.personNames?.firstName as string,
                    secondName: this.personalIdentity.personNames?.surname as string,
                    lastName: this.personalIdentity.personNames?.familyName as string,
                    firstNameLatin: this.personalIdentity.personNames?.firstNameLatin as string,
                    secondNameLatin: this.personalIdentity.personNames?.surnameLatin as string,
                    lastNameLatin: this.personalIdentity.personNames?.lastNameLatin as string,
                    citizenIdentifierNumber: this.personalId,
                    citizenIdentifierType: this.personalIdType,
                    applicationType: this.applicationType,
                    applicationSubmissionType: SubmissionType.DESK,
                    deviceId: this.certificate.deviceId,
                    levelOfAssurance: this.certificate.levelOfAssurance,
                    reasonId: this.form.controls.reason.value?.id as string,
                    reasonText: this.form.controls.customReason?.value as string,
                    certificateId: this.certificate.id,
                    citizenship: this.personalIdentity.nationalityList[0].nationalityName as string,
                    personalIdentityDocument: {
                        identityIssuer: this.personalIdentity?.issuerName as string,
                        identityType: this.personalIdentity?.documentType as string,
                        identityIssueDate: this.personalIdentity?.issueDate as string,
                        identityNumber: this.personalIdentity?.identityDocumentNumber as string,
                        identityValidityToDate: this.personalIdentity?.validDate as string,
                    },
                })
                .pipe(
                    concatMap(response => {
                        this.createdApplicationId = response.id;
                        this.createdApplicationNumber = response.applicationNumber;
                        if (this.isCurrentUserMVREmployee) {
                            return this.mpozeiClientService.exportEidApplication({
                                applicationId: response.id,
                            });
                        } else {
                            return of({});
                        }
                    })
                )
                .subscribe({
                    next: response => {
                        this.requestInProgress = false;
                        if (
                            this.userService.hasRole(RoleType.APP_ADMINISTRATOR) ||
                            this.userService.hasRole(RoleType.OPERATOR) ||
                            this.userService.hasRole(RoleType.RU_MVR_ADMINISTRATOR)
                        ) {
                            this.printPDFBlob(response);
                        }
                        this.goToNextStep();
                    },
                    error: error => {
                        this.requestInProgress = false;
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            default:
                                this.showErrorToast(
                                    this.translateService.translate('global.txtCreateApplicationError')
                                );
                                break;
                        }
                    },
                });
        }
    }

    executeCertificateAction() {
        if (this.isCurrentUserMVREmployee) {
            let guardiansFormIsValid = true;
            if (this.guardiansForm) {
                this.guardiansForm.form.markAllAsTouched();
                guardiansFormIsValid = this.guardiansForm.form.valid;
                this.markAllControlsAsDirty(this.guardiansForm.form.controls.array);
            }

            if (this.personalIdentity && guardiansFormIsValid) {
                this.requestInProgress = true;

                const applicationSignObject: ISignApplicationRequestData = {
                    id: this.applicationId || (this.createdApplicationId as string),
                    email: this.eIdentity?.email as string,
                    phoneNumber: this.eIdentity?.phoneNumber as string,
                };

                if (
                    (this.isDateBelowLawfulAge || this.restrictionsStatus.response.isProhibited) &&
                    this.guardiansForm
                ) {
                    this.guardiansForm.form.controls.array.value.forEach(formGroup => {
                        formGroup.personalIdentityDocument.identityIssueDate = this.formatDate(
                            formGroup.personalIdentityDocument.identityIssueDate
                        );
                        formGroup.personalIdentityDocument.identityValidityToDate = this.formatDate(
                            formGroup.personalIdentityDocument.identityValidityToDate
                        );
                    });
                    applicationSignObject.guardians = this.guardiansForm.form.controls.array.value as any;
                }

                this.mpozeiClientService.signEidApplication(applicationSignObject).subscribe({
                    next: response => {
                        this.requestInProgress = false;
                        switch (this.action) {
                            case ApplicationType.RESUME_EID:
                                this.showSuccessToast(
                                    this.translateService.translate('modules.eidManagement.txtCertificateResumeSuccess')
                                );
                                break;
                            case ApplicationType.STOP_EID:
                                this.showSuccessToast(
                                    this.translateService.translate('modules.eidManagement.txtCertificateStopSuccess')
                                );
                                break;
                            case ApplicationType.REVOKE_EID:
                                this.showSuccessToast(
                                    this.translateService.translate('modules.eidManagement.txtCertificateRevokeSuccess')
                                );
                                break;
                        }
                        this.unsavedFormDataExists = false;
                        this.actionExecuted.emit(this.action);
                    },
                    error: error => {
                        this.requestInProgress = false;
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            default:
                                this.showErrorToast(this.translateService.translate('global.txtSignApplicationError'));
                                break;
                        }
                    },
                });
            }
        } else {
            this.mpozeiClientService
                .completeApplication(this.applicationId || (this.createdApplicationId as string))
                .subscribe({
                    next: response => {
                        this.requestInProgress = false;
                        switch (this.action) {
                            case ApplicationType.RESUME_EID:
                                this.showSuccessToast(
                                    this.translateService.translate('modules.eidManagement.txtCertificateResumeSuccess')
                                );
                                break;
                            case ApplicationType.STOP_EID:
                                this.showSuccessToast(
                                    this.translateService.translate('modules.eidManagement.txtCertificateStopSuccess')
                                );
                                break;
                            case ApplicationType.REVOKE_EID:
                                this.showSuccessToast(
                                    this.translateService.translate('modules.eidManagement.txtCertificateRevokeSuccess')
                                );
                                break;
                        }
                        this.unsavedFormDataExists = false;
                        this.actionExecuted.emit(this.action);
                    },
                    error: error => {
                        this.requestInProgress = false;
                        switch (error.status) {
                            case 400:
                                this.showErrorToast(this.translateService.translate('global.txtInvalidDataError'));
                                break;
                            default:
                                this.showErrorToast(this.translateService.translate('global.txtSignApplicationError'));
                                break;
                        }
                    },
                });
        }
    }

    goToNextStep() {
        this.currentStep = this.currentStep + 1;
    }

    printPDFBlob(data: Blob) {
        const blob = new Blob([data], { type: 'application/pdf' });
        const blobURL = URL.createObjectURL(blob);
        let iframe = document.getElementById('printFrame') as HTMLIFrameElement;
        if (!iframe) {
            iframe = document.createElement('iframe');
            iframe.setAttribute('id', 'printFrame');
            iframe.style.display = 'none';
            document.body.appendChild(iframe);
        }

        iframe.src = blobURL;
        iframe.onload = function () {
            setTimeout(function () {
                iframe.focus();
                iframe.contentWindow?.print();
            }, 1);
        };
    }

    reasonText(): string | void {
        switch (this.action) {
            case ApplicationType.REVOKE_EID:
                return this.translateService.translate(
                    'modules.eidManagement.details.txtPleaseSelectReasonForTermination'
                );
            case ApplicationType.STOP_EID:
                return this.translateService.translate(
                    'modules.eidManagement.details.txtPleaseSelectReasonForStopping'
                );
            case ApplicationType.RESUME_EID:
                return this.translateService.translate(
                    'modules.eidManagement.details.txtPleaseSelectReasonForStarting'
                );
            default:
                break;
        }
    }

    reasonLabelText() {
        switch (this.action) {
            case ApplicationType.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.details.txtReasonForTermination');
            case ApplicationType.STOP_EID:
                return this.translateService.translate('modules.eidManagement.details.txtReasonForStopping');
            case ApplicationType.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.details.txtReasonForStarting');
            default:
                return this.translateService.translate('modules.eidManagement.details.txtReason');
        }
    }

    applicationActionText(): string | void {
        switch (this.action) {
            case ApplicationType.REVOKE_EID:
                return this.translateService.translate('modules.eidManagement.details.txtApplicationForTermination');
            case ApplicationType.STOP_EID:
                return this.translateService.translate('modules.eidManagement.details.txtApplicationForStopping');
            case ApplicationType.RESUME_EID:
                return this.translateService.translate('modules.eidManagement.details.txtApplicationForStarting');
            default:
                break;
        }
    }

    get submitButtonData(): { label: string; class: string; icon: string } {
        switch (this.action) {
            case ApplicationType.REVOKE_EID:
                return {
                    label: this.translateService.translate('modules.eidManagement.details.txtTerminateCertificate'),
                    class: 'p-button p-button-sm p-button-danger ',
                    icon: '&#xe99a;',
                };
            case ApplicationType.STOP_EID:
                return {
                    label: this.translateService.translate('modules.eidManagement.details.txtStopCertificate'),
                    class: 'p-button p-button-sm p-button-warning',
                    icon: '&#xe1a2;',
                };
            case ApplicationType.RESUME_EID:
                return {
                    label: this.translateService.translate('modules.eidManagement.details.txtStartCertificate'),
                    class: 'p-button p-button-sm p-button-success',
                    icon: '&#xe1c4;',
                };
            default:
                return {
                    label: this.translateService.translate('modules.eidManagement.details.txtConfirm'),
                    class: '',
                    icon: '',
                };
        }
    }

    formatDate(date: string | null | Date): string {
        if (!date) {
            return '';
        }
        return this.translocoLocaleService.localizeDate(date, this.translocoLocaleService.getLocale(), {
            timeStyle: 'medium',
        });
    }

    onReasonChange(
        formName: FormGroup<{ reason: FormControl<INomenclature | null>; customReason: FormControl<string | null> }>
    ) {
        if (formName.controls['reason'].value) {
            const customReason = this.customReasonNames.indexOf(formName.controls['reason'].value?.name) > -1;
            if (customReason) {
                this.showCustomReasonField = true;
                formName.controls['customReason'].setValidators([Validators.required]);
            } else {
                this.showCustomReasonField = false;
                formName.controls['customReason'].reset();
                formName.controls['customReason'].setValidators(null);
            }
            formName.controls['customReason'].updateValueAndValidity();
        }
    }

    shouldShowGuardiansForm() {
        return (
            this.isCurrentUserMVREmployee &&
            this.form.controls.reason.value?.name !==
                NomenclatureNameWithAdditionalReasonField.STOPPED_BY_ADMINISTRATOR &&
            this.form.controls.reason.value?.name !==
                NomenclatureNameWithAdditionalReasonField.REVOKED_BY_ADMINISTRATOR &&
            !this.loadingProhibitionStatus &&
            (this.isDateBelowLawfulAge() ||
                !!(this.restrictionsStatus && this.restrictionsStatus.response.isProhibited))
        );
    }

    onConfirmExport() {
        let guardiansFormIsValid = true;
        if (this.guardiansForm) {
            this.guardiansForm.form.markAllAsTouched();
            Object.values(this.guardiansForm.form.controls).forEach((control: any) => {
                if (control.controls) {
                    control.controls.forEach((innerControl: any) => {
                        innerControl.markAsDirty();
                    });
                } else {
                    control.markAsDirty();
                }
            });
            guardiansFormIsValid = this.guardiansForm.form.valid;
        }
        if (this.personalIdentity && guardiansFormIsValid) {
            this.requestInProgress = true;
            this.mpozeiClientService
                .addGuardians(this.applicationId || (this.createdApplicationId as string), {
                    guardians: this.guardiansForm.form.controls.array.value as any,
                })
                .subscribe({
                    next: response => {
                        this.mpozeiClientService
                            .exportEidApplicationConfirmation({
                                applicationId: this.applicationId || (this.createdApplicationId as string),
                            })
                            .subscribe({
                                next: response => {
                                    this.requestInProgress = false;
                                    this.printPDFBlob(response);
                                },
                                error: error => {
                                    this.requestInProgress = false;
                                    switch (error.status) {
                                        case 400:
                                            this.showErrorToast(
                                                this.translateService.translate('global.txtInvalidDataError')
                                            );
                                            break;
                                        default:
                                            this.showErrorToast(
                                                this.translateService.translate('global.txtPrintApplicationError')
                                            );
                                            break;
                                    }
                                },
                            });
                    },
                    error: error => {
                        this.requestInProgress = false;
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

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    markAllControlsAsDirty(control: FormGroup | FormArray | FormControl): void {
        if (control instanceof FormGroup) {
            Object.keys(control.controls).forEach(key => {
                this.markAllControlsAsDirty(control.controls[key] as any);
            });
        } else if (control instanceof FormArray) {
            control.controls.forEach(ctrl => {
                this.markAllControlsAsDirty(ctrl as any);
            });
        } else if (control instanceof FormControl) {
            control.markAsDirty();
        }
    }

    get isCurrentUserMVREmployee() {
        return (
            this.userService.hasRole(RoleType.APP_ADMINISTRATOR) ||
            this.userService.hasRole(RoleType.OPERATOR) ||
            this.userService.hasRole(RoleType.RU_MVR_ADMINISTRATOR)
        );
    }
}
