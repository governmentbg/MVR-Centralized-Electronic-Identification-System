import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterProviderComponent } from './pages/register-provider/register-provider.component';
import { ProviderApplicationsComponent } from './pages/provider-applications/provider-applications.component';
import { ProviderPreviewComponent } from './pages/provider-preview/provider-preview.component';
import { CurrentProviderPreviewComponent } from './pages/current-provider-preview/current-provider-preview.component';
import { RoleType } from '@app/core/enums/auth.enum';
import { AuthGuard } from '@app/core/guards/auth.guard';
import { CitizenGuard } from '@app/core/guards/citizen.guard';
import { providerGuard } from '@app/core/guards/provider.guard';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        children: [],
        canActivate: [providerGuard],
    },
    {
        path: 'applications',
        component: ProviderApplicationsComponent,
        canActivate: [CitizenGuard],
    },
    {
        path: 'register',
        component: RegisterProviderComponent,
        canActivate: [CitizenGuard],
    },
    {
        path: 'current',
        component: CurrentProviderPreviewComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.ADMINISTRATOR] },
    },
    {
        path: 'applications/:providerId',
        component: ProviderPreviewComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ProvidersRoutingModule {}
