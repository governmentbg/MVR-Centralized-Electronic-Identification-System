import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router, ActivationEnd } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { HttpCancelService } from '../services/httpcancel.service';

@Injectable()
export class ManageHttpInterceptor implements HttpInterceptor {
    constructor(router: Router, private httpCancelService: HttpCancelService) {
        router.events.subscribe(event => {
            if (event instanceof ActivationEnd) {
                this.httpCancelService.cancelPendingRequests();
            }
        });
    }

    intercept<T>(req: HttpRequest<T>, next: HttpHandler): Observable<HttpEvent<T>> {
        return next.handle(req).pipe(takeUntil(this.httpCancelService.onCancelPendingRequests()));
    }
}
