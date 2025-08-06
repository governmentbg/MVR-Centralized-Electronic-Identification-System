import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdministratorsComponent } from 'src/app/features/administrators/administrators.component';
import { HomeComponent } from './home.component';
import { Providers } from 'src/app/features/administrators/enums/enums';
import { ProvidersComponent } from 'src/app/features/administrators/components/providers/providers.component';
import { ProviderDataComponent } from 'src/app/features/administrators/components/provider-data/provider-data.component';
const routes: Routes = [
    {
        path: '',
        component: HomeComponent,
    },
    {
        path: Providers.ADMINISTRATORS,
        component: AdministratorsComponent,
    },
    {
        path: Providers.CENTERS,
        component: AdministratorsComponent,
    },
    {
        path: Providers.PROVIDERS,
        component: ProvidersComponent,
    },
    {
        path: `${Providers.ADMINISTRATORS}/:id`,
        component: ProviderDataComponent,
    },
    {
        path: `${Providers.CENTERS}/:id`,
        component: ProviderDataComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class HomeRoutingModule {}
