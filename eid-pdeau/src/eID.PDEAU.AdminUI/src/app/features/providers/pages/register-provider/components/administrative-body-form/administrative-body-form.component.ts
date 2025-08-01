import { Component, Input, OnChanges } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { providerSubject, ProviderSubjectType } from '@app/features/providers/provider.dto';
import { ProviderService } from '@app/features/providers/provider.service';
import { RequestHandler } from '@app/shared/utils/request-handler';

@Component({
    selector: 'app-administrative-body-form',
    templateUrl: './administrative-body-form.component.html',
    styleUrls: ['./administrative-body-form.component.scss'],
})
export class AdministrativeBodyFormComponent implements OnChanges {
    @Input() form!: FormGroup<{
        name: FormControl<string>;
        bulstat: FormControl<string | null>;
        identificationNumber: FormControl<string | null>;
        headquarters: FormControl<string | null>;
        address: FormControl<string | null>;
        email: FormControl<string | null>;
        phone: FormControl<string>;
    }>;

    @Input() subjectType: ProviderSubjectType = providerSubject.enum.Administration;

    constructor(private providerService: ProviderService) {}

    ngOnChanges() {
        if (this.subjectType === providerSubject.enum.Administration) {
            this.providersDetailsQuery.execute();
        } else {
            this.form.controls.name.enable();
        }
    }

    subjectTypesEnum = providerSubject;

    providerDetailsOptions: { name: string; id: string }[] = [];
    providersDetailsQuery = new RequestHandler({
        requestFunction: this.providerService.fetchProvidersDetails,
        onInit: () => {
            this.form.controls.name.disable();
        },
        onSuccess: data => {
            this.providerDetailsOptions = data.map(providerDetails => ({
                name: providerDetails.name,
                id: providerDetails.id,
            }));
            this.form.controls.name.enable();
        },
    });

    onServiceSelect({ value }: { value: string }) {
        if (!value) {
            this.form.controls.bulstat.setValue('');
            this.form.controls.identificationNumber.setValue('');
            return;
        }
        const selectedProviderDetails = this.providersDetailsQuery.data?.find(provider => provider.name === value);
        if (!selectedProviderDetails) return;

        this.form.controls.bulstat.setValue(selectedProviderDetails.uic);
        this.form.controls.identificationNumber.setValue(selectedProviderDetails.identificationNumber);
    }
}
