import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ReactiveFormsModule } from '@angular/forms';
import { EditorModule } from '@tinymce/tinymce-angular';
import { EditContentRoutingModule } from './edit-content-routing.module';
import { EditContentComponent } from './pages/edit-content/edit-content.component';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ContentListComponent } from './pages/content-list/content-list.component';
import { ToastModule } from 'primeng/toast';
import { DividerModule } from 'primeng/divider';
import { SharedModule } from '../../shared/shared.module';
import { SkeletonModule } from 'primeng/skeleton';
@NgModule({
    declarations: [ContentListComponent, EditContentComponent],
    imports: [
        CommonModule,
        CardModule,
        InputTextModule,
        ButtonModule,
        TranslocoModule,
        TranslocoLocaleModule,
        ReactiveFormsModule,
        EditorModule,
        ConfirmDialogModule,
        EditContentRoutingModule,
        ToastModule,
        DividerModule,
        SharedModule,
        SkeletonModule,
    ],
    providers: [ConfirmationService],
})
export class EditContentModule {}
