import { Component, OnDestroy, OnInit } from '@angular/core';
import { RoleType } from '@app/core/enums/auth.enum';
import { TranslocoService } from '@ngneat/transloco';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { RequestHandler } from '@app/shared/utils/request-handler';
import { markFormGroupAsDirty } from '@app/shared/utils/forms';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { EmployeeForm } from '@app/features/administrators/administrators.dto';
import { IdentifierType } from '@app/shared/enums';
import { EGNAdultValidator, EGNValidator } from '@app/shared/validators/uid-validator';
import { cyrillicValidator } from '@app/shared/validators/cyrillic-validator';
import { createInputSanitizer } from '@app/shared/utils/create-input-sanitized';
import { ConfirmationService, LazyLoadEvent } from 'primeng/api';
import { Subscription } from 'rxjs';
import { ManagerStatusAlertService } from '@app/shared/services/manager-status-alert.service';

@Component({
    selector: 'app-employees',
    templateUrl: './employees.component.html',
    styleUrls: ['./employees.component.scss'],
    providers: [ConfirmationService],
})
export class EmployeesComponent implements OnInit, OnDestroy {
    constructor(
        private translateService: TranslocoService,
        private administratorsService: AdministratorsService,
        private confirmationService: ConfirmationService,
        private managerStatusAlertService: ManagerStatusAlertService
    ) {}
    showDialog = false;
    latinNamePattern = /^(?=.)[a-zA-Z\s']*$/;
    employeeForm = new FormGroup({
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
        isAdministrator: new FormControl(false),
        phoneNumber: new FormControl('', {
            validators: [Validators.required],
            nonNullable: true,
        }),
        roles: new FormControl<string[]>([], { validators: [Validators.required], nonNullable: true }),
    });
    identifierTypes: { name: string; id: string }[] = [];
    roles: { name: string; value: string }[] = [];
    private subscriptions = new Subscription();
    employeeId: null | string = null;
    lazyLoadEvent: LazyLoadEvent = {};
    filters: any = {};
    action: 'Edit' | 'Add' = 'Add';

    employeeListQuery = new RequestHandler({
        requestFunction: this.administratorsService.fetchEmployees,
    });
    addEmployeeQuery = new RequestHandler({
        requestFunction: this.administratorsService.createEmployee,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.loadEmployees();
            this.onClosePreview();
        },
    });
    editEmployeeQuery = new RequestHandler({
        requestFunction: this.administratorsService.updateEmployeeById,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.loadEmployees();
            this.onClosePreview();
        },
    });
    deleteEmployeeQuery = new RequestHandler({
        requestFunction: this.administratorsService.deleteEmployeeById,
        onSuccess: res => {
            this.managerStatusAlertService.checkForPendingAttachments();
            this.loadEmployees();
            this.onClosePreview();
        },
    });

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

                this.roles = [
                    {
                        name: this.translateService.translate('enums.administratorsAndCentersRoles.administrator'),
                        value: 'ADMINISTRATOR',
                    },
                    {
                        name: this.translateService.translate('enums.administratorsAndCentersRoles.user'),
                        value: 'USER',
                    },
                ];
            })
        );
    }

    ngOnDestroy() {
        this.subscriptions.unsubscribe();
    }

    onLazyLoad(event: LazyLoadEvent) {
        this.lazyLoadEvent = event;
        this.loadEmployees();
    }

    loadEmployees() {
        const sortOrder = this.lazyLoadEvent.sortOrder === 1 ? 'asc' : 'desc';
        let payload = {
            page: 0,
            size: this.lazyLoadEvent.rows as number,
            sort: this.lazyLoadEvent.sortField ? `${this.lazyLoadEvent.sortField},${sortOrder}` : '',
        };

        if (this.filters !== null && this.filters !== undefined) {
            payload = { ...payload, ...this.filters };
        } else if (this.filters !== null && Object.keys(this.filters).length > 0) {
            payload = { ...payload, ...this.filters };
        }

        if (this.lazyLoadEvent && this.lazyLoadEvent.first && this.lazyLoadEvent.rows) {
            payload.page = this.lazyLoadEvent.first / this.lazyLoadEvent.rows;
        }

        this.employeeListQuery.execute({
            ...payload,
        });
    }

    onSubmit() {
        this.employeeForm.markAllAsTouched();

        markFormGroupAsDirty(this.employeeForm);
        if (this.employeeForm.invalid) return;

        const fb = new FormGroup<EmployeeForm>({
            email: new FormControl(this.employeeForm.controls.email.value, { nonNullable: true }),
            name: new FormControl(this.employeeForm.controls.name.value, { nonNullable: true }),
            phoneNumber: new FormControl(this.employeeForm.controls.phoneNumber.value, { nonNullable: true }),
            citizenIdentifierNumber: new FormControl(this.employeeForm.controls.citizenIdentifierNumber.value, {
                nonNullable: true,
            }),
            citizenIdentifierType: new FormControl(this.employeeForm.controls.citizenIdentifierType.value, {
                nonNullable: true,
            }),
            nameLatin: new FormControl(this.employeeForm.controls.nameLatin.value, { nonNullable: true }),
            roles: new FormControl(this.employeeForm.controls.roles.value, { nonNullable: true }),
            isActive: new FormControl(null),
        });

        if (this.action === 'Edit') {
            this.editEmployeeQuery.execute(fb.value, this.employeeId as string);
        } else {
            this.addEmployeeQuery.execute(fb.value);
        }
        this.employeeForm.reset();
    }

    remove(device: any) {
        this.confirmationService.confirm({
            header: this.translateService.translate('global.txtConfirmation'),
            message: this.translateService.translate(
                'modules.administrators.administrator-preview.deleteEmployeeConfirmation'
            ),
            rejectButtonStyleClass: 'p-button-danger',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.deleteEmployeeQuery.execute(device.id);
            },
        });
    }

    add() {
        this.action = 'Add';
        this.showDialog = true;
    }

    edit(employee: any) {
        this.employeeId = employee.id;
        this.employeeForm.patchValue({
            name: employee.name,
            nameLatin: employee.nameLatin,
            email: employee.email,
            roles: employee.roles,
            citizenIdentifierNumber: employee.citizenIdentifierNumber,
            citizenIdentifierType: employee.citizenIdentifierType,
            phoneNumber: employee.phoneNumber,
            isAdministrator: employee.isAdministrator,
        });
        this.action = 'Edit';
        this.showDialog = true;
    }

    onClosePreview() {
        this.employeeForm.reset();
        this.employeeId = null;
        this.showDialog = false;
    }

    roleNames(values: string[]) {
        return this.roles.filter(role => values.includes(role.value)).map(role => role.name);
    }

    sanitizeNameInput = createInputSanitizer(this.employeeForm.controls.name);

    protected readonly RoleType = RoleType;
}
