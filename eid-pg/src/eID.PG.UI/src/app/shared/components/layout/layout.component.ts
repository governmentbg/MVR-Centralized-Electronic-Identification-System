import { Component, OnInit } from '@angular/core';
import { ToastService } from '../../services/toast.service';

@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss'],
    providers: [ToastService],
})
export class LayoutComponent implements OnInit {
    cookie!: string;

    ngOnInit(): void {
        this.cookie = this.getCookie('cookiePrivacyAccept');
    }

    getCookie(cookieName: string) {
        const name = cookieName + '=';
        const ca = document.cookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return '';
    }

    setCookie(cname: string, cvalue: boolean, exdays: number) {
        const d = new Date();
        //cookie expiration date can be controlled here
        d.setTime(d.getTime() + exdays * 24 * 60 * 60 * 1000);
        const expires = 'expires=' + d.toUTCString();
        document.cookie = cname + '=' + cvalue + ';' + expires + ';path=/';
        this.cookie = this.getCookie(cname);
    }
}
