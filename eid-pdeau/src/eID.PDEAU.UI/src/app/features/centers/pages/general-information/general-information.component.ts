import { Component, OnInit } from '@angular/core';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { ConfirmationService } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PersonForm } from '@app/features/administrators/administrators.dto';
import { IdentifierType } from '@app/shared/enums';
import { EGNAdultValidator, EGNValidator, PinValidator } from '@app/shared/validators/uid-validator';
import { cyrillicValidator } from '@app/shared/validators/cyrillic-validator';
import { Subscription } from 'rxjs';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { createInputSanitizer } from '@app/shared/utils/create-input-sanitized';
import { CentersService } from '@app/features/centers/centers.service';
import { ManagerStatusAlertService } from '@app/shared/services/manager-status-alert.service';
import { ApplicationType } from '@app/features/administrators/enums/administrators.enum';
import { ManagerStatus } from '@app/features/centers/enums/centers.enum';
import { ToastService } from '@app/shared/services/toast.service';

@Component({
    selector: 'app-general-information',
    templateUrl: './general-information.component.html',
    styleUrls: ['./general-information.component.scss'],
    providers: [ConfirmationService],
})
export class GeneralInformationComponent implements OnInit {
    constructor(
        private centersService: CentersService,
        private confirmationService: ConfirmationService,
        private translateService: TranslocoService,
        private managerStatusAlertService: ManagerStatusAlertService,
        private toastService: ToastService,
    ) {}
    information: any = {};
    latinNamePattern = /^(?=.)[a-zA-Z\s']*$/;
    userForm = new FormGroup<PersonForm>({
        citizenIdentifierType: new FormControl(IdentifierType.EGN, {
            validators: [Validators.required],
            nonNullable: true,
        }),
        citizenIdentifierNumber: new FormControl('', {
            validators: [Validators.required, EGNAdultValidator(), EGNValidator()],
            nonNullable: true,
        }),
        name: new FormControl('', {
            validators: [Validators.required, Validators.maxLength(200), cyrillicValidator()],
            nonNullable: true,
        }),
        nameLatin: new FormControl('', {
            validators: [Validators.required, Validators.maxLength(200), Validators.pattern(this.latinNamePattern)],
            nonNullable: true,
        }),
        email: new FormControl('', { validators: [Validators.required, Validators.email], nonNullable: true }),
        phoneNumber: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
    });
    identifierTypes: { name: string; id: string }[] = [];
    private subscriptions = new Subscription();
    showDialog = false;
    action: 'Edit' | 'Add' = 'Add';
    authorizedPersonId: string | null = null;
    sanitizeNameInput = createInputSanitizer(this.userForm.controls.name);
    EidManagerStatus = ManagerStatus;
    notes: any = [];

    fetchCenterQuery = new RequestHandler({
        requestFunction: this.centersService.fetchCenter,
        onSuccess: data => {
            this.information = data;
        },
    });
    addRepresentativeQuery = new RequestHandler({
        requestFunction: this.centersService.createAuthorizedPerson,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.fetchCenterQuery.execute();
            this.onClosePreview();
        },
    });
    editRepresentativeQuery = new RequestHandler({
        requestFunction: this.centersService.updateAuthorizedPersonById,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.fetchCenterQuery.execute();
            this.onClosePreview();
        },
    });
    deleteRepresentativeQuery = new RequestHandler({
        requestFunction: this.centersService.deleteAuthorizedPersonById,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.fetchCenterQuery.execute();
            this.onClosePreview();
        },
    });
    fetchDocumentsAndNotes = new RequestHandler({
        requestFunction: this.centersService.fetchAttachments,
        onSuccess: response => {
            this.notes = response.notes;
        },
    });

    ngOnInit() {
        this.fetchCenterQuery.execute();
        this.fetchDocumentsAndNotes.execute();
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
        this.userForm.controls.citizenIdentifierType.valueChanges.subscribe(value => {
            const validators = [Validators.required];
            if (value === IdentifierType.EGN) {
                validators.push(EGNValidator(), EGNAdultValidator());
            } else if (value === IdentifierType.LNCh) {
                validators.push(PinValidator());
            }
            const control = this.userForm.controls.citizenIdentifierNumber;
            control.setValidators(validators);
            control.updateValueAndValidity();
        });
    }

    onSubmit() {
        this.userForm.markAllAsTouched();

        markFormGroupAsDirty(this.userForm);
        if (this.userForm.invalid) return;

        const fb = new FormGroup<PersonForm>({
            email: new FormControl(this.userForm.controls.email.value, { nonNullable: true }),
            name: new FormControl(this.userForm.controls.name.value, { nonNullable: true }),
            phoneNumber: new FormControl(this.userForm.controls.phoneNumber.value, { nonNullable: true }),
            citizenIdentifierNumber: new FormControl(this.userForm.controls.citizenIdentifierNumber.value, {
                nonNullable: true,
            }),
            citizenIdentifierType: new FormControl(this.userForm.controls.citizenIdentifierType.value, {
                nonNullable: true,
            }),
            nameLatin: new FormControl(this.userForm.controls.nameLatin.value, { nonNullable: true }),
            isActive: new FormControl(null),
        });

        if (this.action === 'Edit') {
            this.editRepresentativeQuery.execute(fb.value, this.authorizedPersonId as string);
        } else {
            this.addRepresentativeQuery.execute(fb.value);
        }
        this.onClosePreview();
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
                this.deleteRepresentativeQuery.execute(device.id);
            },
        });
    }

    add() {
        this.action = 'Add';
        this.showDialog = true;
    }

    edit(user: any) {
        this.authorizedPersonId = user.id;
        this.userForm.patchValue({
            name: user.name,
            nameLatin: user.nameLatin,
            phoneNumber: user.phoneNumber,
            email: user.email,
            citizenIdentifierNumber: user.citizenIdentifierNumber,
            citizenIdentifierType: user.citizenIdentifierType,
        });
        this.action = 'Edit';
        this.showDialog = true;
    }

    onClosePreview() {
        this.userForm.reset();
        this.showDialog = false;
    }

    protected readonly ApplicationType = ApplicationType;
}
