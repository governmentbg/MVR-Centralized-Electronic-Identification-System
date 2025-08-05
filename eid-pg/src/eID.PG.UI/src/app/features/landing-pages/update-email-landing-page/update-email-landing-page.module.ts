import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { UpdateEmailLandingPageComponent } from './update-email-landing-page.component';

const routes: Routes = [
    {
        path: '',
        component: UpdateEmailLandingPageComponent,
    },
];
@NgModule({
    declarations: [UpdateEmailLandingPageComponent],
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        TranslocoLocaleModule,
        TranslocoModule,
        ButtonModule,
        CardModule,
    ],
    exports: [RouterModule],
})
export class UpdateEmailLandingPageModule {}
