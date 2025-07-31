import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private oauthService: OAuthService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const accessToken = this.oauthService.getAccessToken();

        if (accessToken) {
            const cloned = req.clone({
                headers: req.headers.set('Authorization', 'Bearer ' + accessToken),
            });
            return next.handle(cloned);
        } else {
            return next.handle(req);
        }
    }
}
