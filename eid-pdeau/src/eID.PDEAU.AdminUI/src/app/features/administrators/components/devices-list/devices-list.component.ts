import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@ngneat/transloco';
import { IdentifierType } from '@app/shared/enums';
import { Subscription } from 'rxjs';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { DeviceForm } from '@app/features/administrators/administrators.dto';
import { DeviceType } from '@app/features/administrators/enums/administrators.enum';

@Component({
    selector: 'app-devices-list',
    templateUrl: './devices-list.component.html',
    styleUrls: ['./devices-list.component.scss'],
})
export class DevicesListComponent implements OnInit {
    constructor(private translateService: TranslocoService) {}

    @Input() form!: FormArray<FormGroup<DeviceForm>>;

    showDialog = false;
    deviceForm = new FormGroup({
        name: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        type: new FormControl(DeviceType.OTHER, { validators: [Validators.required], nonNullable: true }),
        authorizationLink: new FormControl('', { validators: [Validators.required] }),
        backchannelAuthorizationLink: new FormControl(''),
        description: new FormControl('', { validators: [Validators.required], nonNullable: true }),
    });
    identifierTypes: { name: string; id: string }[] = [];
    private subscriptions = new Subscription();
    checked = false;

    ngOnInit() {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
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
    }

    onSubmit() {
        this.deviceForm.markAllAsTouched();

        markFormGroupAsDirty(this.deviceForm);
        if (this.deviceForm.invalid) return;

        const fb = new FormGroup<DeviceForm>({
            name: new FormControl(this.deviceForm.controls.name.value, { nonNullable: true }),
            type: new FormControl(this.deviceForm.controls.type.value, { nonNullable: true }),
            authorizationLink: new FormControl(this.deviceForm.controls.authorizationLink.value),
            backchannelAuthorizationLink: new FormControl(this.deviceForm.controls.backchannelAuthorizationLink.value),
            description: new FormControl(this.deviceForm.controls.description.value, { nonNullable: true }),
        });

        this.form.push(fb);
        this.deviceForm.reset();
        this.onClosePreview();
    }

    remove(index: number) {
        this.form.removeAt(index);
    }

    onClosePreview() {
        this.showDialog = false;
    }

    onAuthorizationChange(event: any) {
        if (event.checked) {
            this.deviceForm.controls.backchannelAuthorizationLink.setValue('');
            this.deviceForm.controls.authorizationLink.setValue('');

            this.deviceForm.controls.backchannelAuthorizationLink.setValidators([Validators.required]);
            this.deviceForm.controls.authorizationLink.setValidators([]);

            this.deviceForm.controls.backchannelAuthorizationLink.updateValueAndValidity();
            this.deviceForm.controls.authorizationLink.updateValueAndValidity();
        } else {
            this.deviceForm.controls.authorizationLink.setValue('');
            this.deviceForm.controls.backchannelAuthorizationLink.setValue('');

            this.deviceForm.controls.backchannelAuthorizationLink.setValidators([]);
            this.deviceForm.controls.authorizationLink.setValidators([Validators.required]);

            this.deviceForm.controls.backchannelAuthorizationLink.updateValueAndValidity();
            this.deviceForm.controls.authorizationLink.updateValueAndValidity();
        }
    }
}
