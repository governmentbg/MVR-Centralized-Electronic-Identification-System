import { TestBed } from '@angular/core/testing';

import { LandingPagesService } from './landing-pages.service';

describe('LandingPagesService', () => {
    let service: LandingPagesService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(LandingPagesService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
