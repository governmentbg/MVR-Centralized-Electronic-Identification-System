import { TestBed } from '@angular/core/testing';

import { ProvidedServicesService } from './provided-services.service';

describe('ProvidedServicesService', () => {
  let service: ProvidedServicesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProvidedServicesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
