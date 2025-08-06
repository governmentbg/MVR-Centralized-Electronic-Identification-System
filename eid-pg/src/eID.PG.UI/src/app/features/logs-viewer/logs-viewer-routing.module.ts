import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from 'src/app/core/guards/auth.guard';
import { LogsViewerMainComponent } from './pages/logs-viewer-main/logs-viewer-main.component';
import { LogsViewerFromMeComponent } from './pages/logs-viewer-from-me/logs-viewer-from-me.component';
import { LogsViewerToMeComponent } from './pages/logs-viewer-to-me/logs-viewer-to-me.component';
import { blockNavigationGuard } from 'src/app/core/guards/block-navigation.guard';

const routes: Routes = [
    {
        path: '',
        component: LogsViewerMainComponent,
        canActivate: [authGuard],
    },

    {
        path: 'from-me',
        component: LogsViewerFromMeComponent,
        canDeactivate: [blockNavigationGuard],
    },
    {
        path: 'to-me',
        component: LogsViewerToMeComponent,
        canDeactivate: [blockNavigationGuard],
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class LogsViewerRoutingModule {}
