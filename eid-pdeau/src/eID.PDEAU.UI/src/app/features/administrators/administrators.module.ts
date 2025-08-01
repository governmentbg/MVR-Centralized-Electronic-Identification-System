import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdministratorApplicationsComponent } from './pages/administrator-applications/administrator-applications.component';
import { RegisterAdministratorComponent } from './pages/register-administrator/register-administrator.component';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { FileUploadModule } from 'primeng/fileupload';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { SharedModule } from '@app/shared/shared.module';
import { StepsModule } from 'primeng/steps';
import { TableModule } from 'primeng/table';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { LegalRepresentativesListComponent } from '@app/features/administrators/components/legal-representatives-list/legal-representatives-list.component';
import { DevicesListComponent } from '@app/features/administrators/components/devices-list/devices-list.component';
import { EmployeesListComponent } from '@app/features/administrators/components/employees-list/employees-list.component';
import { OfficesListComponent } from '@app/features/administrators/components/offices-list/offices-list.component';
import { AdministrativeBodyFormComponent } from '@app/features/administrators/components/administrative-body-form/administrative-body-form.component';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { AdministratorsRoutingModule } from '@app/features/administrators/administrators-routing.module';
import { ProvidersModule } from '@app/features/providers/providers.module';
import { RegisterFormPreviewComponent } from './components/register-form-preview/register-form-preview.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ApplicationsFilterComponent } from './pages/administrator-applications/components/applications-filter/applications-filter.component';
import { ChipModule } from 'primeng/chip';
import { MultiSelectModule } from 'primeng/multiselect';
import { ChipsModule } from 'primeng/chips';
import { ApplicationStatusComponent } from '@app/features/administrators/components/application-status/application-status.component';
import { InputSwitchModule } from 'primeng/inputswitch';
import { RegionService } from '@app/shared/services/region.service';
import { ApplicationPreviewComponent } from './pages/application-preview/application-preview.component';
import { TabViewModule } from 'primeng/tabview';
import { FilesTableComponent } from '@app/features/administrators/pages/application-preview/components/files-table/files-table.component';
import { EmployeesComponent } from './pages/employees/employees.component';
import { OfficesComponent } from './pages/offices/offices.component';
import { DevicesComponent } from './pages/devices/devices.component';
import { GeneralInformationComponent } from './pages/general-information/general-information.component';
import { ConfirmationService } from 'primeng/api';
import { DocumentsComponent } from './pages/documents/documents.component';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { UserService } from '@app/core/services/user.service';
import { ManagerStatus } from '@app/features/administrators/enums/administrators.enum';
import { AlertService } from '@app/shared/services/alert.service';
import { NavigationEnd, Router } from '@angular/router';
import { SystemType } from '@app/core/enums/auth.enum';
import { InputNumberModule } from 'primeng/inputnumber';
import { TagModule } from 'primeng/tag';
import { ManagerStatusAlertService } from '@app/shared/services/manager-status-alert.service';
import {
    AdministratorStatusActionDialogComponent
} from '@app/features/administrators/components/administrator-status-action-dialog/administrator-status-action-dialog.component';

@NgModule({
    declarations: [
        AdministratorApplicationsComponent,
        RegisterAdministratorComponent,
        LegalRepresentativesListComponent,
        DevicesListComponent,
        LegalRepresentativesListComponent,
        OfficesListComponent,
        EmployeesListComponent,
        AdministrativeBodyFormComponent,
        RegisterFormPreviewComponent,
        ApplicationsFilterComponent,
        ApplicationStatusComponent,
        ApplicationPreviewComponent,
        FilesTableComponent,
        EmployeesComponent,
        OfficesComponent,
        DevicesComponent,
        GeneralInformationComponent,
        DocumentsComponent,
        AdministratorStatusActionDialogComponent
    ],
    imports: [
        CommonModule,
        ButtonModule,
        CardModule,
        DividerModule,
        FileUploadModule,
        FormsModule,
        InputTextModule,
        ReactiveFormsModule,
        SharedModule,
        SharedModule,
        StepsModule,
        TableModule,
        TranslocoLocaleModule,
        TranslocoModule,
        SharedModule,
        StepsModule,
        DialogModule,
        DropdownModule,
        AdministratorsRoutingModule,
        ProvidersModule,
        InputTextareaModule,
        ConfirmDialogModule,
        ChipModule,
        MultiSelectModule,
        ChipsModule,
        InputSwitchModule,
        TabViewModule,
        MultiSelectModule,
        ChipsModule,
        InputSwitchModule,
        InputNumberModule,
        TagModule,
    ],
    providers: [ConfirmationService],
})
export class AdministratorsModule {
    constructor(
        private regionService: RegionService,
        private userService: UserService,
        private router: Router,
        private managerStatusAlertService: ManagerStatusAlertService
    ) {
        this.regionService.loadRegions().subscribe();

        if (this.userService.getSystemType() === SystemType.AEI) {
            this.router.events.subscribe(event => {
                if (event instanceof NavigationEnd) {
                    this.managerStatusAlertService.checkForPendingAttachments();
                }
            });
        }
    }
}
