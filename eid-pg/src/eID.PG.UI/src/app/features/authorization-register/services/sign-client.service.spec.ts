import { TestBed } from '@angular/core/testing';

import { SignClientService } from './sign-client.service';

describe('SignService', () => {
    let service: SignClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(SignClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
