import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { UpdateEmailComponent } from './components/update-email/update-email.component';
import { UpdatePasswordComponent } from './components/update-password/update-password.component';
import { UpdatePersonalInfoComponent } from './components/update-personal-info/update-personal-info.component';
import { ProfileComponent } from './profile.component';

const routes: Routes = [
    {
        path: '',
        component: ProfileComponent,
    },
    {
        path: 'update-password',
        component: UpdatePasswordComponent,
    },
    {
        path: 'update-email',
        component: UpdateEmailComponent,
    },
    {
        path: 'update-personal-info',
        component: UpdatePersonalInfoComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ProfileRoutingModule {}
