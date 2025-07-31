import { AfterViewChecked, Component } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ToastService } from '@app/shared/services/toast.service';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { GeneralInformationAndOfficesType, GeneralInformationType, OfficeType } from '../general-information.dto';
import { GeneralInformationService } from '../general-information.service';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';

@Component({
    selector: 'app-general-information-page',
    templateUrl: './general-information-page.component.html',
    styleUrls: ['./general-information-page.component.scss'],
})
export class GeneralInformationPageComponent implements AfterViewChecked {
    private subscriptions = new Subscription();
    constructor(
        private translateService: TranslocoService,
        private generalInformationService: GeneralInformationService,
        private fb: FormBuilder,
        private toastService: ToastService
    ) {}

    ngAfterViewChecked(): void {
        if (this.newlyAddedOfficeIndex !== null) {
            const officeFormElem = document.querySelector(`[data-office-index="${this.newlyAddedOfficeIndex}"]`);
            officeFormElem?.scrollIntoView({ behavior: 'smooth' });
            this.newlyAddedOfficeIndex = null;
        }
    }

    newlyAddedOfficeIndex: number | null = null;

    form = new FormGroup({
        generalInformation: new FormControl<GeneralInformationType>('', [
            Validators.required,
            Validators.maxLength(500),
        ]),
        offices: new FormArray<FormGroup>([], [Validators.required]),
    });

    generalInformationQuery = new RequestHandler({
        requestFunction: this.generalInformationService.getGeneralInformationAndOffices,
        onSuccess: data => {
            this.form.controls.generalInformation.setValue(data.generalInformation || '');

            if (data.offices.length) {
                data.offices.map(office => {
                    this.form.controls.offices.push(this.getOfficeGormGroup(office));
                });
                return;
            }
            this.form.controls.offices.push(this.getOfficeGormGroup());
        },
    }).execute();

    generalInformationMutation = new RequestHandler({
        requestFunction: this.generalInformationService.mutateGeneralInformationAndOffices,
        onInit: () => {
            this.form.disable();
        },
        onSuccess: () => {
            this.toastService.showSuccessToast(
                this.translateService.translate('global.txtSuccessTitle'),
                this.translateService.translate('modules.general-information.toasts.success')
            );
        },
        onError: () => {
            this.toastService.showErrorToast(
                this.translateService.translate('global.txtErrorTitle'),
                this.translateService.translate('modules.general-information.toasts.failure')
            );
        },
        onComplete: () => {
            this.form.enable();
        },
    });

    isLoading = this.generalInformationMutation.isLoading || this.generalInformationQuery.isLoading;

    get offices(): FormArray<FormGroup> {
        return this.form.get('offices') as FormArray;
    }

    getOfficeGormGroup = (office?: OfficeType) => {
        return this.fb.group({
            name: [office?.name || '', [Validators.required, Validators.maxLength(100)]],
            address: [office?.address || '', [Validators.required, Validators.maxLength(255)]],
            lat: [office?.lat || null, [Validators.required]],
            lon: [office?.lon || null, [Validators.required]],
        });
    };

    addOffice(): void {
        this.form.controls.offices.push(this.getOfficeGormGroup());
        this.newlyAddedOfficeIndex = this.offices.length - 1;
    }

    removeOffice(index: number): void {
        this.offices.removeAt(index);
    }

    onSubmit = () => {
        this.form.markAllAsTouched();

        markFormGroupAsDirty(this.form);
        if (this.form.invalid) {
            return;
        }

        const payload = this.form.getRawValue() as GeneralInformationAndOfficesType;
        this.generalInformationMutation.execute(payload);
    };
}
