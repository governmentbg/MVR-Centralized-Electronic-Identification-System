import { TestBed } from '@angular/core/testing';
import { RedirectToSearchPageGuard } from './redirect-to-search-page.guard';

describe('RedirectToSearchPageGuard', () => {
    let guard: RedirectToSearchPageGuard;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(RedirectToSearchPageGuard);
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });
});
