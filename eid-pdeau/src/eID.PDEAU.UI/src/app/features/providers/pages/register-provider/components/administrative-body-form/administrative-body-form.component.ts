import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { providerSubject, ProviderSubjectType } from '@app/features/providers/provider.dto';
import { ProviderService } from '@app/features/providers/provider.service';
import { RequestHandler } from '@app/shared/utils/request-handler';

@Component({
    selector: 'app-administrative-body-form',
    templateUrl: './administrative-body-form.component.html',
    styleUrls: ['./administrative-body-form.component.scss'],
})
export class AdministrativeBodyFormComponent implements OnInit, OnChanges {
    @Input() form!: FormGroup<{
        name: FormControl<string>;
        bulstat: FormControl<string | null>;
        identificationNumber: FormControl<string | null>;
        headquarters: FormControl<string | null>;
        address: FormControl<string | null>;
        email: FormControl<string | null>;
        phone: FormControl<string>;
    }>;

    @Input() mode: 'create' | 'edit' = 'create';
    @Input() subjectType: ProviderSubjectType = providerSubject.Enum.Administration;

    constructor(private providerService: ProviderService) {}

    ngOnInit(): void {
        if (this.mode === 'create' && this.subjectType === providerSubject.Enum.Administration) {
            this.providersDetailsQuery.execute();
        }
        if (this.mode === 'edit') {
            this.form.disable();
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['subjectType'].currentValue === providerSubject.Enum.PrivateLawSubject) {
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
            this.form.controls.headquarters.setValue('');
            this.form.controls.address.setValue('');
            return;
        }
        const selectedProviderDetails = this.providersDetailsQuery.data?.find(provider => provider.name === value);
        if (!selectedProviderDetails) return;

        this.form.controls.bulstat.setValue(selectedProviderDetails.uic);
        this.form.controls.identificationNumber.setValue(selectedProviderDetails.identificationNumber);
        this.form.controls.headquarters.setValue(selectedProviderDetails.headquarters);
        this.form.controls.address.setValue(selectedProviderDetails.address);
    }
}
