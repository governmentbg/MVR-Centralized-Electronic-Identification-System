import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { SearchFormComponent } from './pages/search-form/search-form.component';
import { SearchResultsComponent } from './pages/search-results/search-results.component';
import { ApplicationFormComponent } from './pages/application-form/application-form.component';
import { RedirectToSearchFormGuard } from './guards/redirect-to-search-form.guard';
import { ApplicationDetailsComponent } from './components/application-details/application-details.component';
import { CertificateDetailsComponent } from './components/certificate-details/certificate-details.component';
import { blockNavigationGuard } from '../../core/guards/block-navigation.guard';
import { ApplicationsListComponent } from './pages/applications-list/applications-list.component';
import { RoleType } from '../../core/enums/auth.enum';
import { redirectGuard } from '../../core/guards/redirect.guard';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        children: [],
        canActivate: [redirectGuard],
    },
    {
        path: 'search',
        component: SearchFormComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.OPERATOR, RoleType.RU_MVR_ADMINISTRATOR, RoleType.AEID] },
    },
    {
        path: 'search/results',
        component: SearchResultsComponent,
        canActivate: [AuthGuard, RedirectToSearchFormGuard],
        data: { roles: [RoleType.OPERATOR, RoleType.RU_MVR_ADMINISTRATOR, RoleType.AEID] },
    },
    {
        path: 'search/new-application',
        component: ApplicationFormComponent,
        canActivate: [AuthGuard, RedirectToSearchFormGuard],
        canDeactivate: [blockNavigationGuard],
        data: {
            blockNavigationGuardMessage: 'modules.eidManagement.txtExitCertificateProcessConfirmation',
            roles: [RoleType.OPERATOR, RoleType.RU_MVR_ADMINISTRATOR],
        },
    },
    {
        path: 'application/:id',
        component: ApplicationDetailsComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.APP_ADMINISTRATOR, RoleType.OPERATOR, RoleType.RU_MVR_ADMINISTRATOR, RoleType.AEID] },
    },
    {
        path: 'certificate/:id',
        component: CertificateDetailsComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.APP_ADMINISTRATOR, RoleType.OPERATOR, RoleType.RU_MVR_ADMINISTRATOR, RoleType.AEID] },
    },
    {
        path: 'applications-list',
        component: ApplicationsListComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.APP_ADMINISTRATOR, RoleType.RU_MVR_ADMINISTRATOR] },
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class EidManagementRoutingModule {}
