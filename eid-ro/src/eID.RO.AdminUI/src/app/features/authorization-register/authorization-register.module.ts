import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthorizationsListComponent } from './pages/authorizations-list/authorizations-list.component';
import { DividerModule } from 'primeng/divider';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { TooltipModule } from 'primeng/tooltip';
import { EmpowermentsFilterComponent } from './components/empowerments-filter/empowerments-filter.component';
import { EmpowermentPreviewComponent } from './components/empowerments-preview/empowerment-preview.component';
import { ButtonModule } from 'primeng/button';
import { ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { ChipModule } from 'primeng/chip';
import { CheckboxModule } from 'primeng/checkbox';
import { SharedModule } from '../../shared/shared.module';
import { AuthorizationRegisterRoutingModule } from './authorization-register-routing.module';
import { EmpowermentCertificatePrintableComponent } from './components/empowerment-certificate-printable/empowerment-certificate-printable.component';
import { DigitOnlyModule } from '@uiowa/digit-only';

@NgModule({
    declarations: [
        AuthorizationsListComponent,
        EmpowermentsFilterComponent,
        EmpowermentPreviewComponent,
        EmpowermentCertificatePrintableComponent,
    ],
    imports: [
        CommonModule,
        AuthorizationRegisterRoutingModule,
        DividerModule,
        CardModule,
        TableModule,
        SkeletonModule,
        TranslocoLocaleModule,
        TooltipModule,
        ButtonModule,
        ReactiveFormsModule,
        InputTextModule,
        DropdownModule,
        CalendarModule,
        ChipModule,
        CheckboxModule,
        SharedModule,
        DigitOnlyModule,
    ],
})
export class AuthorizationRegisterModule {}
