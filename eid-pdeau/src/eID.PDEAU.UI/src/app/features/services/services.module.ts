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
import { ServicesRoutingModule } from './services-routing.module';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { CardModule } from 'primeng/card';
import { ProviderServicesComponent } from './pages/provider-services/provider-services.component';
import { ServicePreviewComponent } from './pages/provider-services/components/services-preview/services-preview.component';
import { ServicesFilterComponent } from './pages/provider-services/components/services-filter/services-filter.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { CheckboxModule } from 'primeng/checkbox';
import { FileUploadModule } from 'primeng/fileupload';
import { TooltipModule } from 'primeng/tooltip';
import { MultiSelectModule } from 'primeng/multiselect';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TabViewModule } from 'primeng/tabview';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { AddServiceComponent } from './pages/add-service/add-service.component';
import { ServiceEditComponent } from './pages/provider-services/components/service-edit/service-edit.component';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ServicesToggleDialogComponent } from '@app/features/services/pages/provider-services/components/services-toggle-dialog/services-toggle-dialog.component';
import { ServiceStatusComponent } from '@app/features/services/components/service-status/service-status.component';

@NgModule({
    declarations: [
        ProviderServicesComponent,
        ServicePreviewComponent,
        ServicesFilterComponent,
        AddServiceComponent,
        ServiceEditComponent,
        ServicesToggleDialogComponent,
        ServiceStatusComponent,
    ],
    imports: [
        ServicesRoutingModule,
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
        MultiSelectModule,
        InputTextareaModule,
        InputSwitchModule,
    ],
    providers: [ConfirmationService],
    exports: [RouterModule],
})
export class ServicesModule {}
