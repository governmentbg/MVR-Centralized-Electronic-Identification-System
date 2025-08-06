import { TestBed } from '@angular/core/testing';

import { RaeiceiClientService } from './raeicei-client.service';

describe('RaeiceiClientService', () => {
    let service: RaeiceiClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(RaeiceiClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
