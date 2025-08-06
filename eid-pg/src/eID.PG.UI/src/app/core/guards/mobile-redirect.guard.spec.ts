import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';
import { mobileRedirectGuard } from './mobile-redirect.guard';

describe('mobileRedirectGuard', () => {
    let guard: CanActivateFn;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(mobileRedirectGuard);
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });
});
