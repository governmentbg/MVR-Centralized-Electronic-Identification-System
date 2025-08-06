import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LogsViewerMainComponent } from './pages/logs-viewer-main/logs-viewer-main.component';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { ButtonModule } from 'primeng/button';
import { LogsViewerRoutingModule } from './logs-viewer-routing.module';
import { TranslocoModule } from '@ngneat/transloco';
import { LogsViewerFromMeComponent } from './pages/logs-viewer-from-me/logs-viewer-from-me.component';
import { LogsViewerToMeComponent } from './pages/logs-viewer-to-me/logs-viewer-to-me.component';
import { SharedModule } from '../../shared/shared.module';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { ToastModule } from 'primeng/toast';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { LogsFilterComponent } from './components/logs-filter/logs-filter.component';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ChipModule } from 'primeng/chip';
import { DigitOnlyModule } from '@uiowa/digit-only';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule } from 'primeng/paginator';
import { ReactiveFormsModule } from '@angular/forms';
import { MultiSelectModule } from 'primeng/multiselect';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@NgModule({
    declarations: [LogsViewerMainComponent, LogsViewerFromMeComponent, LogsViewerToMeComponent, LogsFilterComponent],
    imports: [
        CommonModule,
        CardModule,
        DividerModule,
        ButtonModule,
        LogsViewerRoutingModule,
        TranslocoModule,
        SharedModule,
        TranslocoLocaleModule,
        TableModule,
        SkeletonModule,
        ToastModule,
        VirtualScrollerModule,
        InfiniteScrollModule,
        CalendarModule,
        CheckboxModule,
        ChipModule,
        DigitOnlyModule,
        DropdownModule,
        InputTextModule,
        PaginatorModule,
        ReactiveFormsModule,
        MultiSelectModule,
        ConfirmDialogModule,
    ],
})
export class LogsViewerModule {}
