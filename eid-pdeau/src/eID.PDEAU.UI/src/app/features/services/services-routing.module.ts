import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProviderServicesComponent } from './pages/provider-services/provider-services.component';
import { AddServiceComponent } from './pages/add-service/add-service.component';
import { ProviderTypeGuard } from '@app/core/guards/provider-type.guard';
import { providerSubject } from '@app/features/providers/provider.dto';

const routes: Routes = [
    {
        path: '',
        component: ProviderServicesComponent,
    },
    {
        path: 'add',
        component: AddServiceComponent,
        canActivate: [ProviderTypeGuard(providerSubject.Enum.PrivateLawSubject)],
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ServicesRoutingModule {}
