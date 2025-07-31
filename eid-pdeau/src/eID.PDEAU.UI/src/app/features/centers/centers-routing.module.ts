import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CitizenGuard } from '@app/core/guards/citizen.guard';
import { ApplicationPreviewComponent } from '@app/features/centers/pages/application-preview/application-preview.component';
import { RegisterCenterComponent } from '@app/features/centers/pages/register-center/register-center.component';
import { CenterApplicationsComponent } from '@app/features/centers/pages/center-applications/center-applications.component';
import { AuthGuard } from '@app/core/guards/auth.guard';
import { RoleType } from '@app/core/enums/auth.enum';
import { GeneralInformationComponent } from '@app/features/centers/pages/general-information/general-information.component';
import { EmployeesComponent } from '@app/features/centers/pages/employees/employees.component';
import { OfficesComponent } from '@app/features/centers/pages/offices/offices.component';
import { DocumentsComponent } from '@app/features/centers/pages/documents/documents.component';

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
        path: 'register',
        component: RegisterCenterComponent,
        canActivate: [CitizenGuard],
    },
    {
        path: 'applications/:id',
        component: ApplicationPreviewComponent,
    },
    {
        path: 'general-information',
        component: GeneralInformationComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.ADMINISTRATOR] },
    },
    {
        path: 'employees',
        component: EmployeesComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.ADMINISTRATOR] },
    },
    {
        path: 'offices',
        component: OfficesComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.ADMINISTRATOR] },
    },
    {
        path: 'documents',
        component: DocumentsComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.ADMINISTRATOR] },
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class CentersRoutingModule {}
