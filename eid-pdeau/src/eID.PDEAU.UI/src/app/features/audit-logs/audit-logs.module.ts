import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuditLogsListComponent } from './pages/audit-logs-list/audit-logs-list.component';
import { AuditLogsRoutingModule } from './audit-logs-routing.module';
import { TranslocoModule } from '@ngneat/transloco';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { DividerModule } from 'primeng/divider';
import { SharedModule } from '../../shared/shared.module';
import { LogsFilterComponent } from './components/logs-filter/logs-filter.component';
import { MultiSelectModule } from 'primeng/multiselect';
import { ReactiveFormsModule } from '@angular/forms';
import { PaginatorModule } from 'primeng/paginator';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { DigitOnlyModule } from '@uiowa/digit-only';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { ToastModule } from 'primeng/toast';
import { ChipModule } from 'primeng/chip';
import { DialogModule } from 'primeng/dialog';
import { PanelModule } from 'primeng/panel';

@NgModule({
    declarations: [AuditLogsListComponent, LogsFilterComponent],
    imports: [
        CommonModule,
        AuditLogsRoutingModule,
        CardModule,
        DividerModule,
        ButtonModule,
        TranslocoModule,
        SharedModule,
        TranslocoLocaleModule,
        TableModule,
        SkeletonModule,
        ToastModule,
        VirtualScrollerModule,
        CalendarModule,
        CheckboxModule,
        ChipModule,
        DigitOnlyModule,
        DropdownModule,
        InputTextModule,
        PaginatorModule,
        ReactiveFormsModule,
        MultiSelectModule,
        DialogModule,
        PanelModule,
    ],
})
export class AuditLogsModule {}
