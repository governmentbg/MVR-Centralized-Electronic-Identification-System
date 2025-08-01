import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GeneralInformationPageComponent } from './general-information-page/general-information-page.component';
import { SharedModule } from '@app/shared/shared.module';
import { CardModule } from 'primeng/card';
import { TranslocoModule } from '@ngneat/transloco';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { ToastModule } from 'primeng/toast';
import { GeneralInformationRoutingModule } from './general-information-routing.module';
import { InputNumberModule } from 'primeng/inputnumber';
import { DividerModule } from 'primeng/divider';
import { InputTextareaModule } from 'primeng/inputtextarea';

@NgModule({
    declarations: [GeneralInformationPageComponent],
    imports: [
        GeneralInformationRoutingModule,
        SharedModule,
        CardModule,
        TranslocoModule,
        ReactiveFormsModule,
        InputTextModule,
        ButtonModule,
        CommonModule,
        FormsModule,
        ToastModule,
        InputNumberModule,
        DividerModule,
        InputTextareaModule,
    ],
    providers: [],
    exports: [RouterModule],
})
export class GeneralInformationModule {}
