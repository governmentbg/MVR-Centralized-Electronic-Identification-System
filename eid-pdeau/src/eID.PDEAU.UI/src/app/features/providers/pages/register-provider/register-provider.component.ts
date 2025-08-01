import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '@app/core/services/user.service';
import { IdentifierType } from '@app/shared/enums';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { BulstatValidator, EGNAdultValidator, EGNValidator } from '@app/shared/validators/uid-validator';
import { TranslocoService } from '@ngneat/transloco';
import { format } from 'date-fns';
import { Subscription } from 'rxjs';
import { ProviderService } from '../../provider.service';
import { AdministrativeBodyType, providerSubject, ProviderSubjectType, UserType } from '../../provider.dto';
import { Router } from '@angular/router';
import { ToastService } from '@app/shared/services/toast.service';
import { IBreadCrumbItems } from '@app/shared/components/breadcrumb/breadcrumb.component';
import { cyrillicValidator } from '@app/shared/validators/cyrillic-validator';
import { mobilePhoneValidator } from '@app/shared/validators/mobile-phone-validator';
import { cyrillicAddressValidator } from '@app/shared/validators/cyrillic-address-validator';
import { maxFileSizeValidator } from '@app/shared/validators/max-file-size-validator';

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
                this.form.controls.applicant.controls.uidType.setValue(this.applicantPersonalIdentifier);
                this.providerSubjectTypes = [
                    {
                        name: this.translateService.translate('modules.providers.register.administrativeBody'),
                        value: providerSubject.Enum.Administration,
                    },
                    {
                        name: this.translateService.translate('modules.providers.register.privateLawSubject'),
                        value: providerSubject.Enum.PrivateLawSubject,
                    },
                ];
                this.breadcrumbItems = [
                    {
                        label: this.translateService.translate('modules.providers.applications.title'),
                        onClick: () => this.router.navigate(['/providers']),
                    },
                    {
                        label: this.translateService.translate('modules.providers.register.title'),
                    },
                ];
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    @ViewChild('helpSection') helpSection: ElementRef | undefined;

    providerSubjectTypeEnum = providerSubject;

    breadcrumbItems: IBreadCrumbItems[] = [];

    form = new FormGroup({
        applicant: new FormGroup({
            uidType: new FormControl({ value: this.applicantPersonalIdentifier, disabled: true }),
            uid: new FormControl({ value: this.userService.getUser().uid, disabled: true }),
            name: new FormControl({ value: this.userService.getUser().name, disabled: true }),
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
            headquarters: new FormControl(
                {
                    value: '',
                    disabled: true,
                },
                [Validators.required, cyrillicAddressValidator()]
            ),
            address: new FormControl(
                {
                    value: '',
                    disabled: true,
                },
                [Validators.required, cyrillicAddressValidator()]
            ),
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

        files: new FormControl<File[] | null>(null, [Validators.required, maxFileSizeValidator(10)]),
    });
    registerMutation = new RequestHandler({
        requestFunction: this.providerService.registerProvider,
        onSuccess: () => {
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
                case 400: {
                    const firstKey = errorResponse.errors ? Object.keys(errorResponse.errors)[0] : undefined;

                    if (firstKey === 'files[0]') {
                        const fileNameRegex = errorResponse.errors[firstKey]?.[0]?.match(
                            /(?<=File\s)(.*?)(?=\sis not signed\.)/
                        );
                        errorMessage = this.translateService.translate(
                            'modules.providers.register.toasts.fileNotSigned',
                            { fileName: fileNameRegex[0] }
                        );

                        break;
                    }
                    break;
                }
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
    selectedProviderSubject: ProviderSubjectType = providerSubject.Enum.Administration;

    enableAdministrativeBody = () => {
        this.form.controls.administrativeBody.enable();
        this.form.controls.administrativeBody.controls.bulstat.disable();
        this.form.controls.administrativeBody.controls.identificationNumber.disable();
    };

    onSubjectTypeChange(event: ProviderSubjectType) {
        this.selectedProviderSubject = event;
        this.form.controls.administrativeBody.reset();
        if (event === providerSubject.Enum.Administration) {
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
            this.form.controls.administrativeBody.controls.address.disable();
            this.form.controls.administrativeBody.controls.headquarters.disable();
        }

        if (event === providerSubject.Enum.PrivateLawSubject) {
            (this.form.controls.administrativeBody as FormGroup).removeControl('identificationNumber');
            this.form.controls.administrativeBody.controls.bulstat.enable();
            this.form.controls.administrativeBody.controls.address.enable();
            this.form.controls.administrativeBody.controls.headquarters.enable();
        }
    }

    currentDate = format(new Date(), 'dd.MM.yyyy');

    get applicantPersonalIdentifier(): string {
        return this.translateService.translate('modules.providers.register.' + this.userService.user.uidType);
    }

    onSubmit() {
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) return;

        const payload = {
            providerType: this.selectedProviderSubject,
            administrator: this.form.controls.administrator.value as UserType,
            files: this.form.controls.files.getRawValue() as File[],
            administrativeBody: this.form.controls.administrativeBody.getRawValue() as AdministrativeBodyType,
        };

        this.registerMutation.execute(payload);
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

    scrollToHelpSection() {
        this.helpSection?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }

    goBack() {
        this.router.navigate(['/providers']);
    }
}
