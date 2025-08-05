import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { ReactiveFormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { DialogModule } from 'primeng/dialog';
import { EidReferencesComponent } from './eid-references.component';
import { EidReferencesRoutingModule } from './eid-references-routing.module';
import { DividerModule } from 'primeng/divider';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
    declarations: [EidReferencesComponent],
    imports: [
        CommonModule,
        CardModule,
        InputTextModule,
        ButtonModule,
        TranslocoModule,
        TranslocoLocaleModule,
        EidReferencesRoutingModule,
        ReactiveFormsModule,
        ToastModule,
        TableModule,
        SkeletonModule,
        DialogModule,
        DividerModule,
        SharedModule,
    ],
})
export class EidReferencesModule {}
