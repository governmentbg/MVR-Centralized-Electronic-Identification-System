import { TestBed } from '@angular/core/testing';

import { RedirectToSearchFormGuard } from './redirect-to-search-form.guard';

describe('RedirectToSearchFormGuard', () => {
    let guard: RedirectToSearchFormGuard;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(RedirectToSearchFormGuard);
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });
});
