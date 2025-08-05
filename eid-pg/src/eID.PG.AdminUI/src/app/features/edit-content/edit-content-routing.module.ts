import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { ContentListComponent } from './pages/content-list/content-list.component';

const routes: Routes = [
    {
        path: '',
        component: ContentListComponent,
        canActivate: [AuthGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class EditContentRoutingModule {}
