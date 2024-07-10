import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { ConfigurationsListComponent } from './pages/configurations-list/configurations-list.component';
import { ConfigurationModifyComponent } from './pages/configuration-modify/configuration-modify.component';

const routes: Routes = [
    {
        path: '',
        component: ConfigurationsListComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'edit/:id',
        component: ConfigurationModifyComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'add',
        component: ConfigurationModifyComponent,
        canActivate: [AuthGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ConfigurationsRoutingModule {}
