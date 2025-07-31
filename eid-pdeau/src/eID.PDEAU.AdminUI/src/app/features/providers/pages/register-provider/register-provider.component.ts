import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '@app/core/services/user.service';
import { IdentifierType } from '@app/shared/enums';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { BulstatValidator, EGNAdultValidator, EGNValidator, PinValidator } from '@app/shared/validators/uid-validator';
import { TranslocoService } from '@ngneat/transloco';
import { format } from 'date-fns';
import { Subscription } from 'rxjs';
import { ProviderService } from '../../provider.service';
import {
    AdministrativeBodyType,
    IssuerType,
    ProviderSubjectType,
    ProviderType,
    UserType,
    providerSubject,
} from '../../provider.dto';
import { Router } from '@angular/router';
import { ToastService } from '@app/shared/services/toast.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { cyrillicValidator, twoCyrillicNamesWithUppercaseValidator } from '@app/shared/validators/cyrillic-validator';
import { createInputSanitizer } from '@app/shared/utils/create-name-sanitizer';
import { cyrillicAddressValidator } from '@app/shared/validators/cyrillic-address-validator';
import { RoleType } from '@app/core/enums/auth.enum';
import { mobilePhoneValidator } from '@app/shared/validators/mobile-phone-validator';

@Component({
    selector: 'app-register-provider',
    templateUrl: './register-provider.component.html',
    styleUrls: ['./register-provider.component.scss'],
})
export class RegisterProviderComponent implements OnInit, OnDestroy {
    private subscriptions = new Subscription();
    constructor(
        private userService: UserService,
        private translateService: TranslocoService,
        private providerService: ProviderService,
        private router: Router,
        private toastService: ToastService
    ) {}

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.providers.applications.txtTitle'),
                        onClick: () => this.router.navigate(['/providers/']),
                    },
                    {
                        label: this.translateService.translate('modules.providers.register.title'),
                    },
                ];
                this.buildProviderSubjectTypes();
                this.identifierTypes = [
                    {
                        name: this.translateService.translate('enums.identifierType.EGN'),
                        id: IdentifierType.EGN,
                    },
                    {
                        name: this.translateService.translate('enums.identifierType.LNCh'),
                        id: IdentifierType.LNCh,
                    },
                ];
            })
        );
        this.subscriptions.add(
            this.form.controls.issuer.controls.uidType.valueChanges.subscribe(value => {
                const validators = [Validators.required];
                if (value === IdentifierType.EGN) {
                    validators.push(EGNValidator(), EGNAdultValidator());
                } else if (value === IdentifierType.LNCh) {
                    validators.push(PinValidator());
                }
                const control = this.form.controls.issuer.controls.uid;
                control.setValidators(validators);
                control.updateValueAndValidity();
            })
        );

        this.subscriptions.add(
            this.providerSubjectForm.controls.selectedProviderSubject.valueChanges.subscribe(value => {
                this.onSubjectTypeChange(value);
            })
        );

        this.providerSubjectForm.controls.selectedProviderSubject.setValue(this.providerSubjectTypes[0].value);
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    breadcrumbItems: IBreadCrumbItems[] = [];
    identifierTypes: { name: string; id: string }[] = [];

    form = new FormGroup({
        providerType: new FormControl<ProviderType>(
            { value: ProviderType.Administration, disabled: true },
            { nonNullable: true }
        ),
        externalNumber: new FormControl('', [Validators.required, Validators.pattern(/^[a-zA-Z0-9-]+$/)]),
        issuer: new FormGroup({
            uidType: new FormControl(IdentifierType.EGN, [Validators.required]),
            uid: new FormControl('', [Validators.required, EGNAdultValidator(), EGNValidator()]),
            name: new FormControl('', [
                Validators.required,
                twoCyrillicNamesWithUppercaseValidator(),
                Validators.maxLength(200),
            ]),
        }),
        administrativeBody: new FormGroup({
            name: new FormControl('', {
                validators: [Validators.required],
                nonNullable: true,
            }),
            bulstat: new FormControl(
                {
                    value: '',
                    disabled: true,
                },
                [Validators.required, BulstatValidator()]
            ),
            identificationNumber: new FormControl(
                {
                    value: '',
                    disabled: true,
                },
                [Validators.required]
            ),
            headquarters: new FormControl('', [Validators.required, cyrillicAddressValidator()]),
            address: new FormControl('', [Validators.required, cyrillicAddressValidator()]),
            email: new FormControl('', [Validators.required, Validators.email]),
            phone: new FormControl('', {
                validators: [Validators.required],
                nonNullable: true,
            }),
        }),
        administrator: new FormGroup({
            uidType: new FormControl(IdentifierType.EGN, [Validators.required]),
            uid: new FormControl('', [Validators.required, EGNAdultValidator(), EGNValidator()]),
            name: new FormControl('', [Validators.required, cyrillicValidator(), Validators.maxLength(200)]),
            email: new FormControl('', [Validators.required, Validators.email]),
            phone: new FormControl('', [Validators.required, mobilePhoneValidator()]),
        }),
        files: new FormControl<File[] | null>(null, [Validators.required]),
    });
    providerSubjectForm = new FormGroup({
        selectedProviderSubject: new FormControl(providerSubject.enum.Administration, { nonNullable: true }),
    });
    registerMutation = new RequestHandler({
        requestFunction: this.providerService.registerProvider,
        onSuccess: res => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.providers.register.toasts.successfulRegistration')
            );
            this.router.navigate(['/providers']);
        },
        onError: errorResponse => {
            // Default error message
            let errorMessage = this.translateService.translate('modules.providers.register.toasts.failedRegistration');
            switch (errorResponse.status) {
                case 403:
                    errorMessage = this.translateService.translate(
                        'modules.providers.register.toasts.failedRegistryCheck'
                    );
                    break;
                case 409: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;
                    if (firstKey === 'Uid') {
                        errorMessage = this.translateService.translate('modules.providers.register.toasts.Uid');
                    }
                }
            }

            this.toastService.showErrorToast(this.translateService.translate('global.txtErrorTitle'), errorMessage);
        },
    });
    providerSubjectTypes: { name: string; value: ProviderSubjectType }[] = [];

    onSubjectTypeChange(event: ProviderSubjectType) {
        this.form.controls.administrativeBody.reset();
        this.form.controls.providerType.patchValue(event);
        if (event === providerSubject.enum.Administration) {
            this.form.controls.administrativeBody.addControl(
                'identificationNumber',
                new FormControl(
                    {
                        value: '',
                        disabled: true,
                    },
                    [Validators.required]
                )
            );
            this.form.controls.administrativeBody.controls.bulstat.disable();
        }

        if (event === providerSubject.enum.PrivateLawSubject) {
            (this.form.controls.administrativeBody as FormGroup).removeControl('identificationNumber');
            this.form.controls.administrativeBody.controls.bulstat.enable();
        }
    }

    currentDate = format(new Date(), 'dd.MM.yyyy');

    get issuerPersonalIdentifier(): string {
        return this.translateService.translate('modules.providers.register.' + this.userService.user.uidType);
    }

    onSubmit() {
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        this.registerMutation.execute({
            issuer: this.form.controls.issuer.value as IssuerType,
            providerType: this.form.controls.providerType.value as ProviderType,
            administrativeBody: this.form.controls.administrativeBody.getRawValue() as AdministrativeBodyType,
            administrator: this.form.controls.administrator.value as UserType,
            files: this.form.controls.files.getRawValue() as File[],
            externalNumber: this.form.controls.externalNumber.value as string,
        });
    }

    onFileChange({ currentFiles }: { currentFiles: File[] }) {
        if (currentFiles.length) {
            this.form.controls.files.setValue(currentFiles);
        }
    }

    onFileClear() {
        this.form.controls.files.setValue(null);
    }

    onFileRemove(fileName: string) {
        const files = this.form.controls.files.value;
        if (!files?.length) return;
        const index = files?.findIndex(file => file.name === fileName);
        files.splice(index, 1);
        this.form.controls.files.setValue(files);
    }

    buildProviderSubjectTypes() {
        this.providerSubjectTypes = [];

        if (this.userService.hasRole(RoleType.PRIVATE_LEGAL_ENTITY_ADMINISTRATOR)) {
            this.providerSubjectTypes = [
                {
                    name: this.translateService.translate('modules.providers.register.privateLawSubject'),
                    value: providerSubject.enum.PrivateLawSubject,
                },
            ];
        }

        if (this.userService.hasRole(RoleType.APP_ADMINISTRATOR)) {
            this.providerSubjectTypes = [
                {
                    name: this.translateService.translate('modules.providers.register.administrativeBody'),
                    value: providerSubject.enum.Administration,
                },
                {
                    name: this.translateService.translate('modules.providers.register.privateLawSubject'),
                    value: providerSubject.enum.PrivateLawSubject,
                },
            ];
        }
    }

    applicantNameSanitizer = createInputSanitizer(this.form.controls.issuer.controls.name);
}
