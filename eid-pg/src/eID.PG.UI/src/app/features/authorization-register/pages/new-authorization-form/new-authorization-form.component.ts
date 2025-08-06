import { Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import {
    EGNAdultValidator,
    EGNValidator,
    PinOrAdultEGNValidator,
    PinValidator,
} from '../../../../shared/validators/egn';
import { duplicateToValue, formArrayFormGroupDuplicateValidator } from '../../../../shared/validators/duplicate';
import { BulstatValidator } from '../../../../shared/validators/bulstat';
import { AuthorizationRegisterService } from '../../services/authorization-register.service';
import { Router } from '@angular/router';
import { IdentifierType, OnBehalfOf, TypeOfEmpowerment } from '../../enums/authorization-register.enum';
import { ToastService } from '../../../../shared/services/toast.service';
import { dateIsMinimumTodayValidator, dateMoreThanOrEqualValidate } from '../../../../shared/validators/date';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';
import {
    IEmpowermentServicesPayload,
    IEmpowermentProvidersPayload,
    IEmpowerment,
} from '../../interfaces/authorization-register.interfaces';
import { UserService } from 'src/app/core/services/user.service';
import { ConfirmationDialogService } from '../../../../shared/services/confirmation-dialog.service';

@Component({
    selector: 'app-new-authorization-form',
    templateUrl: './new-authorization-form.component.html',
    styleUrls: ['./new-authorization-form.component.scss'],
})
export class NewAuthorizationFormComponent implements OnInit, OnDestroy {
    constructor(
        public translateService: TranslocoService,
        private confirmationDialogService: ConfirmationDialogService,
        private authorizationRegisterService: AuthorizationRegisterService,
        private toastService: ToastService,
        private router: Router,
        private userService: UserService,
        private formBuilder: FormBuilder
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.breadcrumbItems = [
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                    routerLink: '/authorization-register',
                },
                {
                    label: this.translateService.translate('modules.authorizationRegister.txtFromMe'),
                    routerLink: '/authorization-register/from-me',
                },
                { label: this.translateService.translate('modules.authorizationRegister.txtNewAuthorization') },
            ];

            this.typesOfEmpowerment = [
                {
                    name: this.translateService.translate(
                        'modules.authorizationStatement.statementForm.txtCreateStatementSeparately'
                    ),
                    id: TypeOfEmpowerment.Separately,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationStatement.statementForm.txtCreateStatementTogether'
                    ),
                    id: TypeOfEmpowerment.Together,
                },
            ];

            this.authorizerTypes = [
                {
                    name: this.translateService.translate(
                        'modules.authorizationStatement.statementForm.txtCreateStatementIndividual'
                    ),
                    id: OnBehalfOf.Individual,
                },
                {
                    name: this.translateService.translate(
                        'modules.authorizationStatement.statementForm.txtCreateStatementEntity'
                    ),
                    id: OnBehalfOf.LegalEntity,
                },
            ];

            this.identifierTypes = [
                {
                    name: this.translateService.translate('modules.authorizationStatement.statementForm.EGN'),
                    id: IdentifierType.EGN,
                },
                {
                    name: this.translateService.translate('modules.authorizationStatement.statementForm.LNCh'),
                    id: IdentifierType.LNCh,
                },
            ];
        });
        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            this.onBehalfOfLoading = true;
            this.form.controls.onBehalfOf.disable();
            this.loadProviders(1)
                .then(() => {
                    this.onBehalfOfLoading = false;
                    this.form.controls.onBehalfOf.enable();
                    const state = navigation.extras.state as IEmpowerment;
                    if (state) {
                        this.proxyPersonalNumbers.clear();
                        // in order to preserve typeOfEmpowerment calling addProxy before patchValue
                        state.empoweredUids.forEach((item: any) => {
                            this.addProxy(item.uid, item.uidType, item.name);
                        });
                        this.authorizerUids.clear();
                        const authorizerUidsWithoutCurrentUser = state.authorizerUids?.filter(
                            e => e.uid !== this.authorizerPersonalNumber
                        );
                        authorizerUidsWithoutCurrentUser?.forEach((item: any) => {
                            this.addAuthorizerUid(item.uid, item.uidType, item.name);
                        });
                        const foundProvider = this.providers.find(provider => provider.id === state.providerId);
                        this.form.patchValue({
                            onBehalfOf: state.onBehalfOf,
                            authorizerPersonalNumber: this.authorizerPersonalNumber,
                            typeOfEmpowerment: state.empoweredUids.length > 1 ? TypeOfEmpowerment.Together : null,
                            volume: state.volumeOfRepresentation,
                            startDate: new Date(),
                            endDate: null,
                            issuerPosition: state.issuerPosition,
                            bulstat: state.uid as string,
                            legalEntityName: state.name as string,
                            authorizerPersonalIdentifier: this.authorizerPersonalIdentifier,
                        });

                        this.resetOnBehalfOfControls();

                        // We don't want to trigger ngModelChange which will trigger request
                        if (foundProvider) {
                            this.form.controls.provider.setValue(
                                {
                                    name: state.providerName,
                                    id: foundProvider.id,
                                },
                                {
                                    emitViewToModelChange: false,
                                }
                            );
                            this.loadServices(1).then(() => {
                                const foundService = this.services.find(
                                    service => service.serviceNumber === state.serviceId
                                );
                                // We dont want to trigger ngModelChange which will trigger request
                                if (foundService) {
                                    this.form.controls.service.setValue(foundService, {
                                        emitViewToModelChange: false,
                                    });
                                    this.getScopesByService();
                                }
                            });
                        }
                    }
                })
                .catch(() => {
                    this.onBehalfOfLoading = false;
                    this.form.controls.onBehalfOf.enable();
                });
        } else {
            this.loadProviders(1);
        }
    }

    get _initialBreadcrumbs(): IBreadCrumbItems[] {
        return ([] as IBreadCrumbItems[]).concat([
            {
                label: this.translateService.translate('modules.authorizationRegister.txtTitle'),
                routerLink: '/authorization-register',
            },
            {
                label: this.translateService.translate('modules.authorizationRegister.txtFromMe'),
                routerLink: '/authorization-register/from-me',
            },
            { label: this.translateService.translate('modules.authorizationRegister.txtNewAuthorization') },
        ]);
    }

    breadcrumbItems: IBreadCrumbItems[] = this._initialBreadcrumbs;

    get authorizerPersonalNumber(): string {
        return this.userService.user.uid;
    }

    get authorizerPersonalIdentifier(): string {
        return this.translateService.translate(
            'modules.authorizationStatement.statementForm.' + this.userService.user.uidType
        );
    }

    get authorizerNames(): string {
        return this.userService.user.name;
    }

    languageChangeSubscription: Subscription;

    cyrillicPattern = /^[аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ -']+$/;
    legalEntityNamePattern = /^[аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ -.&\d]+$/;

    identifierForm = this.formBuilder.group({
        uidType: new FormControl(IdentifierType.EGN, [Validators.required]),
        uid: new FormControl('', {
            validators: [Validators.required, Validators.pattern('[0-9]+'), EGNAdultValidator(), EGNValidator()],
            updateOn: 'change',
        }),
        name: new FormControl('', [Validators.required, Validators.pattern(this.cyrillicPattern)]),
    });
    form = new FormGroup({
        onBehalfOf: new FormControl<OnBehalfOf | null>(null, Validators.required),
        authorizerPersonalNumber: new FormControl<string>(
            {
                value: this.authorizerPersonalNumber,
                disabled: true,
            },
            {
                validators: [Validators.required, Validators.pattern('[0-9]+'), PinOrAdultEGNValidator()],
                nonNullable: true,
            }
        ),
        authorizerUids: this.formBuilder.array(
            [],
            [formArrayFormGroupDuplicateValidator(), duplicateToValue(this.authorizerPersonalNumber)]
        ),
        proxyPersonalNumbers: this.formBuilder.array(
            [this.identifierForm],
            [
                Validators.required,
                formArrayFormGroupDuplicateValidator(),
                duplicateToValue(this.authorizerPersonalNumber),
            ]
        ),
        typeOfEmpowerment: new FormControl<number | null>(
            {
                value: null,
                disabled: true,
            },
            Validators.required
        ),
        provider: new FormControl<{ name: string; id: string | undefined } | null>(null, Validators.required),
        service: new FormControl<{ name: string; id: string; serviceNumber: number } | null>(
            {
                value: null,
                disabled: true,
            },
            Validators.required
        ),
        volume: new FormControl<{ name: string }[] | null>(
            {
                value: null,
                disabled: true,
            },
            Validators.required
        ),
        startDate: new FormControl<Date>(new Date(), {
            validators: [Validators.required, dateIsMinimumTodayValidator({ optional: false })],
            nonNullable: true,
        }),
        endDate: new FormControl<Date | null>(null, {
            validators: [dateMoreThanOrEqualValidate('startDate'), dateIsMinimumTodayValidator({ optional: true })],
        }),
        issuerPosition: new FormControl<string | null>(null, Validators.required),
        bulstat: new FormControl<string | null>(null, {
            updateOn: 'change',
        }),
        legalEntityName: new FormControl<string | null>(null, Validators.required),
        authorizerPersonalIdentifier: new FormControl<string | null>(
            {
                value: this.authorizerPersonalIdentifier,
                disabled: true,
            },
            Validators.required
        ),
    });
    authorizerTypes = [
        {
            name: this.translateService.translate(
                'modules.authorizationStatement.statementForm.txtCreateStatementIndividual'
            ),
            id: OnBehalfOf.Individual,
        },
        {
            name: this.translateService.translate(
                'modules.authorizationStatement.statementForm.txtCreateStatementEntity'
            ),
            id: OnBehalfOf.LegalEntity,
        },
    ];

    identifierTypes = [
        {
            name: this.translateService.translate('modules.authorizationStatement.statementForm.EGN'),
            id: IdentifierType.EGN,
        },
        {
            name: this.translateService.translate('modules.authorizationStatement.statementForm.LNCh'),
            id: IdentifierType.LNCh,
        },
    ];

    onBehalfOf = OnBehalfOf;
    providers: any[] = [];
    services: any[] = [];
    volume: { name: string }[] = [];
    typesOfEmpowerment: any[] = [
        {
            name: this.translateService.translate(
                'modules.authorizationStatement.statementForm.txtCreateStatementSeparately'
            ),
            id: 0,
        },
        {
            name: this.translateService.translate(
                'modules.authorizationStatement.statementForm.txtCreateStatementTogether'
            ),
            id: 1,
        },
    ];
    minDate = new Date();
    maxProxyPersonalNumbers = 10;
    isPreviewMode = false;
    requestInProgress = false;
    submitted = false;
    pageSize = 1000;
    providersLoading = false;
    servicesLoading = false;
    volumeLoading = false;
    onBehalfOfLoading = false;
    OnBehalfOf = OnBehalfOf;
    unsavedFormDataExists = true;
    @ViewChild('helpSection') helpSection: ElementRef | undefined;
    @HostListener('window:beforeunload', ['$event'])
    beforeUnloadHandler() {
        // returning true will navigate without confirmation
        // returning false will show a confirm dialog before navigating away
        return !this.unsavedFormDataExists;
    }

    endDateMinValue(): Date {
        return this.form.controls.startDate.value || new Date();
    }

    ngOnInit() {
        this.breadcrumbItems = this._initialBreadcrumbs;
        if (this.form.controls.startDate) {
            this.form.controls.startDate.valueChanges.subscribe({
                next: () => {
                    this.form.controls.endDate.updateValueAndValidity();
                },
            });
        }
        this.form.controls.authorizerUids.valueChanges.subscribe({
            next: value => {
                const validators = [Validators.required, formArrayFormGroupDuplicateValidator()];
                if (this.form.controls.onBehalfOf.value === OnBehalfOf.LegalEntity && value.length > 0) {
                    this.proxyPersonalNumbers.setValidators(validators);
                } else {
                    validators.push(duplicateToValue(this.authorizerPersonalNumber));
                    this.proxyPersonalNumbers.setValidators(validators);
                }
                this.proxyPersonalNumbers.updateValueAndValidity();
            },
        });
    }

    ngOnDestroy() {
        this.languageChangeSubscription.unsubscribe();
    }

    resetOnBehalfOfControls() {
        const validators = [Validators.required, formArrayFormGroupDuplicateValidator()];
        if (this.form.controls.onBehalfOf.value === OnBehalfOf.LegalEntity) {
            this.form.controls.bulstat.setValidators([Validators.required, BulstatValidator()]);
            this.form.controls.issuerPosition.setValidators([Validators.required]);
            this.form.controls.legalEntityName.setValidators([
                Validators.required,
                Validators.pattern(this.legalEntityNamePattern),
            ]);
            if (this.authorizerUids.length === 0) {
                validators.push(duplicateToValue(this.authorizerPersonalNumber));
            }
        } else {
            this.form.controls.bulstat.setValidators(null);
            this.form.controls.issuerPosition.setValidators(null);
            this.form.controls.legalEntityName.setValidators(null);
            this.form.controls.bulstat.patchValue(null);
            this.form.controls.issuerPosition.patchValue(null);
            this.form.controls.legalEntityName.patchValue(null);
            this.form.controls.authorizerUids.clear();
            validators.push(duplicateToValue(this.authorizerPersonalNumber));
        }
        this.form.controls.bulstat.markAsPristine();
        this.form.controls.bulstat.markAsUntouched();
        this.form.controls.issuerPosition.markAsPristine();
        this.form.controls.issuerPosition.markAsUntouched();
        this.form.controls.legalEntityName.markAsPristine();
        this.form.controls.legalEntityName.markAsUntouched();
        this.form.controls.authorizerUids.markAsPristine();
        this.form.controls.authorizerUids.markAsUntouched();
        this.proxyPersonalNumbers.setValidators(validators);
        this.proxyPersonalNumbers.updateValueAndValidity();
    }

    onSubmit() {
        this.submitted = true;
        Object.keys(this.form.controls).forEach(key => {
            if (
                (this.form.get(key) && typeof this.form.get(key)?.value === 'string') ||
                this.form.get(key)?.value instanceof String
            ) {
                this.form.get(key)?.setValue(this.form.get(key)?.value.trim());
            }
        });
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.requestInProgress = true;
            let additionalMessage = this.translateService.translate(
                'modules.authorizationStatement.statementForm.txtNoExpiryDateEmpowermentMessage'
            );
            if (this.form.controls.endDate.value) {
                additionalMessage = '';
            }
            this.confirmationDialogService.showConfirmation({
                rejectButtonStyleClass: 'p-button-danger',
                message: this.translateService.translate(
                    'modules.authorizationStatement.statementForm.txtCreateStatementMessage',
                    {
                        additionalMessage: additionalMessage,
                    }
                ),
                header: this.translateService.translate(
                    'modules.authorizationStatement.statementForm.txtCreateStatementTitle'
                ),
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: this.translateService.translate(
                    'modules.authorizationStatement.statementForm.txtCreateStatementButtonLabel'
                ),
                accept: () => {
                    let typeOfEmpowerment = TypeOfEmpowerment.Separately;
                    if (this.form.controls.typeOfEmpowerment.value !== null) {
                        typeOfEmpowerment = this.form.controls.typeOfEmpowerment.value as number;
                    }
                    let expiryDate = null;
                    if (this.form.controls.endDate.value) {
                        const endDate = new Date(this.form.controls.endDate.value);
                        endDate.setHours(23, 59, 59, 999);
                        expiryDate = endDate;
                    }
                    // It's important not to mutate startDate's value because it affects endDate's value
                    const startDateValue = new Date(this.form.controls.startDate.value);
                    startDateValue.setHours(0, 0, 0, 0);

                    let uid: any = this.userService.user.uid;
                    if (this.form.controls.onBehalfOf.value === OnBehalfOf.LegalEntity) {
                        uid = this.form.controls.bulstat.value?.toString();
                    }

                    this.authorizationRegisterService
                        .addEmpowerment({
                            onBehalfOf: this.form.controls.onBehalfOf.value as OnBehalfOf,
                            issuerPosition: this.form.controls.issuerPosition.value as string,
                            uid: uid,
                            uidType: this.userService.user.uidType,
                            name: this.form.controls.legalEntityName.value || '',
                            empoweredUids: this.form.controls.proxyPersonalNumbers.value as object[],
                            typeOfEmpowerment: typeOfEmpowerment,
                            providerId: this.form.controls.provider.value?.id as string,
                            providerName: this.form.controls.provider.value?.name as string,
                            serviceId: this.form.controls.service.value?.serviceNumber as number,
                            serviceName: this.form.controls.service.value?.name as string,
                            volumeOfRepresentation: this.form.controls.volume.value?.map(item => {
                                return {
                                    name: item.name,
                                };
                            }) as any,
                            startDate: startDateValue,
                            expiryDate: expiryDate,
                            authorizerUids: this.form.controls.authorizerUids.value,
                        })
                        .subscribe({
                            next: () => {
                                this.unsavedFormDataExists = false;
                                this.showSuccessToast(
                                    this.translateService.translate(
                                        'modules.authorizationStatement.statementForm.txtCreateStatementSuccess'
                                    )
                                );
                                this.router.navigate(['/authorization-register']);
                            },
                            error: error => {
                                switch (error.status) {
                                    case 400:
                                        this.showErrorToast(
                                            this.translateService.translate('global.txtInvalidDataError')
                                        );
                                        break;
                                    default:
                                        this.showErrorToast(
                                            this.translateService.translate('global.txtUnexpectedError')
                                        );
                                        break;
                                }
                                this.requestInProgress = false;
                            },
                        });
                },
                reject: () => {
                    this.requestInProgress = false;
                },
            });
        }
    }

    validateAndPreview() {
        this.submitted = true;
        this.form.markAllAsTouched();
        Object.values(this.form.controls).forEach((control: any) => {
            if (control.controls) {
                control.controls.forEach((innerControl: any) => {
                    if (innerControl.controls) {
                        Object.values(innerControl.controls).forEach((innerControl2: any) => {
                            innerControl2.markAsDirty();
                        });
                    } else {
                        innerControl.markAsDirty();
                    }
                });
            } else {
                control.markAsDirty();
            }
        });
        if (this.form.valid) {
            this.isPreviewMode = true;
        }
    }

    cancelPreview() {
        this.isPreviewMode = false;
        this.submitted = false;
    }

    cancel() {
        this.router.navigate(['/authorization-register']);
    }

    get proxyPersonalNumbers() {
        return this.form.get('proxyPersonalNumbers') as FormArray<FormGroup>;
    }

    addProxy(identifierValue = null, uidType = IdentifierType.EGN, name = '') {
        if (this.proxyPersonalNumbers) {
            const validators = [Validators.required, Validators.pattern('[0-9]+')];
            if (IdentifierType.EGN === uidType) {
                validators.push(EGNAdultValidator(), EGNValidator());
            } else if (IdentifierType.LNCh === uidType) {
                validators.push(PinValidator());
            }
            const fb = this.formBuilder.group({
                uidType: new FormControl(uidType, [Validators.required]),
                uid: new FormControl(identifierValue, validators),
                name: new FormControl<string>(name, [Validators.required, Validators.pattern(this.cyrillicPattern)]),
                updateOn: 'change',
            });
            this.proxyPersonalNumbers.push(fb);

            if (this.proxyPersonalNumbers.length > 1) {
                this.form.get('typeOfEmpowerment')?.enable();
                this.form.get('typeOfEmpowerment')?.setValue(null);
                this.form.get('typeOfEmpowerment')?.markAsUntouched();
            }
        }
    }

    removeProxyPersonalNumber(i: number) {
        this.proxyPersonalNumbers.removeAt(i);
        if (this.proxyPersonalNumbers.length === 1) {
            this.form.get('typeOfEmpowerment')?.disable();
            this.form.get('typeOfEmpowerment')?.setValue(null);
            this.form.get('typeOfEmpowerment')?.markAsUntouched();
            this.form.get('typeOfEmpowerment')?.markAsPristine();
        }
    }

    get authorizerUids() {
        return this.form.get('authorizerUids') as FormArray<FormGroup>;
    }

    addAuthorizerUid(identifierValue = null, uidType = IdentifierType.EGN, name = '') {
        if (this.authorizerUids) {
            const validators = [Validators.required, Validators.pattern('[0-9]+')];
            if (IdentifierType.EGN === uidType) {
                validators.push(EGNAdultValidator(), EGNValidator());
            } else if (IdentifierType.LNCh === uidType) {
                validators.push(PinValidator());
            }
            const fb = this.formBuilder.group({
                uidType: new FormControl(uidType, [Validators.required]),
                uid: new FormControl(identifierValue, validators),
                name: new FormControl<string>(name, [Validators.required, Validators.pattern(this.cyrillicPattern)]),
                updateOn: 'change',
            });
            this.authorizerUids.push(fb);
        }
    }

    removeAuthorizerUid(i: number) {
        this.authorizerUids.removeAt(i);
    }

    enableService() {
        this.form.get('service')?.enable();
        this.loadServices(1);
        if (this.form.get('service')?.value) {
            this.form.get('service')?.reset();
            this.form.get('volume')?.disable();
        }
    }

    enableVolume() {
        if (this.form.get('service')?.value) {
            this.form.get('volume')?.enable();
            this.form.get('volume')?.reset();
            this.getScopesByService();
        }
    }

    getSelectedValueLabel(data: any[], selectedValue: number | string | null | any[]) {
        if (Array.isArray(selectedValue)) {
            return data.filter(type => selectedValue.some(e => e.id === type.id)).map(found => found.name);
        } else {
            return selectedValue !== undefined ? data.find(type => type.id === selectedValue)?.name : '';
        }
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    showSuccessToast(message: string) {
        this.toastService.showSuccessToast(this.translateService.translate('global.txtSuccessTitle'), message);
    }

    async loadProviders(page = 1) {
        return new Promise((resolve, reject) => {
            if (page === 1) {
                this.providers = [];
            }

            const params: IEmpowermentProvidersPayload = { pageIndex: page, pageSize: this.pageSize };
            this.providersLoading = true;
            this.form.controls.provider.disable();

            this.authorizationRegisterService.getProviders(params).subscribe({
                next: async (response: any) => {
                    this.providers = this.providers.concat(response.data);
                    const additionalCallsNeeded = Math.ceil(response.totalItems / this.providers.length) - 1;
                    if (additionalCallsNeeded > 0) {
                        await this.loadProviders(page + 1);
                    } else {
                        this.providersLoading = false;
                        this.form.controls.provider.enable();
                        resolve(this.providers);
                    }
                    // reject([]);
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
                    this.providersLoading = false;
                    this.form.controls.provider.enable();
                    reject(error);
                },
            });
        });
    }

    async loadServices(page = 1) {
        return new Promise((resolve, reject) => {
            if (page === 1) {
                this.services = [];
            }

            const params: IEmpowermentServicesPayload = {
                pageIndex: page,
                pageSize: this.pageSize,
                providerid: this.form.controls.provider.value?.id,
                includeEmpowermentOnly: true,
            };
            this.servicesLoading = true;
            this.form.controls.service.disable();

            this.authorizationRegisterService.getServices(params).subscribe({
                next: async (response: any) => {
                    this.services = this.services.concat(
                        response.data.map((item: any) => {
                            return { name: item.name, id: item.id, serviceNumber: item.serviceNumber };
                        })
                    );
                    const additionalCallsNeeded = Math.ceil(response.totalItems / this.services.length) - 1;
                    if (additionalCallsNeeded > 0) {
                        await this.loadServices(page + 1);
                    } else {
                        this.servicesLoading = false;
                        this.form.controls.service.enable();
                        resolve(this.services);
                    }
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
                    this.servicesLoading = false;
                    this.form.controls.service.enable();
                    reject(error);
                },
            });
        });
    }

    getScopesByService() {
        this.volumeLoading = true;
        this.form.controls.volume.disable();
        this.authorizationRegisterService
            .getScopesByService({
                serviceId: this.form.controls.service.value?.id,
                includeDeleted: false,
            })
            .subscribe({
                next: response => {
                    this.volume = response.map((item: any) => {
                        return { name: item['name'] };
                    });
                    this.volumeLoading = false;
                    this.form.controls.volume.enable();
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
                    this.volumeLoading = false;
                    this.form.controls.volume.enable();
                },
            });
    }

    get selectedProviderName() {
        return (
            this.form.controls.provider.value?.name || this.translateService.translate('global.txtDropdownPlaceholder')
        );
    }

    get selectedServiceName() {
        return (
            this.form.controls.service.value?.name || this.translateService.translate('global.txtDropdownPlaceholder')
        );
    }

    get selectedScopeNames() {
        if (this.form.controls.volume.value) {
            return this.form.controls.volume.value?.map(scope => scope.name);
        } else {
            return this.translateService.translate('global.txtDropdownPlaceholder');
        }
    }

    onIdentifierChange(event: any, formGroup: FormGroup) {
        if (event.value === IdentifierType.EGN) {
            formGroup.controls['uid'].setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                EGNAdultValidator(),
                EGNValidator(),
            ]);
        } else if (event.value === IdentifierType.LNCh) {
            formGroup.controls['uid'].setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                PinValidator(),
            ]);
        }
        formGroup.controls['uid'].updateValueAndValidity();
    }

    computeRequiredTranslation(value: any): string {
        let validationMessage = this.translateService.translate('modules.authorizationStatement.statementForm.txtEGN');
        if (IdentifierType.LNCh === value) {
            validationMessage = this.translateService.translate('modules.authorizationStatement.statementForm.txtLNCH');
        }
        return this.translateService.translate('validations.txtPleaseEnterField', {
            field: validationMessage,
        });
    }

    identifierTypeTranslation(uidType: IdentifierType) {
        return this.identifierTypes.find(type => type.id === uidType)?.name;
    }

    scrollToHelpSection() {
        this.helpSection?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }

    helpPanelContent() {
        if (this.form.controls.onBehalfOf.valid) {
            return this.form.controls.onBehalfOf.value === OnBehalfOf.LegalEntity
                ? this.translateService.translate('help.txtEmpowermentLegalEntity')
                : this.translateService.translate('help.txtEmpowermentIndividual');
        } else {
            return this.translateService.translate('help.txtEmpowermentNoSelection');
        }
    }
}
