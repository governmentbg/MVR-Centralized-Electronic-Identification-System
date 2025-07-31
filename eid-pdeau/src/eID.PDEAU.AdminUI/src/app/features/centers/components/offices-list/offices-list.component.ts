import { Component, Input } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { Subscription } from 'rxjs';
import { RegionService } from '@app/shared/services/region.service';
import { TranslocoService } from '@ngneat/transloco';
import { EidManagerFrontOfficeForm, Region } from '@app/features/centers/centers.dto';

@Component({
    selector: 'app-offices-list',
    templateUrl: './offices-list.component.html',
    styleUrls: ['./offices-list.component.scss'],
})
export class OfficesListComponent {
    constructor(private translateService: TranslocoService, private regionService: RegionService) {
        this.regions = this.regionService.regions.map(device => {
            return {
                value: device.code,
                name: device.descriptions[this.translateService.getActiveLang() as keyof Region['descriptions']] || '',
            };
        });
    }

    @Input() form!: FormArray<FormGroup<EidManagerFrontOfficeForm>>;

    subscriptions: Subscription = new Subscription();
    regions: { value: string; name: string }[] = [];
    showDialog = false;
    officeForm = new FormGroup({
        name: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        location: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        region: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        latitude: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        longitude: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        description: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        workingHours: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        email: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        contact: new FormControl('', { validators: [Validators.required], nonNullable: true }),
    });

    onSubmit() {
        this.officeForm.markAllAsTouched();

        markFormGroupAsDirty(this.officeForm);
        if (this.officeForm.invalid) return;

        const fb = new FormGroup<EidManagerFrontOfficeForm>({
            name: new FormControl(this.officeForm.controls.name.value, { nonNullable: true }),
            location: new FormControl(this.officeForm.controls.location.value, { nonNullable: true }),
            region: new FormControl(this.officeForm.controls.region.value, { nonNullable: true }),
            longitude: new FormControl(this.officeForm.controls.longitude.value, { nonNullable: true }),
            latitude: new FormControl(this.officeForm.controls.latitude.value, { nonNullable: true }),
            description: new FormControl(this.officeForm.controls.description.value, { nonNullable: true }),
            workingHours: new FormControl([this.officeForm.controls.workingHours.value], { nonNullable: true }),
            email: new FormControl(this.officeForm.controls.email.value, { nonNullable: true }),
            contact: new FormControl(this.officeForm.controls.contact.value, { nonNullable: true }),
        });

        this.form.push(fb);
        this.officeForm.reset();
        this.onClosePreview();
    }

    remove(index: number) {
        this.form.removeAt(index);
    }

    onClosePreview() {
        this.showDialog = false;
    }
}
