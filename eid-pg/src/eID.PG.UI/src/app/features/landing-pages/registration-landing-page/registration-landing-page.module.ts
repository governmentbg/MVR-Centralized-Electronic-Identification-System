import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { RegistrationLandingPageComponent } from './registration-landing-page.component';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

const routes: Routes = [
    {
        path: '',
        component: RegistrationLandingPageComponent,
    },
];
@NgModule({
    declarations: [RegistrationLandingPageComponent],
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
export class RegistrationLandingPageModule {}
