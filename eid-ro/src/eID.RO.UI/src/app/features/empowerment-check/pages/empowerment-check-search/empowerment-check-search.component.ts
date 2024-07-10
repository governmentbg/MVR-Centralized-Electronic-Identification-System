import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import {
    BulstatValidator,
    EGNAdultValidator,
    EGNValidator,
} from '../../shared/validators/uid-validator';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { OnBehalfOf } from '../../enums/empowerment-check.enum';
import { AuthorizationRegisterService } from 'src/app/features/authorization-register/services/authorization-register.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { IEmpowermentServicesPayload } from 'src/app/features/authorization-register/interfaces/authorization-register.interfaces';
import jwt_decode from 'jwt-decode';
import { IdentifierType } from 'src/app/features/authorization-register/enums/authorization-register.enum';
import { PinValidator } from 'src/app/shared/validators/egn';
import {AppConfigService} from "../../../../core/services/config.service";

@Component({
    selector: 'app-empowerment-check-search',
    templateUrl: './empowerment-check-search.component.html',
    styleUrls: ['./empowerment-check-search.component.scss'],
})
export class EmpowermentCheckSearchComponent {
    constructor(
        public translateService: TranslocoService,
        private router: Router,
        private authorizationRegisterService: AuthorizationRegisterService,
        private toastService: ToastService,
        private appConfigService: AppConfigService
    ) {
        this.languageChangeSubscription = translateService.langChanges$.subscribe(() => {
            this.authorizerTypes = [
                {
                    name: this.translateService.translate('empowermentCheckModule.txtCreateStatementIndividual'),
                    id: OnBehalfOf.Individual,
                },
                {
                    name: this.translateService.translate('empowermentCheckModule.txtCreateStatementEntity'),
                    id: OnBehalfOf.LegalEntity,
                },
            ];
        });

        const token = localStorage.getItem(appConfigService.config.keycloak.keycloakTokenKey);
        if (token) {
            const decoded: any = jwt_decode(token);
            if (decoded && decoded.BATCHID) {
                this.supplierId = decoded.BATCHID;
                this.loadServices(1);
            }
        }

        const navigation = this.router.getCurrentNavigation();
        if (navigation) {
            const state = navigation.extras.state as any;
            if (state) {
                this.form.patchValue({
                    onBehalfOf: state.onBehalfOf || null,
                    authorizerUid: state.authorizerUid || null,
                    empoweredUid: state.empoweredUid || null,
                    service: state.service || null,
                    volumeOfRepresentation: state.volumeOfRepresentation || null,
                    statusOn: state.statusOn || new Date(),
                    authorizerUidType: state.authorizerUidType,
                    empoweredUidType: state.empoweredUidType,
                });
                if (state.volumeOfRepresentation) {
                    this.enableVolume(state.volumeOfRepresentation);
                } else {
                    this.enableVolume();
                }
                this.onAuthorizerUidTypeChange(state.authorizerUidType);
                this.onEmpoweredUidTypeChange(state.empoweredUidType);
            }
        }
    }

    languageChangeSubscription: Subscription;
    form = new FormGroup({
        onBehalfOf: new FormControl<string | null>(null),
        authorizerUid: new FormControl<string | null>(null, [
            Validators.required,
            Validators.pattern('[0-9]+'),
            EGNAdultValidator(),
            EGNValidator(),
        ]),
        empoweredUid: new FormControl<string | null>(null, [
            Validators.required,
            Validators.pattern('[0-9]+'),
            EGNAdultValidator(),
            EGNValidator(),
        ]),
        service: new FormControl<{ name: string; id: string; serviceNumber: number } | null>(
            {
                value: null,
                disabled: true,
            },
            Validators.required
        ),

        volumeOfRepresentation: new FormControl<{ name: string; id: string; code: number }[] | null>({
            value: null,
            disabled: true,
        }),
        statusOn: new FormControl<Date>(new Date(), Validators.required),
        authorizerUidType: new FormControl(IdentifierType.EGN, [Validators.required]),
        empoweredUidType: new FormControl(IdentifierType.EGN, [Validators.required]),
    });
    requestInProgress = false;
    submitted = false;
    onBehalfOf = OnBehalfOf;
    authorizerTypes = [
        {
            name: this.translateService.translate('empowermentCheckModule.txtCreateStatementIndividual'),
            id: OnBehalfOf.Individual,
        },
        {
            name: this.translateService.translate('empowermentCheckModule.txtCreateStatementEntity'),
            id: OnBehalfOf.LegalEntity,
        },
    ];
    volume: any[] = [];
    services: any[] = [];
    volumeLoading = false;
    servicesLoading = false;
    pageSize = 1000;
    supplierId = '';
    identifierType = IdentifierType;
    identifierTypes = [
        {
            name: this.translateService.translate('empowermentCheckModule.EGN'),
            id: IdentifierType.EGN,
        },
        {
            name: this.translateService.translate('empowermentCheckModule.LNCh'),
            id: IdentifierType.LNCh,
        },
        {
            name: this.translateService.translate('empowermentCheckModule.NotSpecified'),
            id: IdentifierType.NotSpecified,
        },
    ];

    identifierTypesWIthoutBulstat = [
        {
            name: this.translateService.translate('empowermentCheckModule.EGN'),
            id: IdentifierType.EGN,
        },
        {
            name: this.translateService.translate('empowermentCheckModule.LNCh'),
            id: IdentifierType.LNCh,
        },
    ];

    async loadServices(page = 1) {
        return new Promise((resolve, reject) => {
            if (page === 1) {
                this.services = [];
            }
            const params: IEmpowermentServicesPayload = {
                pageIndex: page,
                pageSize: this.pageSize,
                batchId: this.supplierId,
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

    onBehalfOfChange() {
        this.identifierTypes = [
            {
                name: this.translateService.translate('empowermentCheckModule.EGN'),
                id: IdentifierType.EGN,
            },
            {
                name: this.translateService.translate('empowermentCheckModule.LNCh'),
                id: IdentifierType.LNCh,
            },
            {
                name: this.translateService.translate('empowermentCheckModule.NotSpecified'),
                id: IdentifierType.NotSpecified,
            },
        ];
        if (this.form.controls.onBehalfOf.value === OnBehalfOf.Individual) {
            this.identifierTypes = [
                {
                    name: this.translateService.translate('empowermentCheckModule.EGN'),
                    id: IdentifierType.EGN,
                },
                {
                    name: this.translateService.translate('empowermentCheckModule.LNCh'),
                    id: IdentifierType.LNCh,
                },
            ];
            this.form.controls.authorizerUidType.patchValue(IdentifierType.EGN);
            this.form.controls.authorizerUid.setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                EGNAdultValidator(),
                EGNValidator(),
            ]);
            this.form.controls.authorizerUid.updateValueAndValidity();
        }
    }

    enableVolume(preselectData?: any) {
        if (this.form.get('service')?.value) {
            this.form.get('volumeOfRepresentation')?.enable();
            this.form.get('volumeOfRepresentation')?.reset();
            this.getScopesByService(preselectData);
        }
    }

    getScopesByService(preselectData?: any) {
        this.volumeLoading = true;
        this.form.controls.volumeOfRepresentation.disable();
        this.authorizationRegisterService
            .getScopesByService({
                serviceId: this.form.controls.service.value?.id,
                includeDeleted: false,
            })
            .subscribe({
                next: (response: any) => {
                    this.volume = response;
                    this.volumeLoading = false;
                    this.form.controls.volumeOfRepresentation.enable();
                    if (preselectData) {
                        this.form.patchValue({
                            volumeOfRepresentation: preselectData,
                        });
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
                    this.volumeLoading = false;
                    this.form.controls.volumeOfRepresentation.enable();
                },
            });
    }

    onSubmit() {
        this.form.markAllAsTouched();
        this.submitted = true;
        if (this.form.valid) {
            this.submitted = false;
            this.form.markAsPristine();
            this.router.navigate(['empowerment-check/search-results'], {
                skipLocationChange: true,
                state: this.form.value,
            });
        }
    }

    showErrorToast(message: string) {
        this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), message);
    }

    onAuthorizerUidTypeChange(value: string) {
        if (value === IdentifierType.EGN) {
            this.form.controls.authorizerUid.setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                EGNAdultValidator(),
                EGNValidator(),
            ]);
        } else if (value === IdentifierType.LNCh) {
            this.form.controls.authorizerUid.setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                PinValidator(),
            ]);
        } else if (value === IdentifierType.NotSpecified) {
            this.form.controls.authorizerUid.setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                BulstatValidator(),
            ]);
        }
        this.form.controls.authorizerUid.updateValueAndValidity();
    }

    onEmpoweredUidTypeChange(value: string) {
        if (value === IdentifierType.EGN) {
            this.form.controls.empoweredUid.setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                EGNAdultValidator(),
                EGNValidator(),
            ]);
        } else if (value === IdentifierType.LNCh) {
            this.form.controls.empoweredUid.setValidators([
                Validators.required,
                Validators.pattern('[0-9]+'),
                PinValidator(),
            ]);
        }
        this.form.controls.empoweredUid.updateValueAndValidity();
    }

    computeTranslation(translationKey: string, formControlName: any): string {
        const value = (this.form.controls as any)[formControlName].value;
        let validationМеssage = this.translateService.translate('empowermentCheckModule.txtEGNEmpoweredPerson');
        if (IdentifierType.LNCh === value) {
            validationМеssage = this.translateService.translate('empowermentCheckModule.txtLNCHEmpoweredPerson');
        } else if (IdentifierType.NotSpecified === value) {
            validationМеssage = this.translateService.translate('empowermentCheckModule.txtBulstat');
        }
        return this.translateService.translate(`validations.${translationKey}`, {
            field: validationМеssage,
        });
    }
}
