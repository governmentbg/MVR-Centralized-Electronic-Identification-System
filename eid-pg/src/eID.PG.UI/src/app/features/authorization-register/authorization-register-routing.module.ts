import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizationRegisterComponent } from './pages/authorization-register/authorization-register.component';
import { NewAuthorizationFormComponent } from './pages/new-authorization-form/new-authorization-form.component';
import { AuthorizationFromMeComponent } from './pages/authorization-from-me/authorization-from-me.component';
import { AuthorizationToMeComponent } from './pages/authorization-to-me/authorization-to-me.component';
import { EmpowermentPreviewComponent } from './components/empowerement-preview/empowerment-preview.component';
import { AuthorizationForLegalEntityComponent } from './pages/authorization-for-legal-entity/authorization-for-legal-entity.component';
import { blockNavigationGuard } from '../../core/guards/block-navigation.guard';

const routes: Routes = [
    {
        path: '',
        component: AuthorizationRegisterComponent,
    },
    {
        path: 'new-authorization',
        component: NewAuthorizationFormComponent,
        canDeactivate: [blockNavigationGuard],
    },
    {
        path: 'from-me',
        component: AuthorizationFromMeComponent,
    },
    {
        path: 'to-me',
        component: AuthorizationToMeComponent,
    },
    {
        path: 'details/:id',
        component: EmpowermentPreviewComponent,
    },
    {
        path: 'legal-entity',
        component: AuthorizationForLegalEntityComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AuthorizationRegisterRoutingModule {}
