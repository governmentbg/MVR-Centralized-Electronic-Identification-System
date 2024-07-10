import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChannelsComponent } from './pages/channels/channels.component';
import { ChannelsRoutingModule } from './channels-routing.module';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { TranslocoRootModule } from 'src/app/transloco-root.module';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SelectButtonModule } from 'primeng/selectbutton';
import { SkeletonModule } from 'primeng/skeleton';

@NgModule({
    declarations: [ChannelsComponent],
    imports: [
        CommonModule,
        ChannelsRoutingModule,
        TableModule,
        ToastModule,
        ButtonModule,
        ConfirmDialogModule,
        ReactiveFormsModule,
        SharedModule,
        TranslocoRootModule,
        FormsModule,
        SelectButtonModule,
        SkeletonModule,
    ],
})
export class ChannelsModule {}
