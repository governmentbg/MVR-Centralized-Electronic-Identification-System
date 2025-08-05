import { NgModule } from '@angular/core';
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
import { ProviderDataComponent } from 'src/app/features/administrators/components/provider-data/provider-data.component';
import { MapComponent } from 'src/app/features/administrators/map/map.component';
import { AdministratorsComponent } from 'src/app/features/administrators/administrators.component';
import { HomeComponent } from './home.component';
import { HomeRoutingModule } from './home-routing.module';
import { ProvidersComponent } from 'src/app/features/administrators/components/providers/providers.component';

@NgModule({
    declarations: [HomeComponent, AdministratorsComponent, MapComponent, ProviderDataComponent, ProvidersComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        CardModule,
        DividerModule,
        ButtonModule,
        TranslocoModule,
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
        SharedModule,
        HomeRoutingModule,
        DividerModule
    ],
})
export class HomeModule {}
