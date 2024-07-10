import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfigurationsListComponent } from './pages/configurations-list/configurations-list.component';
import { ConfigurationsRoutingModule } from './configurations-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfigurationModifyComponent } from './pages/configuration-modify/configuration-modify.component';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ToastModule } from 'primeng/toast';
import { SkeletonModule } from 'primeng/skeleton';

@NgModule({
    declarations: [ConfigurationsListComponent, ConfigurationModifyComponent],
    imports: [
        CommonModule,
        ConfigurationsRoutingModule,
        SharedModule,
        FormsModule,
        TableModule,
        InputTextModule,
        InputNumberModule,
        ButtonModule,
        CheckboxModule,
        SelectButtonModule,
        ConfirmDialogModule,
        ReactiveFormsModule,
        RadioButtonModule,
        ToastModule,
        SkeletonModule,
    ],
})
export class ConfigurationsModule {}
