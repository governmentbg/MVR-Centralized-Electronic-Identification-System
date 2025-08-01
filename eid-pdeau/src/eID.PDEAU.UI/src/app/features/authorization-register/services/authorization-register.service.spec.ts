import { TestBed } from '@angular/core/testing';

import { AuthorizationRegisterService } from './authorization-register.service';

describe('AuthorizationRegisterService', () => {
    let service: AuthorizationRegisterService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(AuthorizationRegisterService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
