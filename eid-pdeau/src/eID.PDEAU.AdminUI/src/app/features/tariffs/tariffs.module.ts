import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TariffsRoutingModule } from '@app/features/tariffs/tariffs-routing.module';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { TableModule } from 'primeng/table';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { TranslocoModule } from '@ngneat/transloco';
import { SharedModule } from '@app/shared/shared.module';
import { ServicesTariffsComponent } from './pages/services-tariffs/services-tariffs.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ServicesTariffsFormComponent } from '@app/features/tariffs/components/services-tariffs-form/services-tariffs-form.component';
import { DiscountsComponent } from './pages/discounts/discounts.component';
import { DiscountFormComponent } from './components/discount-form/discount-form.component';
import { CheckboxModule } from 'primeng/checkbox';
import { CalendarModule } from 'primeng/calendar';
import { InputNumberModule } from 'primeng/inputnumber';

@NgModule({
    declarations: [ServicesTariffsComponent, ServicesTariffsFormComponent, DiscountsComponent, DiscountFormComponent],
    imports: [
        CommonModule,
        ButtonModule,
        CardModule,
        DividerModule,
        TableModule,
        TranslocoLocaleModule,
        TranslocoModule,
        SharedModule,
        TariffsRoutingModule,
        ConfirmDialogModule,
        InputTextModule,
        FormsModule,
        ReactiveFormsModule,
        DropdownModule,
        CheckboxModule,
        CalendarModule,
        InputNumberModule,
    ],
})
export class TariffsModule {}
