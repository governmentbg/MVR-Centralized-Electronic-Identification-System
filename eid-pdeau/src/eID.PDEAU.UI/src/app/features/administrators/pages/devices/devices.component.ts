import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { DeviceForm } from '@app/features/administrators/administrators.dto';
import { ConfirmationService } from 'primeng/api';
import { ManagerStatusAlertService } from '@app/shared/services/manager-status-alert.service';
import { DeviceType } from '@app/features/administrators/enums/administrators.enum';

@Component({
    selector: 'app-devices',
    templateUrl: './devices.component.html',
    styleUrls: ['./devices.component.scss'],
    providers: [ConfirmationService],
})
export class DevicesComponent implements OnInit {
    constructor(
        private translateService: TranslocoService,
        private administratorsService: AdministratorsService,
        private confirmationService: ConfirmationService,
        private managerStatusAlertService: ManagerStatusAlertService
    ) {}
    devices: any = [];
    showDialog = false;
    deviceForm = new FormGroup({
        name: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        type: new FormControl(DeviceType.OTHER, { validators: [Validators.required], nonNullable: true }),
        authorizationLink: new FormControl('', { validators: [Validators.required] }),
        backchannelAuthorizationLink: new FormControl(''),
        description: new FormControl('', { validators: [Validators.required], nonNullable: true }),
    });
    checked = false;
    identifierTypes: { name: string; id: string }[] = [];
    action: 'Edit' | 'Add' = 'Add';
    deviceId = '';

    deviceListQuery = new RequestHandler({
        requestFunction: this.administratorsService.fetchDevices,
        onSuccess: res => {
            this.devices = res;
        },
    });
    addDeviceQuery = new RequestHandler({
        requestFunction: this.administratorsService.createDevice,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.deviceListQuery.execute();
            this.onClosePreview();
        },
    });
    editDeviceQuery = new RequestHandler({
        requestFunction: this.administratorsService.updateDeviceById,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.deviceListQuery.execute();
            this.onClosePreview();
        },
    });
    deleteDeviceQuery = new RequestHandler({
        requestFunction: this.administratorsService.deleteDeviceById,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.deviceListQuery.execute();
            this.onClosePreview();
        },
    });

    ngOnInit() {
        this.deviceListQuery.execute();
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
            isActive: new FormControl(null),
        });

        if (this.action === 'Edit') {
            this.editDeviceQuery.execute({ ...fb.value, ...{ id: this.deviceId } });
        } else {
            this.addDeviceQuery.execute(fb.value);
        }
        this.deviceForm.reset();
    }

    remove(device: any) {
        this.confirmationService.confirm({
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate(
                'modules.administrators.administrator-preview.deleteDeviceConfirmation'
            ),
            rejectButtonStyleClass: 'p-button-danger',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.deleteDeviceQuery.execute(device.id);
            },
        });
    }

    add() {
        this.action = 'Add';
        this.checked = false;
        this.showDialog = true;
    }

    edit(device: any) {
        this.checked = device.backchannelAuthorizationLink ? true : false;
        this.deviceForm.patchValue({
            name: device.name,
            type: device.type,
            authorizationLink: device.authorizationLink,
            backchannelAuthorizationLink: device.backchannelAuthorizationLink,
            description: device.description,
        });
        this.action = 'Edit';
        this.deviceId = device.id;
        this.showDialog = true;
    }

    onClosePreview() {
        this.deviceForm.reset();
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
