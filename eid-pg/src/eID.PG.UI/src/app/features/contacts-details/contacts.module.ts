import { NgModule } from '@angular/core';
import { ContactsDetailsComponent } from './contacts-details.component';
import { Routes, RouterModule } from '@angular/router';
import { FieldsetModule } from 'primeng/fieldset';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { TranslocoModule } from '@ngneat/transloco';
import { CardModule } from 'primeng/card';
import { CommonModule } from '@angular/common';
import { DividerModule } from 'primeng/divider';

const routes: Routes = [
    {
        path: '',
        component: ContactsDetailsComponent,
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
        DividerModule
    ],
    declarations: [ContactsDetailsComponent],
    exports: [RouterModule],
})
export class ContactsModule {}
