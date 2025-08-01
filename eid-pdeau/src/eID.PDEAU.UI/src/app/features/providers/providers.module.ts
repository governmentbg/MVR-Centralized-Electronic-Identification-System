import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DividerModule } from 'primeng/divider';
import { TranslocoModule } from '@ngneat/transloco';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { ButtonModule } from 'primeng/button';
import { RadioButtonModule } from 'primeng/radiobutton';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { SharedModule } from '@app/shared/shared.module';
import { ChipModule } from 'primeng/chip';
import { ChipsModule } from 'primeng/chips';
import { ProvidersRoutingModule } from './providers-routing.module';
import { TableModule } from 'primeng/table';
import { RegisterProviderComponent } from './pages/register-provider/register-provider.component';
import { ProviderApplicationsComponent } from './pages/provider-applications/provider-applications.component';
import { SkeletonModule } from 'primeng/skeleton';
import { AdministrativeBodyFormComponent } from './pages/register-provider/components/administrative-body-form/administrative-body-form.component';
import { AdministratorFormComponent } from './pages/register-provider/components/administrator-form/administrator-form.component';
import { AisFormComponent } from './pages/register-provider/components/ais-form/ais-form.component';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ApplicationsFilterComponent } from './pages/provider-applications/components/applications-filter/applications-filter.component';
import { CardModule } from 'primeng/card';
import { InputNumberModule } from 'primeng/inputnumber';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { CheckboxModule } from 'primeng/checkbox';
import { FileUploadModule } from 'primeng/fileupload';
import { TooltipModule } from 'primeng/tooltip';

import { TabViewModule } from 'primeng/tabview';
import { ProviderPreviewComponent } from './pages/provider-preview/provider-preview.component';
import { ProviderDetailsComponent } from './pages/provider-preview/provider-details/provider-details.component';
import { UsersTableComponent } from './pages/provider-preview/users-table/users-table.component';
import { FilesTableComponent } from './pages/provider-preview/files-table/files-table.component';
import { ApplicationStatusComponent } from './components/application-status/application-status.component';
import { EditProviderComponent } from './pages/provider-preview/edit-provider/edit-provider.component';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { CurrentProviderPreviewComponent } from './pages/current-provider-preview/current-provider-preview.component';
import { StatusHistoryComponent } from './pages/provider-preview/status-history/status-history.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { StatisticsTableComponent } from '@app/features/providers/pages/provider-preview/statistics-table/statistics-table.component';
import { CalendarModule } from 'primeng/calendar';

@NgModule({
    declarations: [
        AdministrativeBodyFormComponent,
        AdministratorFormComponent,
        AisFormComponent,
        RegisterProviderComponent,
        ProviderApplicationsComponent,
        ApplicationStatusComponent,
        ApplicationsFilterComponent,
        ProviderPreviewComponent,
        ProviderDetailsComponent,
        UsersTableComponent,
        FilesTableComponent,
        EditProviderComponent,
        CurrentProviderPreviewComponent,
        StatusHistoryComponent,
        StatisticsTableComponent,
    ],
    imports: [
        ProvidersRoutingModule,
        TranslocoModule,
        DividerModule,
        ReactiveFormsModule,
        InputTextModule,
        ButtonModule,
        CommonModule,
        DropdownModule,
        InputMaskModule,
        RadioButtonModule,
        FormsModule,
        SharedModule,
        TableModule,
        SkeletonModule,
        TranslocoLocaleModule,
        ChipModule,
        ChipsModule,
        CardModule,
        InputNumberModule,
        AutoCompleteModule,
        ConfirmDialogModule,
        CheckboxModule,
        FileUploadModule,
        TooltipModule,
        TabViewModule,
        ToastModule,
        DialogModule,
        InputTextareaModule,
        CalendarModule,
    ],
    providers: [ConfirmationService],
    exports: [RouterModule],
})
export class ProvidersModule {}
