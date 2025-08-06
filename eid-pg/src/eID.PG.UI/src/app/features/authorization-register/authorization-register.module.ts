import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthorizationRegisterComponent } from './pages/authorization-register/authorization-register.component';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { ButtonModule } from 'primeng/button';
import { AuthorizationRegisterRoutingModule } from './authorization-register-routing.module';
import { TranslocoModule } from '@ngneat/transloco';
import { NewAuthorizationFormComponent } from './pages/new-authorization-form/new-authorization-form.component';
import { AuthorizationFromMeComponent } from './pages/authorization-from-me/authorization-from-me.component';
import { AuthorizationToMeComponent } from './pages/authorization-to-me/authorization-to-me.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { EmpowermentsFilterComponent } from './components/empowerments-filter/empowerments-filter.component';
import { SidebarModule } from 'primeng/sidebar';
import { ChipsModule } from 'primeng/chips';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ChipModule } from 'primeng/chip';
import { DigitOnlyModule } from '@uiowa/digit-only';
import { CheckboxModule } from 'primeng/checkbox';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { EmpowermentPreviewComponent } from './components/empowerement-preview/empowerment-preview.component';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { DocumentSigningFormComponent } from './components/document-signing-form/document-signing-form.component';
import { AuthorizationForLegalEntityComponent } from './pages/authorization-for-legal-entity/authorization-for-legal-entity.component';
import { ConfirmationService } from 'primeng/api';
@NgModule({
    declarations: [
        AuthorizationRegisterComponent,
        NewAuthorizationFormComponent,
        AuthorizationFromMeComponent,
        AuthorizationToMeComponent,
        EmpowermentsFilterComponent,
        EmpowermentPreviewComponent,
        DocumentSigningFormComponent,
        AuthorizationForLegalEntityComponent,
    ],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        CardModule,
        DividerModule,
        ButtonModule,
        AuthorizationRegisterRoutingModule,
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
    ],
    providers: [ConfirmationService],
})
export class AuthorizationRegisterModule {}
