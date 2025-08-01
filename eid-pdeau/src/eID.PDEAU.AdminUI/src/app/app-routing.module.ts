import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';
import { AuthGuard } from './core/guards/auth.guard';
import { PageUnauthorizedComponent } from './shared/components/page-unauthorized/page-unauthorized.component';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { RoleType } from '@app/core/enums/auth.enum';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        canActivate: [AuthGuard],

        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'providers',
            },
            {
                path: 'providers',
                loadChildren: () => import('./features/providers/providers.module').then(m => m.ProvidersModule),
                canActivate: [AuthGuard],
                data: {
                    roles: [
                        RoleType.PRIVATE_LEGAL_ENTITY_ADMINISTRATOR,
                        RoleType.CIS_BACKOFFICE,
                        RoleType.APP_ADMINISTRATOR,
                    ],
                },
            },
            {
                path: 'administrators',
                canActivate: [AuthGuard],
                loadChildren: () =>
                    import('./features/administrators/administrators.module').then(m => m.AdministratorsModule),
                data: { roles: [RoleType.CIS_BACKOFFICE, RoleType.APP_ADMINISTRATOR] },
            },
            {
                path: 'centers',
                canActivate: [AuthGuard],
                loadChildren: () => import('./features/centers/centers.module').then(m => m.CentersModule),
                data: { roles: [RoleType.CIS_BACKOFFICE, RoleType.APP_ADMINISTRATOR] },
            },
            {
                path: 'system-processes',
                canActivate: [AuthGuard],
                loadChildren: () =>
                    import('./features/system-processes/system-processes.module').then(m => m.SystemProcessesModule),
                data: { roles: [RoleType.APP_ADMINISTRATOR] },
            },
            {
                path: 'logs-viewer',
                canActivate: [AuthGuard],
                loadChildren: () => import('@eid/ngx-eid-logs-viewer').then(m => m.NgxEidLogsViewerModule),
                data: { roles: [RoleType.APP_ADMINISTRATOR] },
            },
            {
                path: 'tariffs',
                canActivate: [AuthGuard],
                loadChildren: () => import('./features/tariffs/tariffs.module').then(m => m.TariffsModule),
                data: { roles: [RoleType.APP_ADMINISTRATOR] },
            },
            {
                path: 'document-types',
                canActivate: [AuthGuard],
                loadChildren: () =>
                    import('./features/document-types/document-types.module').then(m => m.DocumentTypesModule),
                data: { roles: [RoleType.APP_ADMINISTRATOR] },
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
