import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterProviderComponent } from './pages/register-provider/register-provider.component';
import { ProviderPreviewComponent } from './pages/provider-preview/provider-preview.component';
import { ProvidersComponent } from './pages/providers/providers.component';

const routes: Routes = [
    {
        path: '',
        component: ProvidersComponent,
    },
    {
        path: 'register',
        component: RegisterProviderComponent,
    },
    {
        path: ':providerId',
        component: ProviderPreviewComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ProvidersRoutingModule {}
