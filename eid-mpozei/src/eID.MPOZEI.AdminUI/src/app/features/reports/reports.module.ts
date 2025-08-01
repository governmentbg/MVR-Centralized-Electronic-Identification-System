import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OperatorsReportComponent } from './pages/operators-report/operators-report.component';
import { SharedModule } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { TranslocoModule } from '@ngneat/transloco';
import { ReportsRoutingModule } from './reports-routing.module';
import { CalendarModule } from 'primeng/calendar';
import { ReactiveFormsModule } from '@angular/forms';
import { ChipsModule } from 'primeng/chips';
import { ReportsFilterComponent } from './components/reports-filter/reports-filter.component';
import { ChipModule } from 'primeng/chip';
import { DigitOnlyModule } from '@uiowa/digit-only';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';

@NgModule({
    declarations: [OperatorsReportComponent, ReportsFilterComponent],
    imports: [
        CommonModule,
        ReportsRoutingModule,
        SharedModule,
        TableModule,
        TranslocoModule,
        CalendarModule,
        ReactiveFormsModule,
        ChipsModule,
        ChipModule,
        DigitOnlyModule,
        DropdownModule,
        MultiSelectModule,
    ],
})
export class ReportsModule {}
