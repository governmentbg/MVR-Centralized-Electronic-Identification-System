import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RouterModule, Routes } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ForgottenPasswordComponent } from './forgotten-password.component';
import { InputTextModule } from 'primeng/inputtext';
import { ReactiveFormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { CardModule } from 'primeng/card';
import { SharedModule } from 'src/app/shared/shared.module';
const routes: Routes = [
    {
        path: '',
        component: ForgottenPasswordComponent,
    },
];

@NgModule({
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        ButtonModule,
        TranslocoLocaleModule,
        TranslocoModule,
        InputTextModule,
        ReactiveFormsModule,
        ToastModule,
        CardModule,
        SharedModule,
    ],
    declarations: [ForgottenPasswordComponent],
    exports: [RouterModule],
})
export class ForgottenPasswordModule {}
