import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatasetsRoutingModule } from './datasets-routing.module';
import { DatasetsComponent } from './pages/datasets/datasets.component';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { SkeletonModule } from 'primeng/skeleton';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TranslocoRootModule } from 'src/app/transloco-root.module';
import { DatasetFormComponent } from './pages/dataset-form/dataset-form.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { BreadcrumbComponent } from 'src/app/shared/components/breadcrumb/breadcrumb.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
    declarations: [DatasetsComponent, DatasetFormComponent, BreadcrumbComponent],
    imports: [
        ReactiveFormsModule,
        FormsModule,
        CommonModule,
        DatasetsRoutingModule,
        ButtonModule,
        DropdownModule,
        TableModule,
        CardModule,
        DividerModule,
        SkeletonModule,
        CheckboxModule,
        InputTextModule,
        ConfirmDialogModule,
        SharedModule,
        TranslocoRootModule,
    ],
})
export class DatasetsModule {}
