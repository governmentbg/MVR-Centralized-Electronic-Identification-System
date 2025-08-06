import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { ButtonModule } from 'primeng/button';
import { TranslocoModule } from '@ngneat/transloco';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { SidebarModule } from 'primeng/sidebar';
import { ChipsModule } from 'primeng/chips';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ChipModule } from 'primeng/chip';
import { DigitOnlyModule } from '@uiowa/digit-only';
import { CheckboxModule } from 'primeng/checkbox';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { EidManagementComponent } from './pages/eid-management/eid-management.component';
import { ApplicationsComponent } from './pages/applications/applications.component';
import { CertificatesComponent } from './pages/certificates/certificates.component';
import { NewApplicationFormComponent } from './pages/new-application-form/new-application-form.component';
import { EidManagementRoutingModule } from './eid-management-routing.module';
import { StepsComponent } from './components/steps/steps.component';
import { ApplicationsFilterComponent } from './components/applications-filter/applications-filter.component';
import { ApplicationPreviewComponent } from './components/application-preview/application-preview.component';
import { CertificatesFilterComponent } from './components/certificates-filter/certificates-filter.component';
import { CertificatesPreviewComponent } from './components/certificates-preview/certificates-preview.component';
import { CertificatesHistoryComponent } from './components/certificates-history/certificates-history.component';
import { NomenclatureService } from './services/nomenclature.service';
import { InplaceModule } from 'primeng/inplace';
@NgModule({
    declarations: [
        EidManagementComponent,
        ApplicationsComponent,
        CertificatesComponent,
        NewApplicationFormComponent,
        StepsComponent,
        ApplicationsFilterComponent,
        ApplicationPreviewComponent,
        CertificatesFilterComponent,
        CertificatesPreviewComponent,
        CertificatesHistoryComponent,
    ],
    imports: [
        CommonModule,
        EidManagementRoutingModule,
        ReactiveFormsModule,
        CardModule,
        DividerModule,
        ButtonModule,
        TranslocoModule,
        SharedModule,
        DropdownModule,
        InputTextModule,
        InputNumberModule,
        CalendarModule,
        AutoCompleteModule,
        MultiSelectModule,
        ConfirmDialogModule,
        TranslocoLocaleModule,
        SidebarModule,
        ChipsModule,
        RadioButtonModule,
        ChipModule,
        DigitOnlyModule,
        CheckboxModule,
        TableModule,
        SkeletonModule,
        DialogModule,
        ToastModule,
        TooltipModule,
        InplaceModule,
    ],
    providers: [NomenclatureService],
})
export class EidManagementModule {}
