import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';
import { AuthGuard } from './core/guards/auth.guard';
import { ModuleLayoutComponent } from './features/empowerment-check/pages/module-layout/module-layout.component';
import { PageUnauthorizedComponent } from './shared/components/page-unauthorized/page-unauthorized.component';

const routes: Routes = [
    {
        path: '',
        component: ModuleLayoutComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'empowerment-check/search',
            },
            {
                path: 'empowerment-check',
                loadChildren: () =>
                    import('./features/empowerment-check/empowerment-check.module').then(m => m.EmpowermentCheckModule),
            },
        ],
    },
    {
        path: 'unauthorized',
        component: PageUnauthorizedComponent,
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
