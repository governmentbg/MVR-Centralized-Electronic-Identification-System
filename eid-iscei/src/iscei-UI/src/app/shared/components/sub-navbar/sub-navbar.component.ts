import { Component, ElementRef, HostListener, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';
import { IUser } from 'src/app/core/interfaces/IUser';
import { Subscription } from 'rxjs';
import { OAuthService } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';
@Component({
    selector: 'app-sub-navbar',
    templateUrl: './sub-navbar.component.html',
    styleUrls: ['./sub-navbar.component.scss'],
})
export class SubNavbarComponent implements OnInit, OnDestroy {
    user: IUser | any = null;
    bodyElement!: HTMLElement;
    userSubscription!: Subscription;
    fontSizes = ['x-small', 'small', 'medium', 'large', 'larger'];
    currentFontSize = 'medium';
    removedStyleElements: Element[] = [];
    constructor(
        private oAuthService: OAuthService,
        private authService: AuthService,
        private userService: UserService,
        private renderer: Renderer2,
        private elementRef: ElementRef,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.userSubscription = this.userService.userSubject.subscribe(() => this.addUserDetails());
        this.addUserDetails();

        const savedFontSize = localStorage.getItem('fontSize');
        if (savedFontSize && this.fontSizes.includes(savedFontSize)) {
            this.currentFontSize = savedFontSize;
            this.renderer.setStyle(document.documentElement, 'font-size', this.currentFontSize);
        }
    }

    ngOnDestroy(): void {
        this.userSubscription.unsubscribe();
    }

    addUserDetails() {
        const fetchData = this.userService.getUser();
        if (fetchData.name) {
            this.user = fetchData;
            sessionStorage.setItem('user', JSON.stringify(this.user));
            return;
        }
        const userData = sessionStorage.getItem('user');
        if (userData) {
            this.user = JSON.parse(userData);
            return;
        }
        // Logging out user is invalid AND session is active
        if (this.oAuthService.hasValidAccessToken()) {
            this.logout();
        }
    }

    removeAllStyles() {
        const styleElements = document.querySelectorAll('style');
        styleElements.forEach(style => {
            this.removedStyleElements.push(style);
            style.remove();
        });
        const linkElements = document.querySelectorAll('link[rel="stylesheet"]');
        linkElements.forEach(link => {
            this.removedStyleElements.push(link);
            link.remove();
        });
    }

    restoreAllStyles() {
        this.removedStyleElements.forEach(element => {
            document.head.appendChild(element);
        });
        this.removedStyleElements = [];
    }

    logout() {
        sessionStorage.removeItem('isLoggedIn');
        sessionStorage.removeItem('user');
        this.user = null;
        this.authService.removeTokenData();
        this.oAuthService.logOut();
    }

    memorizePage() {
        // if user was at for example /login/mobile-eid before logging in, the service should NOT remember where he was
        if (!this.router.url.includes('login')) {
            this.oAuthService.redirectUri = this.router.url;
        }
    }

    isUserLogged() {
        const token = this.oAuthService.hasValidAccessToken();
        return token;
    }

    decreaseFontSize() {
        this.changeFontSize(false);
    }

    increaseFontSize() {
        this.changeFontSize(true);
    }

    defaultFontSize() {
        this.currentFontSize = 'medium';
        this.renderer.setStyle(document.documentElement, 'font-size', this.currentFontSize);
        localStorage.setItem('fontSize', this.currentFontSize);
    }

    changeFontSize(increase: boolean) {
        const currentIndex = this.fontSizes.indexOf(this.currentFontSize);
        if (increase && currentIndex < this.fontSizes.length - 1) {
            this.currentFontSize = this.fontSizes[currentIndex + 1];
        } else if (!increase && currentIndex > 0) {
            this.currentFontSize = this.fontSizes[currentIndex - 1];
        }
        this.renderer.setStyle(document.documentElement, 'font-size', this.currentFontSize);
        localStorage.setItem('fontSize', this.currentFontSize);
    }

    @HostListener('document:click', ['$event'])
    clickOutside(event: Event) {
        const targetElement = event.target as HTMLElement;

        const isClickedInside = this.elementRef.nativeElement.contains(targetElement);
        if (!isClickedInside) {
            // Close the collapsed element here
            const collapsedElement = this.elementRef.nativeElement.querySelector('.navbar-collapse.show');
            if (collapsedElement) {
                collapsedElement.classList.remove('show');
            }
        }
    }
}
