import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { SystemProcessesListComponent } from './pages/system-processes-list/system-processes-list.component';

const routes: Routes = [
    {
        path: '',
        component: SystemProcessesListComponent,
        canActivate: [AuthGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class SystemProcessesRoutingModule {}
