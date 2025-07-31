import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentTypesRoutingModule } from '@app/features/document-types/document-types-routing.module';
import { DocumentTypesListComponent } from './pages/document-types-list/document-types-list.component';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TabViewModule } from 'primeng/tabview';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TranslocoModule } from '@ngneat/transloco';
import { DocumentTypesFormComponent } from './components/document-types-form/document-types-form.component';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@app/shared/shared.module';
import { InputTextModule } from 'primeng/inputtext';

@NgModule({
    declarations: [DocumentTypesListComponent, DocumentTypesFormComponent],
    imports: [
        CommonModule,
        DocumentTypesRoutingModule,
        ButtonModule,
        CardModule,
        SharedModule,
        TabViewModule,
        TableModule,
        SkeletonModule,
        ConfirmDialogModule,
        TranslocoModule,
        CalendarModule,
        CheckboxModule,
        InputNumberModule,
        ReactiveFormsModule,
        SharedModule,
        InputTextModule,
    ],
})
export class DocumentTypesModule {}
