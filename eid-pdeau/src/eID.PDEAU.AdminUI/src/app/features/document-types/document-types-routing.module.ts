import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DocumentTypesListComponent } from '@app/features/document-types/pages/document-types-list/document-types-list.component';
import { DocumentTypesFormComponent } from '@app/features/document-types/components/document-types-form/document-types-form.component';

const routes: Routes = [
    {
        path: '',
        component: DocumentTypesListComponent,
    },
    {
        path: 'new',
        component: DocumentTypesFormComponent,
    },
    {
        path: 'edit',
        component: DocumentTypesFormComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class DocumentTypesRoutingModule {}
