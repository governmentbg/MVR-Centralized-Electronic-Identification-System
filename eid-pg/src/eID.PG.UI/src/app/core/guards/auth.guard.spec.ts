import { TestBed } from '@angular/core/testing';
import { authGuard } from './auth.guard';
import { CanActivateFn } from '@angular/router';

describe('AuthGuard', () => {
    let guard: CanActivateFn;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(authGuard);
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });
});
