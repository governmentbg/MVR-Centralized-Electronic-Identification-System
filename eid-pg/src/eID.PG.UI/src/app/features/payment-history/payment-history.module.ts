import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FieldsetModule } from 'primeng/fieldset';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { TranslocoModule } from '@ngneat/transloco';
import { CardModule } from 'primeng/card';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';
import { PaymentHistoryComponent } from './payment-history.component';
import { SkeletonModule } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { ReactiveFormsModule } from '@angular/forms';
import { DividerModule } from 'primeng/divider';
import { PaymentHistoryFilterComponent } from './components/payment-history-filter/payment-history-filter.component';
import { ButtonModule } from 'primeng/button';
import { ChipModule } from 'primeng/chip';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { InputTextModule } from 'primeng/inputtext';
const routes: Routes = [
    {
        path: '',
        component: PaymentHistoryComponent,
    },
];
@NgModule({
    imports: [
        RouterModule.forChild(routes),
        FieldsetModule,
        TranslocoLocaleModule,
        TranslocoModule,
        CardModule,
        ToastModule,
        TableModule,
        SkeletonModule,
        CommonModule,
        SharedModule,
        ReactiveFormsModule,
        DividerModule,
        ButtonModule,
        ChipModule,
        DropdownModule,
        CalendarModule,
        InputTextModule
    ],
    declarations: [PaymentHistoryComponent, PaymentHistoryFilterComponent],
    exports: [RouterModule],
})
export class PaymentHistoryModule {}
