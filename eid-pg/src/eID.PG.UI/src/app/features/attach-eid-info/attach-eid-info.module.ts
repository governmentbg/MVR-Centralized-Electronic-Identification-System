import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttachEidInfoComponent } from './attach-eid-info.component';
import { Routes, RouterModule } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { FieldsetModule } from 'primeng/fieldset';

const routes: Routes = [
    {
        path: '',
        component: AttachEidInfoComponent,
    },
];

@NgModule({
    imports: [
        RouterModule.forChild(routes),
        FieldsetModule,
        TranslocoLocaleModule,
        TranslocoModule,
        CardModule,
        CommonModule,
        DividerModule,
    ],
    declarations: [AttachEidInfoComponent],
    exports: [RouterModule],
})
export class AttachEidInfoModule {}
