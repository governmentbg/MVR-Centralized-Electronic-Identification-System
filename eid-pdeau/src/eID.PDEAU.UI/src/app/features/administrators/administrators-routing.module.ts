import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdministratorApplicationsComponent } from '@app/features/administrators/pages/administrator-applications/administrator-applications.component';
import { RegisterAdministratorComponent } from '@app/features/administrators/pages/register-administrator/register-administrator.component';
import { CitizenGuard } from '@app/core/guards/citizen.guard';
import { ApplicationPreviewComponent } from '@app/features/administrators/pages/application-preview/application-preview.component';
import { AuthGuard } from '@app/core/guards/auth.guard';
import { RoleType } from '@app/core/enums/auth.enum';
import { EmployeesComponent } from '@app/features/administrators/pages/employees/employees.component';
import { OfficesComponent } from '@app/features/administrators/pages/offices/offices.component';
import { DevicesComponent } from '@app/features/administrators/pages/devices/devices.component';
import {
    GeneralInformationComponent
} from '@app/features/administrators/pages/general-information/general-information.component';
import { DocumentsComponent } from '@app/features/administrators/pages/documents/documents.component';

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
        path: 'register',
        component: RegisterAdministratorComponent,
        canActivate: [CitizenGuard],
    },
    {
        path: 'applications/:id',
        component: ApplicationPreviewComponent,
    },
    {
        path: 'current',
        component: ApplicationPreviewComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.ADMINISTRATOR] },
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
        path: 'devices',
        component: DevicesComponent,
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
export class AdministratorsRoutingModule {}
