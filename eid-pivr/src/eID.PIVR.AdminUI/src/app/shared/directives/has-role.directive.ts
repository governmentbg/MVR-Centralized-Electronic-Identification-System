import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { UserService } from '../../core/services/user.service';
import { RoleType } from '../../core/enums/auth.enum';

@Directive({
    selector: '[appHasRole]',
})
export class HasRoleDirective implements OnInit {
    @Input() appHasRole!: RoleType[];

    constructor(
        private viewContainerRef: ViewContainerRef,
        private templateRef: TemplateRef<any>,
        private userService: UserService
    ) {}

    ngOnInit() {
        const userRoles = this.userService.getUserRoles();
        // if there are no roles clear the view container ref
        if (!userRoles) {
            this.viewContainerRef.clear();
        }

        // if user has the needed role then render the element
        if (userRoles.some(role => this.appHasRole.includes(role as RoleType))) {
            this.viewContainerRef.createEmbeddedView(this.templateRef);
        } else {
            this.viewContainerRef.clear();
        }
    }
}
