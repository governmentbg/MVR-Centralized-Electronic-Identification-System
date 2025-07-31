import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationsComponent } from './pages/notifications/notifications.component';
import { NotificationsRoutingModule } from './notifications-routing.module';
import { TableModule } from 'primeng/table';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { TabMenuModule } from 'primeng/tabmenu';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SelectButtonModule } from 'primeng/selectbutton';
import { CheckboxModule } from 'primeng/checkbox';
import { SkeletonModule } from 'primeng/skeleton';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';

@NgModule({
    declarations: [NotificationsComponent],
    imports: [
        CommonModule,
        NotificationsRoutingModule,
        TableModule,
        TabMenuModule,
        ButtonModule,
        ConfirmDialogModule,
        ReactiveFormsModule,
        FormsModule,
        SharedModule,
        SelectButtonModule,
        CheckboxModule,
        SkeletonModule,
        DialogModule,
        InputTextareaModule,
        TranslocoLocaleModule,
    ],
})
export class NotificationsModule {}
