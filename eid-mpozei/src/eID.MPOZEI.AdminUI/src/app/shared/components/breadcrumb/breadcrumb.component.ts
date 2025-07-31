import { Component, Input } from '@angular/core';
import { IBreadCrumbItems } from '../../interfaces/shared.interfaces';

@Component({
    selector: 'app-breadcrumb',
    templateUrl: './breadcrumb.component.html',
    styleUrls: ['./breadcrumb.component.scss'],
})
export class BreadcrumbComponent {
    @Input() items: IBreadCrumbItems[] = [];
}
