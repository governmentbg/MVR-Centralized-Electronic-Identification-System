import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
    constructor(private router: Router, private oAuthService: OAuthService) {}

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(request).pipe(
            catchError((err: HttpErrorResponse) => {
                if (err.status === 401) {
                    this.oAuthService.redirectUri = this.router.url;
                    this.oAuthService.logOut();
                }

                return throwError(() => err.error);
            })
        );
    }
}
