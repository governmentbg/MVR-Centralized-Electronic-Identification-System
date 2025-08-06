import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RoleType } from '../../../core/enums/auth.enum';

@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent implements OnInit {
    constructor(private router: Router) {}
    isMenuToggled = false;
    RoleType = RoleType;
    ngOnInit() {
        if (this.router.url === '/logs-viewer/journals' || this.router.url === '/logs-viewer/integrity') {
            this.isMenuToggled = true;
        }
    }
    toggleSubMenu(value: boolean) {
        this.isMenuToggled = value;
        if (this.isMenuToggled) {
            this.router.navigate(['/logs-viewer/journals']);
        }
    }
}
