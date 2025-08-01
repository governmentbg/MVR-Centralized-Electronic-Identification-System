import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdministratorApplicationsComponent } from '@app/features/administrators/pages/administrator-applications/administrator-applications.component';
import { RegisterAdministratorComponent } from '@app/features/administrators/pages/register-administrator/register-administrator.component';
import { ApplicationPreviewComponent } from '@app/features/administrators/pages/application-preview/application-preview.component';
import { CircumstancePreviewComponent } from '@app/features/administrators/pages/circumstance-preview/circumstance-preview.component';
import { AdministratorRegistrationsComponent } from '@app/features/administrators/pages/administrator-registrations/administrator-registrations.component';
import { AdministratorPreviewComponent } from '@app/features/administrators/pages/administrator-preview/administrator-preview.component';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'applications',
    },
    {
        path: 'applications',
        component: AdministratorApplicationsComponent,
    },
    {
        path: 'registrations',
        component: AdministratorRegistrationsComponent,
    },
    {
        path: 'applications/register',
        component: RegisterAdministratorComponent,
    },
    {
        path: 'applications/:id',
        component: ApplicationPreviewComponent,
    },
    {
        path: 'registrations/:id',
        component: AdministratorPreviewComponent,
    },
    {
        path: 'registrations/circumstance/:id',
        component: CircumstancePreviewComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AdministratorsRoutingModule {}
