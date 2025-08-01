import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './components/navbar/navbar.component';
import { RouterModule } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { LayoutComponent } from './components/layout/layout.component';
import { InputComponent } from './components/input/input.component';
import { ShellComponent } from './components/shell/shell.component';
import { FooterComponent } from './components/footer/footer.component';
import { TranslocoRootModule } from '../transloco-root.module';
import { LanguageSelectorComponent } from './components/language-selector/language-selector.component';
import { FormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { PageUnauthorizedComponent } from './components/page-unauthorized/page-unauthorized.component';
import { SubNavbarComponent } from './components/sub-navbar/sub-navbar.component';
import { MenubarModule } from 'primeng/menubar';
import { DropdownModule } from 'primeng/dropdown';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { TrimDirective } from './directives/trim.directive';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { InplaceModule } from 'primeng/inplace';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
@NgModule({
    declarations: [
        NavbarComponent,
        PageNotFoundComponent,
        LayoutComponent,
        InputComponent,
        ShellComponent,
        FooterComponent,
        LanguageSelectorComponent,
        PageUnauthorizedComponent,
        SubNavbarComponent,
        BreadcrumbComponent,
        TrimDirective
    ],
    imports: [
        CommonModule,
        RouterModule,
        TranslocoRootModule,
        FormsModule,
        ToastModule,
        MenubarModule,
        DropdownModule,
        ButtonModule,
        DividerModule,
        InplaceModule,
        ConfirmDialogModule,
    ],
    exports: [
        NavbarComponent,
        PageNotFoundComponent,
        InputComponent,
        TranslocoRootModule,
        SubNavbarComponent,
        BreadcrumbComponent,
        TrimDirective
    ],
    providers: [MessageService, ConfirmationService],
})
export class SharedModule {}
