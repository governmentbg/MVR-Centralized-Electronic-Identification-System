import { TestBed } from '@angular/core/testing';

import { EmpowermentXmlParserService } from './empowerment-xml-parser.service';

describe('EmpowermentXmlParserService', () => {
    let service: EmpowermentXmlParserService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(EmpowermentXmlParserService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
