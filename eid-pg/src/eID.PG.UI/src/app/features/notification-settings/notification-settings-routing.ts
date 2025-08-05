import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from 'src/app/core/guards/auth.guard';
import { NotificationSettingsComponent } from './pages/notification-settings.component';

const routes: Routes = [
    {
        path: '',
        component: NotificationSettingsComponent,
        canActivate: [authGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class NotificationSettingsRoutingModule {}
