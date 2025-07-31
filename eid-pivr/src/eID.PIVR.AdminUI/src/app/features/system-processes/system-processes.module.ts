import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { TabMenuModule } from 'primeng/tabmenu';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { SelectButtonModule } from 'primeng/selectbutton';
import { CheckboxModule } from 'primeng/checkbox';
import { SkeletonModule } from 'primeng/skeleton';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { SystemProcessesRoutingModule } from './system-processes-routing.module';
import { SystemProcessesListComponent } from './pages/system-processes-list/system-processes-list.component';

@NgModule({
    declarations: [SystemProcessesListComponent],
    imports: [
        CommonModule,
        SystemProcessesRoutingModule,
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
    ],
})
export class SystemProcessesModule {}
