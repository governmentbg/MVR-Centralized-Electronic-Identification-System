import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdministrationLoginComponent } from './administration-login.component';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { DividerModule } from 'primeng/divider';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ToastModule } from 'primeng/toast';
import { SharedModule } from 'src/app/shared/shared.module';
const routes: Routes = [
    {
        path: '',
        component: AdministrationLoginComponent,
    },
];
@NgModule({
    declarations: [AdministrationLoginComponent],
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        ButtonModule,
        TranslocoLocaleModule,
        TranslocoModule,
        CardModule,
        ReactiveFormsModule,
        PasswordModule,
        InputTextModule,
        DropdownModule,
        ToastModule,
        DividerModule,
        CheckboxModule,
        SharedModule
    ],
    exports: [RouterModule],
})
export class AdministrationLoginModule {}
