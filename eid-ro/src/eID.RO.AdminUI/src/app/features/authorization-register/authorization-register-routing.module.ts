import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizationsListComponent } from './pages/authorizations-list/authorizations-list.component';

const routes: Routes = [
    {
        path: '',
        component: AuthorizationsListComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AuthorizationRegisterRoutingModule {}
