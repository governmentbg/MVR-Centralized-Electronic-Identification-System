import { TestBed } from '@angular/core/testing';
import { AuthInterceptor } from '@app/core/interceptors/http-requests.interceptor';

describe('HttpRequestsInterceptor', () => {
    beforeEach(() =>
        TestBed.configureTestingModule({
            providers: [AuthInterceptor],
        })
    );

    it('should be created', () => {
        const interceptor: AuthInterceptor = TestBed.inject(AuthInterceptor);
        expect(interceptor).toBeTruthy();
    });
});
