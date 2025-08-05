import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchRoutingModule } from './search-routing.module';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ReactiveFormsModule } from '@angular/forms';
import { EditorModule } from '@tinymce/tinymce-angular';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { DialogModule } from 'primeng/dialog';
import { SearchContentComponent } from './search-content.component';
import { DividerModule } from 'primeng/divider';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
    declarations: [SearchContentComponent],
    imports: [
        CommonModule,
        SearchRoutingModule,
        CardModule,
        InputTextModule,
        ButtonModule,
        TranslocoModule,
        TranslocoLocaleModule,
        ReactiveFormsModule,
        EditorModule,
        ToastModule,
        TableModule,
        SkeletonModule,
        DialogModule,
        DividerModule,
        SharedModule,
    ],
})
export class SearchModule {}
