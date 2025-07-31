import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FilesJournalComponent } from './pages/files-journal/files-journal.component';
import { LogsSearchComponent } from './pages/logs-search/logs-search.component';
import { LogFileSearchResultsComponent } from './pages/log-file-search-results/log-file-search-results.component';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { FileDetailsComponent } from './pages/file-details/file-details.component';
import { TranslocoModule } from '@ngneat/transloco';
import { ToastModule } from 'primeng/toast';
import { AppConfigService } from './services/journal-configuration.service';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { InputTextModule } from 'primeng/inputtext';
import { DividerModule } from 'primeng/divider';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DropdownModule } from 'primeng/dropdown';
import { VirtualScrollerModule } from 'primeng/virtualscroller';
import { NgxEidLogsViewerRoutingModule } from './ngx-eid-logs-viewer-routing.module';
import { IntegrityComponent } from './pages/integrity/integrity.component';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { AccordionModule } from 'primeng/accordion';
import { JsonFormatterComponent } from './components/json-formatter/json-formatter.component';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';

@NgModule({
    declarations: [
        FilesJournalComponent,
        FileDetailsComponent,
        LogsSearchComponent,
        LogFileSearchResultsComponent,
        BreadcrumbComponent,
        IntegrityComponent,
        JsonFormatterComponent,
    ],
    imports: [
        CommonModule,
        NgxEidLogsViewerRoutingModule,
        TableModule,
        SkeletonModule,
        FormsModule,
        ReactiveFormsModule,
        ButtonModule,
        CalendarModule,
        ToastModule,
        TranslocoModule,
        TranslocoLocaleModule,
        InputTextModule,
        DividerModule,
        ProgressSpinnerModule,
        DropdownModule,
        VirtualScrollerModule,
        SelectButtonModule,
        ConfirmDialogModule,
        AccordionModule,
    ],
    exports: [FilesJournalComponent, IntegrityComponent],
    providers: [AppConfigService],
})
export class NgxEidLogsViewerModule {}
