import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FilesJournalComponent } from './pages/files-journal/files-journal.component';
import { LogsSearchComponent } from './pages/logs-search/logs-search.component';
import { LogFileSearchResultsComponent } from './pages/log-file-search-results/log-file-search-results.component';
import { FileDetailsComponent } from './pages/file-details/file-details.component';
import { RedirectToJournalGuard } from './guards/redirect-to-journal.guard';
import { RedirectToSearchPageGuard } from './guards/redirect-to-search-page.guard';
import { IntegrityComponent } from './pages/integrity/integrity.component';

const routes: Routes = [
    {
        path: 'journals',
        component: FilesJournalComponent,
    },
    {
        path: 'journals/file-details',
        component: FileDetailsComponent,
        canActivate: [RedirectToJournalGuard],
    },
    {
        path: 'journals/search-files',
        component: LogsSearchComponent,
    },
    {
        path: 'journals/log-file-results',
        component: LogFileSearchResultsComponent,
        canActivate: [RedirectToSearchPageGuard],
    },
    {
        path: 'integrity',
        component: IntegrityComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class NgxEidLogsViewerRoutingModule {}
