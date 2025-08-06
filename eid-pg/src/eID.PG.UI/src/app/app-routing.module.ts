import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { authGuard } from './core/guards/auth.guard';
import { PageUnauthorizedComponent } from './shared/components/page-unauthorized/page-unauthorized.component';
import { ButtonModule } from 'primeng/button';
import { LayoutWebviewComponent } from './shared/components/layout-webview/layout-webview.component';
import { mobileRedirectGuard } from './core/guards/mobile-redirect.guard';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,

        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'home',
            },
            {
                path: 'login',
                loadChildren: () => import('./features/login/login.module').then(m => m.LoginModule),
            },
            {
                path: 'home',
                canMatch: [mobileRedirectGuard],
                loadChildren: () => import('./pages/home/home.module').then(m => m.HomeModule),
            },
            {
                path: 'authorization-register',
                canActivate: [authGuard],
                loadChildren: () =>
                    import('./features/authorization-register/authorization-register.module').then(
                        m => m.AuthorizationRegisterModule
                    ),
            },
            {
                path: 'eid-management',
                canActivate: [authGuard],
                loadChildren: () =>
                    import('./features/eid-management/eid-management.module').then(m => m.EidManagementModule),
            },
            {
                path: 'eid-references',
                loadChildren: () =>
                    import('./features/eid-references/eid-references.module').then(m => m.EidReferencesModule),
            },
            {
                path: 'profile',
                canActivate: [authGuard],
                loadChildren: () => import('./features/profile/profile.module').then(m => m.ProfileModule),
            },
            {
                path: 'register',
                loadChildren: () => import('./features/register/register.module').then(m => m.RegisterModule),
            },
            {
                path: 'forgotten-password',
                loadChildren: () =>
                    import('./features/forgotten-password/forgotten-password.module').then(
                        m => m.ForgottenPasswordModule
                    ),
            },
            {
                path: 'profile/reset-password',
                loadChildren: () =>
                    import(
                        './features/landing-pages/forgotten-password-landing-page/forgotten-password-landing-page.module'
                    ).then(m => m.ForgottenPasswordLandingPageModule),
            },
            {
                path: 'registration/confirm-profile',
                loadChildren: () =>
                    import('./features/landing-pages/registration-landing-page/registration-landing-page.module').then(
                        m => m.RegistrationLandingPageModule
                    ),
            },
            {
                path: 'profile/change-email',
                loadChildren: () =>
                    import('./features/landing-pages/update-email-landing-page/update-email-landing-page.module').then(
                        m => m.UpdateEmailLandingPageModule
                    ),
            },
            {
                path: 'notification-settings',
                loadChildren: () =>
                    import('./features/notification-settings/notification-settings.module').then(
                        m => m.NotificationSettingsModule
                    ),
            },
            {
                path: 'payment-history',
                loadChildren: () =>
                    import('./features/payment-history/payment-history.module').then(m => m.PaymentHistoryModule),
            },
            {
                path: 'contacts',
                canActivate: [mobileRedirectGuard],
                loadChildren: () => import('./features/contacts-details/contacts.module').then(m => m.ContactsModule),
            },
            {
                path: 'eid-attachment-details',
                canActivate: [mobileRedirectGuard],
                loadChildren: () => import('./features/attach-eid-info/attach-eid-info.module').then(m => m.AttachEidInfoModule),
            },
            {
                path: 'useful-information',
                canActivate: [mobileRedirectGuard],
                loadChildren: () =>
                    import('./features/useful-information/useful-information.module').then(
                        m => m.UsefulInformationModule
                    ),
            },
            {
                path: 'search-content',
                loadChildren: () => import('./features/search-content/search.module').then(m => m.SearchModule),
            },
            {
                path: 'card-management',
                loadChildren: () =>
                    import('./features/card-management/card-management.module').then(m => m.CardManagementModule),
            },
            {
                path: 'logs-viewer',
                canActivate: [authGuard],
                loadChildren: () => import('./features/logs-viewer/logs-viewer.module').then(m => m.LogsViewerModule),
            },
        ],
    },
    {
        path: 'unauthorized',
        component: PageUnauthorizedComponent,
    },
    {
        path: 'mobile',
        component: LayoutWebviewComponent,
        children: [
            {
                path: 'contacts',
                loadChildren: () => import('./features/contacts-details/contacts.module').then(m => m.ContactsModule),
            },
            {
                path: 'eid-attachment-details',
                loadChildren: () => import('./features/attach-eid-info/attach-eid-info.module').then(m => m.AttachEidInfoModule),
            },
            {
                path: 'useful-information',
                loadChildren: () =>
                    import('./features/useful-information/useful-information.module').then(
                        m => m.UsefulInformationModule
                    ),
            },
            {
                path: 'home',
                loadChildren: () => import('./pages/home/home.module').then(m => m.HomeModule),
            },
        ],
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
