import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { TranslocoModule } from '@ngneat/transloco';
import { SharedModule } from '../../shared/shared.module';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ToastModule } from 'primeng/toast';
import { ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { MessagesModule } from 'primeng/messages';
import { DropdownModule } from 'primeng/dropdown';
import { PasswordModule } from 'primeng/password';

@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        CardModule,
        DividerModule,
        ButtonModule,
        TranslocoModule,
        SharedModule,
        TranslocoLocaleModule,
        ToastModule,
        ReactiveFormsModule,
        InputTextModule,
        MessagesModule,
        DropdownModule,
        PasswordModule,
    ],
})
export class CardManagementModule {}
