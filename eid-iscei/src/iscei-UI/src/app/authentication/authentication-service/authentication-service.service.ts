import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, from, map, switchMap, throwError } from 'rxjs';
import { generateState, generateCodeVerifier, generateCodeChallenge } from '../validators/encryption-methods';
import { Router } from '@angular/router';
import { AppConfigService } from '../../core/services/config.service';
import { AssuranceTypes } from '../enums/authentication';
import { TranslocoService } from '@ngneat/transloco';
@Injectable({
    providedIn: 'root',
})
export class AuthenticationService {
    constructor(
        private http: HttpClient,
        private translocoService: TranslocoService,
        private router: Router,
        private appConfigService: AppConfigService
    ) {}

    private apiUrl = '/api/v1';

    getAuthChallenge(data: any) {
        return this.http.post<any>(`${this.apiUrl}/auth/generate-authentication-challenge`, data);
    }

    submitForm(data: any, clientId: string, scope?: string, system_id?: string) {
        let params = new HttpParams().set('clientId', clientId);
        if (scope && system_id) {
            params = params.set('scope', scope).set('system_id', system_id).set('clientId', clientId);
        }
        return this.http.post<any>(
            `${this.apiUrl}/approval-request/auth/citizen`,
            {
                type: data.type,
                citizenNumber: data.citizenNumber,
                levelOfAssurance: AssuranceTypes.HIGH,
                requestFrom: {
                    type: 'AUTH',
                    system: {
                        BG: this.translocoService.translate('global.txtMobileAuthDescription', {}, 'bg'),
                        EN: this.translocoService.translate('global.txtMobileAuthDescription', {}, 'en'),
                    },
                },
            },
            { params }
        );
    }

    getToken(data: any, clientId: string) {
        const params = new HttpParams().set('clientId', clientId);
        const headers = new HttpHeaders({
            'Content-Type': 'application/json',
        });
        return this.http.post<any>(`${this.apiUrl}/approval-request/token`, JSON.stringify({ auth_req_id: data }), {
            headers,
            params,
        });
    }

    codeFlowAuth(data: any, origin: string, client: string, scope?: string, system_id?: string): Observable<any> {
        const clientId = client;
        const redirectUri = window.location.origin;
        const state = generateState();
        const codeVerifier = generateCodeVerifier();

        return from(generateCodeChallenge(codeVerifier)).pipe(
            switchMap(codeChallenge => {
                let params = new HttpParams()
                    .set('client_id', clientId)
                    .set('response_type', 'code')
                    .set('state', state)
                    .set('redirect_uri', redirectUri)
                    .set('code_challenge', codeChallenge)
                    .set('code_challenge_method', 'S256');
                if (scope && system_id) {
                    params = params
                        .set('scope', scope)
                        .set('system_id', system_id)
                        .set('client_id', clientId)
                        .set('response_type', 'code')
                        .set('state', state)
                        .set('redirect_uri', redirectUri)
                        .set('code_challenge', codeChallenge)
                        .set('code_challenge_method', 'S256');
                }

                return this.http
                    .post(`${this.apiUrl}/code-flow/auth`, data, {
                        params,
                        observe: 'response',
                        responseType: 'text',
                    })
                    .pipe(
                        map((response: HttpResponse<any>) => {
                            localStorage.setItem('codeVerifier', codeVerifier);
                            if (response.url) {
                                const url = new URL(response.url);
                                const code = url.searchParams.get('code');
                                this.router.navigate(['login/personal-document-eid/auth'], {
                                    queryParams: { code: code, origin: origin, clientId: clientId },
                                });
                            }
                        })
                    );
            })
        );
    }

    getCodeFlowToken(code: string, clientId: string) {
        const redirectUri = window.location.origin;
        const codeVerifier = localStorage.getItem('codeVerifier');
        if (!codeVerifier) {
            return throwError(() => new Error('Code verifier not found'));
        }
        const params = new HttpParams()
            .set('grant_type', 'authorization_code')
            .set('code', code)
            .set('client_id', clientId)
            .set('redirect_uri', redirectUri)
            .set('code_verifier', codeVerifier);
        return this.http.get<any>(`${this.apiUrl}/code-flow/token`, { params });
    }

    submitOtpCode(code: string, sessionId: string){
        return this.http.post<any>(`${this.apiUrl}/auth/verify-otp`, {
            sessionId: sessionId,
            otp: code
        });
    }
}
