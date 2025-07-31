import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CurrentProviderService } from '@app/core/services/current-provider.service';
import { UserService } from '@app/core/services/user.service';
import {
    minimumLevelOfAssuranceEnum,
    MinimumLevelOfAssuranceType,
    ProviderServiceType,
} from '@app/features/services/services.dto';
import { ProviderServicesService } from '@app/features/services/provider-services.service';
import { scopeDuplicateScopeValidator } from '@app/features/services/utils';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { ToastService } from '@app/shared/services/toast.service';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { Subscription } from 'rxjs';

type AssuranceLevelOption = { name: string; value: MinimumLevelOfAssuranceType };
type PersonalDataType = { name: string; value: string };

@Component({
    selector: 'app-service-edit',
    templateUrl: './service-edit.component.html',
    styleUrls: ['./service-edit.component.scss'],
})
export class ServiceEditComponent implements OnInit, OnDestroy {
    constructor(
        private translateService: TranslocoService,
        private providerServicesService: ProviderServicesService,
        private fb: FormBuilder,
        private toastService: ToastService,
        private router: Router,
        private confirmationService: ConfirmationService,
        private userService: UserService,
        private currentProviderService: CurrentProviderService
    ) {}

    @Input() service!: ProviderServiceType;
    @Output() closePreview = new EventEmitter();
    fieldChanged = false; // Flag to track if the field has changed
    private subscriptions = new Subscription();

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.services.services-table.title'),
                        onClick: () => this.onClosePreview(),
                    },
                    {
                        label: this.translateService.translate('modules.services.service-preview.title'),
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

        this.form.patchValue({
            name: this.service?.name,
            description: this.service?.description,
            isEmpowerment: this.service?.isEmpowerment,
            personalData: this.generatePersonalInformationOptions(this.service?.requiredPersonalInformation || []),
        });

        this.selectedAssuranceLevel = this.service?.minimumLevelOfAssurance;

        if (!this.service?.id) return;

        this.serviceScopeQuery.execute(this.service?.id);

        this.subscriptions.add(
            this.form.valueChanges.subscribe(_ => {
                this.fieldChanged = true;
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    form = new FormGroup({
        name: new FormControl({ value: '', disabled: true }, [Validators.required]),
        description: new FormControl(''),
        isEmpowerment: new FormControl<boolean>(false),
        scopes: new FormArray<FormControl<string>>([]),
        personalData: new FormControl<PersonalDataType[]>([]),
    });

    breadcrumbItems: IBreadCrumbItems[] = [];
    assuranceLevelOptions: AssuranceLevelOption[] = [];
    selectedAssuranceLevel: MinimumLevelOfAssuranceType = minimumLevelOfAssuranceEnum.Enum.High;
    selectedAssuranceLevelIsInvalidForEdit = false;
    suggestions: string[] = [];
    personalDataOptions: PersonalDataType[] = [];

    defaultScopeQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchProviderServicesFullScope,
        onSuccess: data => {
            this.suggestions = [...this.suggestions, ...data];
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.services.service-preview.toasts.defaultScopeFetchFail')
            );
            this.router.navigate(['/providers/services']);
        },
    }).execute();

    serviceScopeQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchServicesScope,
        onSuccess: data => {
            data.forEach(scope => {
                this.addScope(scope.name);
                this.suggestions.push(scope.name);
            });
            if (!data.length) {
                this.addScope();
            }
            // reset the flag because we manually add some fields to the form
            this.fieldChanged = false;
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.services.service-preview.toasts.serviceScopeFetchFail')
            );
            this.router.navigate(['/providers/services']);
        },
    });

    personalInformationQuery = new RequestHandler({
        requestFunction: this.providerServicesService.fetchPersonalInformationEnum,
        onSuccess: data => {
            this.personalDataOptions = this.generatePersonalInformationOptions(data);
        },
    }).execute();

    serviceMutation = new RequestHandler({
        requestFunction: this.providerServicesService.editService,
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.services.service-preview.toasts.serviceEditFail')
            );
            this.router.navigate(['/services']);
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.services.service-preview.toasts.serviceEditSuccess')
            );
            this.fieldChanged = false;
            this.onClosePreview();
        },
    });

    generatePersonalInformationOptions = (personalData: string[]) => {
        return personalData.map(item => ({
            name: this.translateService.translate(`modules.services.enums.${item}`),
            value: item,
        }));
    };

    get scopes(): FormArray {
        return this.form.get('scopes') as FormArray;
    }

    addScope(defaultValue = ''): void {
        this.scopes.push(this.fb.control(defaultValue, [Validators.required, scopeDuplicateScopeValidator()]));
    }

    removeScope(index: number): void {
        this.scopes.removeAt(index);
    }

    get isLoading() {
        return this.defaultScopeQuery.isLoading || this.serviceScopeQuery.isLoading || this.serviceMutation.isLoading;
    }

    onClosePreview() {
        this.closePreview.emit();
    }

    cancelEdit() {
        if (!this.fieldChanged) {
            this.onClosePreview();
            return;
        }
        this.confirmationService.confirm({
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate('modules.services.service-preview.backDialogTitle'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.onClosePreview();
            },
        });
    }

    confirmEdit() {
        this.confirmationService.confirm({
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate('modules.services.service-preview.editDialogTitle'),
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                if (!this.service) return;
                const providerDetailsId = this.currentProviderService.getProvider()?.detailsId;
                if (!providerDetailsId) {
                    throw new Error('Provider details id not available when adding service');
                }

                this.serviceMutation.execute(this.service.id, {
                    description: this.form.value.description!,
                    serviceScopeNames: this.form.value.scopes || [],
                    isEmpowerment: this.form.value.isEmpowerment!,
                    providerDetailsId: this.service.providerDetailsId,
                    userId: this.userService.user?.userId,
                    minimumLevelOfAssurance: this.selectedAssuranceLevel,
                    requiredPersonalInformation: this.form.value?.personalData?.map(item => item.value) || [],
                });
            },
        });
    }

    onSubmit(): void {
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (
            !(
                [
                    minimumLevelOfAssuranceEnum.enum.High,
                    minimumLevelOfAssuranceEnum.enum.Substantial,
                    minimumLevelOfAssuranceEnum.enum.Low,
                ] as MinimumLevelOfAssuranceType[]
            ).includes(this.selectedAssuranceLevel)
        ) {
            this.selectedAssuranceLevelIsInvalidForEdit = true;
            return;
        }
        if (this.form.invalid) return;
        this.confirmEdit();
    }

    searchSuggestions = ({ query }: { query: string }) => {
        if (!this.defaultScopeQuery.data) return;

        const enteredScopes = this.form.value.scopes || [];

        const allAvailable = this.defaultScopeQuery.data.filter(scope => !enteredScopes.includes(scope));

        this.suggestions = allAvailable.filter(suggestion => suggestion.toLowerCase().includes(query?.toLowerCase()));
    };
}
