import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';

@Component({
    selector: 'app-layout-webview',
    templateUrl: './layout-webview.component.html',
    styleUrls: ['./layout-webview.component.scss'],
})
export class LayoutWebviewComponent implements OnInit {
    currentUrl = '/mobile/home';
    homeUrl = '/mobile/home';
    urlsWithoutBackButton = ['useful-information', 'contacts'];
    constructor(
        private location: Location,
        private router: Router,
        private translocoService: TranslocoService,
        private route: ActivatedRoute
    ) {
        this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(event => {
            const navEndEvent = event as NavigationEnd;
            setTimeout(() => {
                window.scrollTo(0, 0);
            });
            this.currentUrl = navEndEvent.urlAfterRedirects.split('?')[0];
        });
    }

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            const lang = params['lang'];
            if (lang) {
                this.translocoService.setActiveLang(lang);
            }
        });
    }

    onBack(): void {
        if (window.history.length > 1) {
            this.location.back();
        } else {
            this.router.navigateByUrl(this.homeUrl);
        }
    }

    isBackButtonVisible() {
        const isHome = this.currentUrl === this.homeUrl;
        const isInHiddenList = this.urlsWithoutBackButton.some(url => this.currentUrl.includes(url));
        return !isHome && !isInHiddenList;
    }
}
