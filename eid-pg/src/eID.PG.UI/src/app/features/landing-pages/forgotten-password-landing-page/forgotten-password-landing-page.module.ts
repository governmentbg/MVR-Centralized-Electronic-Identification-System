import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ChipModule } from 'primeng/chip';
import { ForgottenPasswordLandingPageComponent } from './forgotten-password-landing-page.component';
import { ToastModule } from 'primeng/toast';
import { ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { KeyFilterModule } from 'primeng/keyfilter';
import { PasswordModule } from 'primeng/password';
const routes: Routes = [
    {
        path: '',
        component: ForgottenPasswordLandingPageComponent,
    },
];
@NgModule({
    declarations: [ForgottenPasswordLandingPageComponent],
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        TranslocoLocaleModule,
        TranslocoModule,
        ChipModule,
        ToastModule,
        ReactiveFormsModule,
        ButtonModule,
        InputTextModule,
        CardModule,
        KeyFilterModule,
        PasswordModule
    ],
    exports: [RouterModule],
})
export class ForgottenPasswordLandingPageModule {}
