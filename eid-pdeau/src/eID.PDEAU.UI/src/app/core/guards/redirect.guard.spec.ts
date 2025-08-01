import { TestBed } from '@angular/core/testing';

import { redirectGuard } from './redirect.guard';

describe('RedirectGuard', () => {
    let guard: () => void;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(redirectGuard);
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });
});
