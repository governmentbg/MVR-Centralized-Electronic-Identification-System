import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsersTableComponent } from './pages/users-table/users-table.component';
import { UserRegisterComponent } from './pages/user-register/user-register.component';
import { UserPreviewComponent } from './pages/user-preview/user-preview.component';

const routes: Routes = [
    {
        path: '',
        component: UsersTableComponent,
    },
    {
        path: 'register',
        component: UserRegisterComponent,
    },
    {
        path: ':id',
        component: UserPreviewComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ProvidersRoutingModule {}
