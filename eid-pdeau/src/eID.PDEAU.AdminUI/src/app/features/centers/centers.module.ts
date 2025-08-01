import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
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
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { RegisterFormPreviewComponent } from './components/register-form-preview/register-form-preview.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ChipModule } from 'primeng/chip';
import { MultiSelectModule } from 'primeng/multiselect';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ApplicationPreviewComponent } from './pages/application-preview/application-preview.component';
import { TabViewModule } from 'primeng/tabview';
import { CenterApplicationsComponent } from '@app/features/centers/pages/center-applications/center-applications.component';
import { RegisterCenterComponent } from '@app/features/centers/pages/register-center/register-center.component';
import { ApplicationsFilterComponent } from '@app/features/centers/pages/center-applications/components/applications-filter/applications-filter.component';
import { ApplicationStatusComponent } from '@app/features/centers/components/application-status/application-status.component';
import { AdministrativeBodyFormComponent } from '@app/features/centers/components/administrative-body-form/administrative-body-form.component';
import { EmployeesListComponent } from '@app/features/centers/components/employees-list/employees-list.component';
import { OfficesListComponent } from '@app/features/centers/components/offices-list/offices-list.component';
import { LegalRepresentativesListComponent } from '@app/features/centers/components/legal-representatives-list/legal-representatives-list.component';
import { CentersRoutingModule } from '@app/features/centers/centers-routing.module';
import { RegionService } from '@app/shared/services/region.service';
import { FilesTableComponent } from '@app/features/centers/pages/application-preview/components/files-table/files-table.component';
import { CenterApplicationActionDialogComponent } from '@app/features/centers/pages/application-preview/components/administrator-application-action-dialog/center-application-action-dialog.component';
import { CircumstancePreviewComponent } from '@app/features/centers/pages/circumstance-preview/circumstance-preview.component';
import { CenterCircumstanceActionDialogComponent } from '@app/features/centers/pages/circumstance-preview/components/center-circumstance-action-dialog/center-circumstance-action-dialog.component';
import { CenterPreviewComponent } from './pages/center-preview/center-preview.component';
import { CenterRegistrationsComponent } from './pages/center-registrations/center-registrations.component';
import { TagModule } from 'primeng/tag';
import { CenterStatusActionDialogComponent } from './pages/center-preview/components/center-status-action-dialog/center-status-action-dialog.component';

@NgModule({
    declarations: [
        CenterApplicationsComponent,
        RegisterCenterComponent,
        LegalRepresentativesListComponent,
        OfficesListComponent,
        EmployeesListComponent,
        AdministrativeBodyFormComponent,
        RegisterFormPreviewComponent,
        ApplicationsFilterComponent,
        ApplicationStatusComponent,
        ApplicationPreviewComponent,
        FilesTableComponent,
        CenterApplicationActionDialogComponent,
        CircumstancePreviewComponent,
        CenterCircumstanceActionDialogComponent,
        CenterPreviewComponent,
        CenterRegistrationsComponent,
        CenterStatusActionDialogComponent,
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
        CentersRoutingModule,
        InputTextareaModule,
        ConfirmDialogModule,
        ChipModule,
        TabViewModule,
        MultiSelectModule,
        InputSwitchModule,
        TagModule,
    ],
})
export class CentersModule {
    constructor(private regionService: RegionService) {
        this.regionService.loadRegions().subscribe();
    }
}
