import { Component } from '@angular/core';
import { UserService } from '@app/core/services/user.service';
import { RoleType, SystemType } from '@app/core/enums/auth.enum';

@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent {
    constructor(private userService: UserService) {}
    RoleType = RoleType;
    SystemType = SystemType;
    _userService = this.userService;
}
