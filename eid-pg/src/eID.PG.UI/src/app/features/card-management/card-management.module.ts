import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardManagementMainComponent } from './pages/card-management-main/card-management-main.component';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { TranslocoModule } from '@ngneat/transloco';
import { SharedModule } from '../../shared/shared.module';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ToastModule } from 'primeng/toast';
import { CardManagementRoutingModule } from './card-management-routing.module';
import { PinChangeFormComponent } from './pages/pin-change-form/pin-change-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { MessagesModule } from 'primeng/messages';
import { DropdownModule } from 'primeng/dropdown';
import { PasswordModule } from 'primeng/password';
import { KeyFilterModule } from 'primeng/keyfilter';

@NgModule({
    declarations: [CardManagementMainComponent, PinChangeFormComponent],
    imports: [
        CommonModule,
        CardModule,
        DividerModule,
        ButtonModule,
        CardManagementRoutingModule,
        TranslocoModule,
        SharedModule,
        TranslocoLocaleModule,
        ToastModule,
        ReactiveFormsModule,
        InputTextModule,
        MessagesModule,
        DropdownModule,
        PasswordModule,
        KeyFilterModule,
    ],
})
export class CardManagementModule {}
