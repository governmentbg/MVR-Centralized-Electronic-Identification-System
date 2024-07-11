import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { DatasetsComponent } from './pages/datasets/datasets.component';
import { DatasetFormComponent } from './pages/dataset-form/dataset-form.component';

const routes: Routes = [
    {
        path: '',
        component: DatasetsComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'form',
        component: DatasetFormComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class DatasetsRoutingModule {}
