import { Component, Input } from '@angular/core';
import { IBreadCrumbItems } from '../../interfaces/IBreadCrumbItems';

@Component({
    selector: 'app-empowerment-check-breadcrumb',
    templateUrl: './empowerment-check-breadcrumb.component.html',
    styleUrls: ['./empowerment-check-breadcrumb.component.scss'],
})
export class EmpowermentCheckBreadcrumbComponent {
    @Input() items: IBreadCrumbItems[] = [];
}
