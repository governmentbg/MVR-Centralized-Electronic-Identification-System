import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ProviderServicesService } from '../../provider-services.service';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { Router } from '@angular/router';
import { scopeDuplicateScopeValidator } from '../../utils';
import { minimumLevelOfAssuranceEnum } from '../../services.dto';
import { CurrentProviderService } from '@app/core/services/current-provider.service';

type AssuranceLevelOptions = { name: string; value: string };
type PersonalDataType = { name: string; value: string };

@Component({
    selector: 'app-add-service',
    templateUrl: './add-service.component.html',
    styleUrls: ['./add-service.component.scss'],
})
export class AddServiceComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private providerServicesService: ProviderServicesService,
        private translateService: TranslocoService,
        private toastService: ToastService,
        private router: Router,
        private fb: FormBuilder,
        private currentProviderService: CurrentProviderService
    ) {}

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.services.services-table.title'),
                        onClick: () => this.router.navigate(['/services']),
                    },
                    {
                        label: this.translateService.translate('modules.services.service-add.txtTitle'),
                    },
                ];
                this.assuranceLevelOptions = [
                    {
                        value: minimumLevelOfAssuranceEnum.Enum.High,
                        name: this.translateService.translate('modules.services.enums.High'),
                    },
                    {
                        value: minimumLevelOfAssuranceEnum.Enum.Substantial,
                        name: this.translateService.translate('modules.services.enums.Substantial'),
                    },
                ];
                this.personalDataOptions = this.generatePersonalInformationOptions(
                    this.personalDataOptions.map(item => item.value)
                );

                this.form.controls.personalData.setValue(
                    this.generatePersonalInformationOptions(
                        this.form.controls.personalData.value?.map(item => item.value) || []
                    )
                );
            })
        );
        this.subscriptions.add(
            this.form.controls.isEmpowerment.valueChanges.subscribe(value => {
                const control = this.form.controls.scopes;
                if (value) {
                    control.setValidators([Validators.required]);
                    control.updateValueAndValidity();
                } else {
                    control.setValidators([]);
                    control.updateValueAndValidity();
                }
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }
    breadcrumbItems: IBreadCrumbItems[] = [];
    serviceNamePattern = /^[A-ZА-Я][a-zA-Z0-9а-яА-Я- аАбБвВгГдДеЕжЖзЗиИйЙкКлЛмМнНоОпПрРсСтТуУфФхХцЦчЧшШщЩъЪьЬюЮяЯ]*$/;
    assuranceLevelOptions: AssuranceLevelOptions[] = [];
    selectedAssuranceLevel = minimumLevelOfAssuranceEnum.Enum.High;
    personalDataOptions: PersonalDataType[] = [];

    roles: { name: string; id: string }[] = [];

    form = new FormGroup({
        name: new FormControl('', [Validators.required, Validators.pattern(this.serviceNamePattern)]),
        description: new FormControl('', [Validators.required]),
        isEmpowerment: new FormControl<boolean>(false, [Validators.required]),
        personalData: new FormControl<PersonalDataType[]>([], [Validators.required]),
        scopes: new FormArray<FormControl<string>>([], [Validators.required]),
    });

    suggestions: string[] = [];

    defaultScopeQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchProviderServicesDefaultScope,
        onSuccess: data => {
            data.forEach(scope => {
                this.addScope(scope);
            });
            this.suggestions = [...this.suggestions, ...data];
        },
    }).execute();

    availableScopeQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchProviderServicesAvailableScope,
        onSuccess: data => {
            this.suggestions = [...this.suggestions, ...data];
        },
    }).execute();

    personalInformationQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchPersonalInformationEnum,
        onInit: () => {
            this.form.controls.personalData.disable();
        },
        onSuccess: data => {
            this.personalDataOptions = this.generatePersonalInformationOptions(data);
        },
        onComplete: () => {
            this.form.controls.personalData.enable();
        },
    }).execute();

    addServiceMutation = new RequestHandler({
        requestFunction: this.providerServicesService.addService,
        onInit: () => {
            this.form.disable();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.services.service-add.toasts.addSuccessful')
            );
            this.router.navigate(['/services']);
        },
        onError: () => {
            this.form.enable();
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.services.service-add.toasts.addFailed')
            );
        },
        onComplete: () => {
            this.form.enable();
        },
    });

    generatePersonalInformationOptions = (personalData: string[]) => {
        return personalData.map(item => ({
            name: this.translateService.translate(`modules.services.enums.${item}`),
            value: item,
        }));
    };

    addScope(defaultValue = ''): void {
        this.scopes.push(this.fb.control(defaultValue, [Validators.required, scopeDuplicateScopeValidator()]));
    }

    removeScope(index: number): void {
        this.scopes.removeAt(index);
    }

    get scopes(): FormArray {
        return this.form.get('scopes') as FormArray;
    }
    get isLoading() {
        return (
            this.defaultScopeQuery.isLoading || this.addServiceMutation.isLoading || this.availableScopeQuery.isLoading
        );
    }

    onSubmit() {
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        const providerDetailsId = this.currentProviderService.getProvider()?.detailsId;
        if (!providerDetailsId) {
            throw new Error('Provider details id not available when adding service');
        }
        this.addServiceMutation.execute({
            providerDetailsId,
            name: this.form.value.name as string,
            description: this.form.value.description || '',
            isEmpowerment: this.form.value.isEmpowerment as boolean,
            minimumLevelOfAssurance: this.selectedAssuranceLevel,
            requiredPersonalInformation: this.form.value?.personalData?.map(item => item.value) || [],
            serviceScopeNames: this.form.value.scopes || [],
        });
    }

    searchSuggestions = ({ query }: { query: string }) => {
        if (!this.defaultScopeQuery.data || !this.availableScopeQuery.data) return;

        const enteredScopes = this.form.value.scopes || [];
        const allScopes = [...this.defaultScopeQuery.data, ...this.availableScopeQuery.data];

        const allAvailable = allScopes.filter(scope => !enteredScopes.includes(scope));

        this.suggestions = allAvailable.filter(suggestion => suggestion.toLowerCase().includes(query?.toLowerCase()));
    };
}
