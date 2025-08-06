import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from 'src/app/core/guards/auth.guard';
import { EidManagementComponent } from './pages/eid-management/eid-management.component';
import { ApplicationsComponent } from './pages/applications/applications.component';
import { CertificatesComponent } from './pages/certificates/certificates.component';
import { NewApplicationFormComponent } from './pages/new-application-form/new-application-form.component';
import { CertificatesPreviewComponent } from './components/certificates-preview/certificates-preview.component';
import { ApplicationPreviewComponent } from './components/application-preview/application-preview.component';

const routes: Routes = [
    {
        path: '',
        component: EidManagementComponent,
        canActivate: [authGuard],
    },
    {
        path: 'new-application',
        component: NewApplicationFormComponent,
    },
    {
        path: 'applications',
        component: ApplicationsComponent,
    },
    {
        path: 'certificates',
        component: CertificatesComponent,
    },
    {
        path: 'certificates/:id',
        component: CertificatesPreviewComponent,
        canActivate: [authGuard],
    },
    {
        path: 'applications/:id',
        component: ApplicationPreviewComponent,
        canActivate: [authGuard],
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class EidManagementRoutingModule {}
