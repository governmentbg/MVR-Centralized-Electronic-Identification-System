import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { LoginComponent } from './login.component';
import { RouterModule, Routes } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { TranslocoLocaleModule } from '@ngneat/transloco-locale';
import { CardModule } from 'primeng/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { MobileEidLoginComponent } from './mobile-eid-login/mobile-eid-login.component';
import { PersonalDocumentEidLoginComponent } from './personal-document-eid-login/personal-document-eid-login.component';
import { AuthPersonalDocumentComponent } from './components/auth-personal-document/auth-personal-document.component';
import { DropdownModule } from 'primeng/dropdown';
import { ToastModule } from 'primeng/toast';
import { DividerModule } from 'primeng/divider';
import { CheckboxModule } from 'primeng/checkbox';
import { SharedModule } from 'src/app/shared/shared.module';
import { AuthMultifactorComponent } from './auth-multifactor/auth-multifactor.component';
import { KnobModule } from 'primeng/knob';
import { InputMaskModule } from 'primeng/inputmask';
import { MessagesModule } from 'primeng/messages';
const routes: Routes = [
    {
        path: '',
        component: LoginComponent,
    },
    {
        path: 'mobile-eid',
        component: MobileEidLoginComponent,
    },
    {
        path: 'personal-document-eid',
        component: PersonalDocumentEidLoginComponent,
    },
    {
        path: 'personal-document-eid/auth',
        component: AuthPersonalDocumentComponent,
    },
    {
        path: '2fa',
        component: AuthMultifactorComponent,
    },
];

@NgModule({
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        ButtonModule,
        TranslocoLocaleModule,
        TranslocoModule,
        CardModule,
        ReactiveFormsModule,
        PasswordModule,
        InputTextModule,
        DropdownModule,
        ToastModule,
        DividerModule,
        CheckboxModule,
        SharedModule,
        KnobModule,
        InputMaskModule,
        FormsModule,
        MessagesModule
    ],
    declarations: [
        LoginComponent,
        MobileEidLoginComponent,
        PersonalDocumentEidLoginComponent,
        AuthPersonalDocumentComponent,
        AuthMultifactorComponent
    ],
    exports: [RouterModule],
})
export class LoginModule {}
