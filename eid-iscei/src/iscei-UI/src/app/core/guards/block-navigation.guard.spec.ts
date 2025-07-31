import { TestBed } from '@angular/core/testing';

import { blockNavigationGuard } from './block-navigation.guard';
import { Observable } from 'rxjs';

describe('BlockNavigationGuard', () => {
    let guard: Observable<boolean>;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        guard = TestBed.inject(blockNavigationGuard);
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });
});
