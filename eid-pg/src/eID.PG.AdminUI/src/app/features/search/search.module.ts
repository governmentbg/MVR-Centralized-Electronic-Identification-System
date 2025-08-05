import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchComponent } from './pages/search.component';
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

@NgModule({
    declarations: [SearchComponent],
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
    ],
})
export class SearchModule {}
