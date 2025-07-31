import { TestBed } from '@angular/core/testing';

import { RedirectToJournalGuard } from './redirect-to-journal.guard';

describe('RedirectToJournalGuard', () => {
    let guard: RedirectToJournalGuard;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(RedirectToJournalGuard);
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });
});
