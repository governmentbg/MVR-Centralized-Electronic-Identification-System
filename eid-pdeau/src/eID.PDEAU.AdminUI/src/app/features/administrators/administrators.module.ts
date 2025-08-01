import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdministratorApplicationsComponent } from '@app/features/administrators/pages/administrator-applications/administrator-applications.component';
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
import { TranslocoModule } from '@ngneat/transloco';
import { LegalRepresentativesListComponent } from '@app/features/administrators/components/legal-representatives-list/legal-representatives-list.component';
import { DevicesListComponent } from '@app/features/administrators/components/devices-list/devices-list.component';
import { EmployeesListComponent } from '@app/features/administrators/components/employees-list/employees-list.component';
import { OfficesListComponent } from '@app/features/administrators/components/offices-list/offices-list.component';
import { AdministrativeBodyFormComponent } from '@app/features/administrators/components/administrative-body-form/administrative-body-form.component';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { AdministratorsRoutingModule } from '@app/features/administrators/administrators-routing.module';
import { UserFormComponent } from '@app/features/general-information/components/user-form/user-form.component';
import { RegisterFormPreviewComponent } from './components/register-form-preview/register-form-preview.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ApplicationsFilterComponent } from './pages/administrator-applications/components/applications-filter/applications-filter.component';
import { ChipModule } from 'primeng/chip';
import { MultiSelectModule } from 'primeng/multiselect';
import { ApplicationStatusComponent } from '@app/features/administrators/components/application-status/application-status.component';
import { InputSwitchModule } from 'primeng/inputswitch';
import { RegionService } from '@app/shared/services/region.service';
import { ApplicationPreviewComponent } from './pages/application-preview/application-preview.component';
import { TabViewModule } from 'primeng/tabview';
import { FilesTableComponent } from '@app/features/administrators/pages/application-preview/components/files-table/files-table.component';
import { AdministratorApplicationActionDialogComponent } from './pages/application-preview/components/administrator-application-action-dialog/administrator-application-action-dialog.component';
import { CircumstancePreviewComponent } from './pages/circumstance-preview/circumstance-preview.component';
import { AdministratorCircumstanceActionDialogComponent } from '@app/features/administrators/pages/circumstance-preview/components/administrator-circumstance-action-dialog/administrator-circumstance-action-dialog.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { AdministratorRegistrationsComponent } from './pages/administrator-registrations/administrator-registrations.component';
import { AdministratorPreviewComponent } from './pages/administrator-preview/administrator-preview.component';
import { TagModule } from 'primeng/tag';
import { AdministratorStatusActionDialogComponent } from './pages/administrator-preview/components/administrator-status-action-dialog/administrator-status-action-dialog.component';

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
        UserFormComponent,
        RegisterFormPreviewComponent,
        ApplicationsFilterComponent,
        ApplicationStatusComponent,
        ApplicationPreviewComponent,
        FilesTableComponent,
        AdministratorApplicationActionDialogComponent,
        CircumstancePreviewComponent,
        AdministratorCircumstanceActionDialogComponent,
        AdministratorRegistrationsComponent,
        AdministratorPreviewComponent,
        AdministratorStatusActionDialogComponent,
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
        StepsModule,
        TableModule,
        TranslocoLocaleModule,
        TranslocoModule,
        SharedModule,
        StepsModule,
        DialogModule,
        DropdownModule,
        AdministratorsRoutingModule,
        InputTextareaModule,
        ConfirmDialogModule,
        ChipModule,
        MultiSelectModule,
        TabViewModule,
        InputSwitchModule,
        InputNumberModule,
        TagModule,
    ],
})
export class AdministratorsModule {
    constructor(private regionService: RegionService) {
        this.regionService.loadRegions().subscribe();
    }
}
