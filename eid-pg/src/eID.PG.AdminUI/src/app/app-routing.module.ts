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
                redirectTo: 'search',
            },
            {
                path: 'search',
                loadChildren: () => import('./features/search/search.module').then(m => m.SearchModule),
                data: { roles: [RoleType.CIS_BACKOFFICE, RoleType.APP_ADMINISTRATOR] },
            },
            {
                path: 'create-content',
                loadChildren: () =>
                    import('./features/create-content/create-content.module').then(m => m.CreateContentModule),
                data: { roles: [RoleType.CIS_BACKOFFICE, RoleType.APP_ADMINISTRATOR] },
            },
            {
                path: 'edit-content',
                loadChildren: () =>
                    import('./features/edit-content/edit-content.module').then(m => m.EditContentModule),
                data: { roles: [RoleType.CIS_BACKOFFICE, RoleType.APP_ADMINISTRATOR] },
            },
            {
                path: 'system-processes',
                loadChildren: () =>
                    import('./features/system-processes/system-processes.module').then(m => m.SystemProcessesModule),
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
