import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { TranslocoRootModule } from '../transloco-root.module';
import { FormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { PageUnauthorizedComponent } from './components/page-unauthorized/page-unauthorized.component';

@NgModule({
    declarations: [PageNotFoundComponent, PageUnauthorizedComponent],
    imports: [CommonModule, RouterModule, TranslocoRootModule, FormsModule, ToastModule],
    exports: [PageNotFoundComponent, TranslocoRootModule],
    providers: [MessageService],
})
export class SharedModule {}
