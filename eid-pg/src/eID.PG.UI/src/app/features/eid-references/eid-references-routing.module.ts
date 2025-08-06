import { NgModule } from '@angular/core';
import { EidReferencesComponent } from './eid-references.component';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
    {
        path: '',
        component: EidReferencesComponent,
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class EidReferencesRoutingModule { }
