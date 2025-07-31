import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApplicationPreviewComponent } from '@app/features/centers/pages/application-preview/application-preview.component';
import { RegisterCenterComponent } from '@app/features/centers/pages/register-center/register-center.component';
import { CenterApplicationsComponent } from '@app/features/centers/pages/center-applications/center-applications.component';
import { CircumstancePreviewComponent } from '@app/features/centers/pages/circumstance-preview/circumstance-preview.component';
import { CenterRegistrationsComponent } from '@app/features/centers/pages/center-registrations/center-registrations.component';
import { CenterPreviewComponent } from '@app/features/centers/pages/center-preview/center-preview.component';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'applications',
    },
    {
        path: 'applications',
        component: CenterApplicationsComponent,
    },
    {
        path: 'registrations',
        component: CenterRegistrationsComponent,
    },
    {
        path: 'applications/register',
        component: RegisterCenterComponent,
    },
    {
        path: 'applications/:id',
        component: ApplicationPreviewComponent,
    },
    {
        path: 'registrations/:id',
        component: CenterPreviewComponent,
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
export class CentersRoutingModule {}
