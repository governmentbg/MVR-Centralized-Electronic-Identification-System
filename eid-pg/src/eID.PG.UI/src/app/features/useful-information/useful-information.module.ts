import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FieldsetModule } from 'primeng/fieldset';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { TranslocoModule } from '@ngneat/transloco';
import { UsefulInformationComponent } from './useful-information.component';
import { CardModule } from 'primeng/card';
import { TreeModule } from 'primeng/tree';

import { CommonModule } from '@angular/common';
import { PagePreviewComponent } from './components/page-preview/page-preview.component';
import { SharedModule } from "../../shared/shared.module";
import { DividerModule } from 'primeng/divider';
const routes: Routes = [
    {
        path: '',
        component: UsefulInformationComponent,
    },
];
@NgModule({
    imports: [
        RouterModule.forChild(routes),
        FieldsetModule,
        TranslocoLocaleModule,
        TranslocoModule,
        CardModule,
        TreeModule,
        CommonModule,
        SharedModule,
        DividerModule
    ],
    declarations: [UsefulInformationComponent, PagePreviewComponent],
    exports: [RouterModule],
})
export class UsefulInformationModule {}
