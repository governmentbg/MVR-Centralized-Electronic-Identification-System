import { TestBed } from '@angular/core/testing';
import { ManageHttpInterceptor } from './managehttp.interceptor';

describe('ManageHttpInterceptor', () => {
    beforeEach(() =>
        TestBed.configureTestingModule({
            providers: [ManageHttpInterceptor],
        })
    );

    it('should be created', () => {
        const interceptor: ManageHttpInterceptor = TestBed.inject(ManageHttpInterceptor);
        expect(interceptor).toBeTruthy();
    });
});
