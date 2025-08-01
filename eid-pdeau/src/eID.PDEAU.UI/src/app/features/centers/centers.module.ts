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
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { ProvidersModule } from '@app/features/providers/providers.module';
import { RegisterFormPreviewComponent } from './components/register-form-preview/register-form-preview.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ChipModule } from 'primeng/chip';
import { MultiSelectModule } from 'primeng/multiselect';
import { ChipsModule } from 'primeng/chips';
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
import { ConfirmationService } from 'primeng/api';
import { EmployeesComponent } from '@app/features/centers/pages/employees/employees.component';
import { OfficesComponent } from '@app/features/centers/pages/offices/offices.component';
import { GeneralInformationComponent } from '@app/features/centers/pages/general-information/general-information.component';
import { AdministratorsService } from '@app/features/administrators/administrators.service';
import { UserService } from '@app/core/services/user.service';
import { AlertService } from '@app/shared/services/alert.service';
import { NavigationEnd, Router } from '@angular/router';
import { SystemType } from '@app/core/enums/auth.enum';
import { ManagerStatus } from '@app/features/administrators/enums/administrators.enum';
import { CentersService } from '@app/features/centers/centers.service';
import { DocumentsComponent } from './pages/documents/documents.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { TagModule } from 'primeng/tag';
import { ManagerStatusAlertService } from '@app/shared/services/manager-status-alert.service';
import {
    CenterStatusActionDialogComponent
} from '@app/features/centers/components/center-status-action-dialog/center-status-action-dialog.component';

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
        EmployeesComponent,
        OfficesComponent,
        GeneralInformationComponent,
        DocumentsComponent,
        CenterStatusActionDialogComponent
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
export class CentersModule {
    constructor(
        private regionService: RegionService,
        private userService: UserService,
        private router: Router,
        private managerStatusAlertService: ManagerStatusAlertService
    ) {
        this.regionService.loadRegions().subscribe();

        if (this.userService.getSystemType() === SystemType.CEI) {
            this.router.events.subscribe(event => {
                if (event instanceof NavigationEnd) {
                    this.managerStatusAlertService.checkForPendingAttachments();
                }
            });
        }
    }
}
