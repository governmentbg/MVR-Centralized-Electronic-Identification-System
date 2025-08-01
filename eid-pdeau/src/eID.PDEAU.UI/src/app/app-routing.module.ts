import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';
import { PageUnauthorizedComponent } from './shared/components/page-unauthorized/page-unauthorized.component';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { AuthGuard } from '@app/core/guards/auth.guard';
import { RoleType, SystemType } from '@app/core/enums/auth.enum';
import { UserService } from '@app/core/services/user.service';
import { redirectGuard } from '@app/core/guards/redirect.guard';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        children: [
            {
                path: '',
                pathMatch: 'full',
                children: [],
                canActivate: [redirectGuard],
            },
            {
                path: 'login',
                loadChildren: () => import('./features/login/login.module').then(m => m.LoginModule),
            },
            {
                path: 'providers',
                canActivate: [AuthGuard],
                loadChildren: () => import('./features/providers/providers.module').then(m => m.ProvidersModule),
            },
            {
                path: 'services',
                loadChildren: () => import('./features/services/services.module').then(m => m.ServicesModule),
                canActivate: [AuthGuard],
                data: { roles: [RoleType.ADMINISTRATOR] },
            },
            {
                path: 'users',
                loadChildren: () => import('./features/users/users.module').then(m => m.UsersModule),
                canActivate: [AuthGuard],
                data: { roles: [RoleType.ADMINISTRATOR] },
            },
            {
                path: 'empowerment-check',
                loadChildren: () =>
                    import('./features/empowerment-check/empowerment-check.module').then(m => m.EmpowermentCheckModule),
                canActivate: [AuthGuard],
                data: { roles: [RoleType.EMPLOYEE] },
            },
            {
                path: 'general-information',
                loadChildren: () =>
                    import('./features/general-information/general-information.module').then(
                        m => m.GeneralInformationModule
                    ),
                canActivate: [AuthGuard],
                data: { roles: [RoleType.EMPLOYEE] },
            },
            {
                path: 'administrators',
                canActivate: [AuthGuard],
                loadChildren: () =>
                    import('./features/administrators/administrators.module').then(m => m.AdministratorsModule),
            },
            {
                path: 'centers',
                canActivate: [AuthGuard],
                loadChildren: () => import('./features/centers/centers.module').then(m => m.CentersModule),
            },
            {
                path: 'tariffs',
                canActivate: [AuthGuard],
                loadChildren: () => import('./features/tariffs/tariffs.module').then(m => m.TariffsModule),
                data: { roles: [RoleType.ADMINISTRATOR], requiredSystemType: [SystemType.AEI] },
            },
            {
                path: 'audit-logs',
                loadChildren: () => import('./features/audit-logs/audit-logs.module').then(m => m.AuditLogsModule),
                data: { roles: [RoleType.ADMINISTRATOR] },
                canActivate: [AuthGuard],
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
export class AppRoutingModule {
    constructor(private userService: UserService) {}
}
