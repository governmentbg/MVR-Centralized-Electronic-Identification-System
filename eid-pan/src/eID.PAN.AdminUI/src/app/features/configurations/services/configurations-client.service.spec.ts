import { TestBed } from '@angular/core/testing';

import { ConfigurationsClientService } from './configurations-client.service';

describe('ConfigurationsClientService', () => {
    let service: ConfigurationsClientService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(ConfigurationsClientService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
