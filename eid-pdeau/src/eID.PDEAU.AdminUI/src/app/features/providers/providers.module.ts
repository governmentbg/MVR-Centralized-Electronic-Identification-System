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
import { SkeletonModule } from 'primeng/skeleton';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { CardModule } from 'primeng/card';
import { RegisterProviderComponent } from './pages/register-provider/register-provider.component';
import { AdministrativeBodyFormComponent } from './pages/register-provider/components/administrative-body-form/administrative-body-form.component';
import { AdministratorFormComponent } from './pages/register-provider/components/administrator-form/administrator-form.component';
import { AisFormComponent } from './pages/register-provider/components/ais-form/ais-form.component';
import { FileUploadModule } from 'primeng/fileupload';
import { TabViewModule } from 'primeng/tabview';
import { ApplicationStatusComponent } from './components/application-status/application-status.component';
import { ProviderPreviewComponent } from './pages/provider-preview/provider-preview.component';
import { UsersTableComponent } from './pages/provider-preview/users-table/users-table.component';
import { FilesTableComponent } from './pages/provider-preview/files-table/files-table.component';
import { ProviderDetailsComponent } from './pages/provider-preview/provider-details/provider-details.component';
import { ProvidersComponent } from './pages/providers/providers.component';
import { ProvidersFilterComponent } from './pages/providers/components/applications-filter/providers-filter.component';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { ProviderConfirmDialogComponent } from './pages/provider-preview/provider-confirm-dialog/provider-confirm-dialog.component';
import { ProviderDenyDialogComponent } from './pages/provider-preview/provider-deny-dialog/provider-deny-dialog.component';
import { ProviderReturnDialogComponent } from './pages/provider-preview/provider-return-dialog/provider-return-dialog.component';
import { TooltipModule } from 'primeng/tooltip';
import { StatusHistoryComponent } from './pages/provider-preview/status-history/status-history.component';
import { ServicesFilterComponent } from '@app/features/providers/pages/provider-services/components/services-filter/services-filter.component';
import { ServicesDenyDialogComponent } from '@app/features/providers/pages/provider-services/components/services-deny-dialog/services-deny-dialog.component';
import { ServicesConfirmDialogComponent } from '@app/features/providers/pages/provider-services/components/services-confirm-dialog/services-confirm-dialog.component';
import { ServicePreviewComponent } from '@app/features/providers/pages/provider-services/components/services-preview/services-preview.component';
import { CheckboxModule } from 'primeng/checkbox';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ServiceStatusComponent } from '@app/features/providers/components/service-status/service-status.component';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CalendarModule } from 'primeng/calendar';
import { StatisticsTableComponent } from './pages/provider-preview/statistics-table/statistics-table.component';
@NgModule({
    declarations: [
        ProvidersComponent,
        ApplicationStatusComponent,
        ProvidersFilterComponent,
        ProviderPreviewComponent,
        RegisterProviderComponent,
        AdministrativeBodyFormComponent,
        AdministratorFormComponent,
        AisFormComponent,
        UsersTableComponent,
        FilesTableComponent,
        ProviderDetailsComponent,
        ProviderConfirmDialogComponent,
        ProviderDenyDialogComponent,
        ProviderReturnDialogComponent,
        StatusHistoryComponent,
        ServicesFilterComponent,
        ServicesDenyDialogComponent,
        ServicesConfirmDialogComponent,
        ServicePreviewComponent,
        ServicesFilterComponent,
        ServicesDenyDialogComponent,
        ServicesConfirmDialogComponent,
        ServicePreviewComponent,
        ServiceStatusComponent,
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
        FileUploadModule,
        TabViewModule,
        ConfirmDialogModule,
        DialogModule,
        TooltipModule,
        CheckboxModule,
        InputSwitchModule,
        InputTextareaModule,
        CalendarModule,
    ],
    providers: [ConfirmationService],
    exports: [RouterModule, StatusHistoryComponent],
})
export class ProvidersModule {}
