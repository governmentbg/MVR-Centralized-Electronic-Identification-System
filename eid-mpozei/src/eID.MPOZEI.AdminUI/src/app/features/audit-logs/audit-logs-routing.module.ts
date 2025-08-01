import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { AuditLogsListComponent } from './pages/audit-logs-list/audit-logs-list.component';
import { blockNavigationGuard } from 'src/app/core/guards/block-navigation.guard';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'preview',
    },
    {
        path: 'preview',
        component: AuditLogsListComponent,
        canActivate: [AuthGuard],
        canDeactivate: [blockNavigationGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AuditLogsRoutingModule {}
