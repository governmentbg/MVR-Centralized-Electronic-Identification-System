import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleType } from './core/enums/auth.enum';
import { PageUnauthorizedComponent } from './shared/components/page-unauthorized/page-unauthorized.component';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'eid-management',
            },
            {
                path: 'eid-management',
                loadChildren: () =>
                    import('./features/eid-management/eid-management.module').then(m => m.EidManagementModule),
                canActivate: [AuthGuard],
            },
            {
                path: 'logs-viewer',
                loadChildren: () => import('@eid/ngx-eid-logs-viewer').then(m => m.NgxEidLogsViewerModule),
                data: { roles: [RoleType.APP_ADMINISTRATOR] },
                canActivate: [AuthGuard],
            },
            {
                path: 'system-processes',
                loadChildren: () =>
                    import('./features/system-processes/system-processes.module').then(m => m.SystemProcessesModule),
                data: { roles: [RoleType.APP_ADMINISTRATOR] },
                canActivate: [AuthGuard],
            },
            {
                path: 'audit-logs',
                loadChildren: () => import('./features/audit-logs/audit-logs.module').then(m => m.AuditLogsModule),
                data: { roles: [RoleType.APP_ADMINISTRATOR, RoleType.AUDIT_ADMIN] },
                canActivate: [AuthGuard],
            },
            {
                path: 'reports',
                loadChildren: () => import('./features/reports/reports.module').then(m => m.ReportsModule),
                data: { roles: [RoleType.APP_ADMINISTRATOR, RoleType.RU_MVR_ADMINISTRATOR] },
                canActivate: [AuthGuard],
            },
            {
                path: 'unauthorized',
                component: PageUnauthorizedComponent,
            },
        ],
    },
    {
        path: '**',
        component: PageNotFoundComponent,
    },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
