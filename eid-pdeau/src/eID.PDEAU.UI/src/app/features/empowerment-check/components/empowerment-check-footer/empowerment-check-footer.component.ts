import { Component } from '@angular/core';

@Component({
    selector: 'app-empowerment-check-footer',
    templateUrl: './empowerment-check-footer.component.html',
    styleUrls: ['./empowerment-check-footer.component.scss'],
})
export class EmpowermentCheckFooterComponent {
    currentYear = new Date().getFullYear();
}
