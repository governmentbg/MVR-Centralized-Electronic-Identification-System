import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ReactiveFormsModule } from '@angular/forms';
import { EditorModule } from '@tinymce/tinymce-angular';
import { CreateContentComponent } from './pages/create-content.component';
import { CreateContentRoutingModule } from './create-content-routing.module';
import { ToastModule } from 'primeng/toast';
import { DividerModule } from 'primeng/divider';

@NgModule({
    declarations: [CreateContentComponent],
    imports: [
        CommonModule,
        CardModule,
        InputTextModule,
        ButtonModule,
        TranslocoModule,
        TranslocoLocaleModule,
        ReactiveFormsModule,
        EditorModule,
        CreateContentRoutingModule,
        ToastModule,
        DividerModule,
    ],
})
export class CreateContentModule {}
