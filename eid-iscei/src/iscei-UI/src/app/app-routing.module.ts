import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { PageUnauthorizedComponent } from './shared/components/page-unauthorized/page-unauthorized.component';
import { ButtonModule } from 'primeng/button';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,

        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'login',
            },
            {
                path: 'login',
                loadChildren: () => import('./features/login/login.module').then(m => m.LoginModule),
            },
            {
                path: 'administration-login',
                loadChildren: () =>
                    import('./features/administration-login/administration-login.module').then(
                        m => m.AdministrationLoginModule
                    ),
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
    imports: [RouterModule.forRoot(routes), ButtonModule],
    exports: [RouterModule],
})
export class AppRoutingModule {}
