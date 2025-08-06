import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from 'src/app/core/guards/auth.guard';
import { PinChangeFormComponent } from './pages/pin-change-form/pin-change-form.component';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'change-pin',
    },
    {
        path: 'change-pin',
        component: PinChangeFormComponent,
        canActivate: [authGuard],
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class CardManagementRoutingModule {}
