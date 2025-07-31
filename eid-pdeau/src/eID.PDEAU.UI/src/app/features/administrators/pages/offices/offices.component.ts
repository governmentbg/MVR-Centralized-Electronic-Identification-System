import { Component, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { EidManagerFrontOfficeForm, Region } from '@app/features/administrators/administrators.dto';
import { RegionService } from '@app/shared/services/region.service';
import { ConfirmationService } from 'primeng/api';
import { ManagerStatusAlertService } from '@app/shared/services/manager-status-alert.service';

@Component({
    selector: 'app-offices',
    templateUrl: './offices.component.html',
    styleUrls: ['./offices.component.scss'],
    providers: [ConfirmationService],
})
export class OfficesComponent implements OnInit {
    constructor(
        private translateService: TranslocoService,
        private regionService: RegionService,
        private administratorsService: AdministratorsService,
        private confirmationService: ConfirmationService,
        private managerStatusAlertService: ManagerStatusAlertService
    ) {}
    offices: any = [];
    showDialog = false;
    officeForm = new FormGroup({
        name: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        location: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        region: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        latitude: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        longitude: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        description: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        workingHours: new FormControl<string>('', { validators: [Validators.required], nonNullable: true }),
        email: new FormControl('', { validators: [Validators.required], nonNullable: true }),
        contact: new FormControl('', { validators: [Validators.required], nonNullable: true }),
    });
    action: 'Edit' | 'Add' = 'Add';
    officeId = '';

    officeListQuery = new RequestHandler({
        requestFunction: this.administratorsService.fetchOffices,
        onSuccess: res => {
            this.offices = res;
        },
    });
    addOfficeQuery = new RequestHandler({
        requestFunction: this.administratorsService.createOffice,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.officeListQuery.execute();
            this.onClosePreview();
        },
    });
    editOfficeQuery = new RequestHandler({
        requestFunction: this.administratorsService.updateOfficeById,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.officeListQuery.execute();
            this.onClosePreview();
        },
    });
    deleteOfficeQuery = new RequestHandler({
        requestFunction: this.administratorsService.deleteOfficeById,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.officeListQuery.execute();
            this.onClosePreview();
        },
    });

    get regions() {
        return this.regionService.regions.map(device => {
            return {
                value: device.code,
                name: device.descriptions[this.translateService.getActiveLang() as keyof Region['descriptions']] || '',
            };
        });
    }

    ngOnInit() {
        this.officeListQuery.execute();
    }

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
            isActive: new FormControl(null),
        });

        if (this.action === 'Edit') {
            this.editOfficeQuery.execute({ ...fb.value, ...{ id: this.officeId } });
        } else {
            this.addOfficeQuery.execute(fb.value);
        }
        this.officeForm.reset();
    }

    remove(device: any) {
        this.confirmationService.confirm({
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate(
                'modules.administrators.administrator-preview.deleteOfficeConfirmation'
            ),
            rejectButtonStyleClass: 'p-button-danger',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.deleteOfficeQuery.execute(device.id);
            },
        });
    }

    add() {
        this.action = 'Add';
        this.showDialog = true;
    }

    edit(office: any) {
        this.officeForm.patchValue({
            name: office.name,
            contact: office.contact,
            description: office.description,
            email: office.email,
            latitude: office.latitude,
            location: office.location,
            longitude: office.longitude,
            region: office.region,
            workingHours: office.workingHours.join(),
        });
        this.action = 'Edit';
        this.officeId = office.id;
        this.showDialog = true;
    }

    onClosePreview() {
        this.officeForm.reset();
        this.showDialog = false;
    }
}
