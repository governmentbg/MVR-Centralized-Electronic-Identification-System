import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegisterComponent } from './register.component';
import { RouterModule, Routes } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ChipModule } from 'primeng/chip';
import { ReactiveFormsModule } from '@angular/forms';
import { InputMaskModule } from 'primeng/inputmask';
import { MessagesModule } from 'primeng/messages';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CardModule } from 'primeng/card';
import { KeyFilterModule } from 'primeng/keyfilter';
import { SharedModule } from 'src/app/shared/shared.module';
const routes: Routes = [
    {
        path: '',
        component: RegisterComponent,
    },
];
@NgModule({
    declarations: [RegisterComponent],
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        TranslocoLocaleModule,
        TranslocoModule,
        ChipModule,
        ReactiveFormsModule,
        InputMaskModule,
        MessagesModule,
        InputTextModule,
        ButtonModule,
        InputNumberModule,
        CardModule,
        KeyFilterModule,
        SharedModule,
    ],
    exports: [RouterModule],
})
export class RegisterModule {}
