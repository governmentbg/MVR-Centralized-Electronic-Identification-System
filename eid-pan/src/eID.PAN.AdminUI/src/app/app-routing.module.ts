import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';
import { LayoutComponent } from './shared/components/layout/layout.component';
import { AuthGuard } from './core/guards/auth.guard';

const routes: Routes = [
    {
        path: '',
        component: LayoutComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'channels',
            },
            {
                path: 'channels',
                loadChildren: () => import('./features/channels/channels.module').then(m => m.ChannelsModule),
            },
            {
                path: 'configurations',
                loadChildren: () =>
                    import('./features/configurations/configurations.module').then(m => m.ConfigurationsModule),
            },
            {
                path: 'notifications',
                loadChildren: () =>
                    import('./features/notifications/notifications.module').then(m => m.NotificationsModule),
            },
            {
                path: 'logs-viewer',
                loadChildren: () => import('@eid/ngx-eid-logs-viewer').then(m => m.NgxEidLogsViewerModule),
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
