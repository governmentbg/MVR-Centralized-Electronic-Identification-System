import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CertificatesFilterComponent } from './certificates-filter.component';

describe('CertificatesFilterComponent', () => {
    let component: CertificatesFilterComponent;
    let fixture: ComponentFixture<CertificatesFilterComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CertificatesFilterComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CertificatesFilterComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
