import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SearchContentComponent } from './search-content.component';

const routes: Routes = [
    {
        path: '',
        component: SearchContentComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class SearchRoutingModule { }
