import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CertificatesHistoryComponent } from './certificates-history.component';

describe('CertificatesHistoryComponent', () => {
    let component: CertificatesHistoryComponent;
    let fixture: ComponentFixture<CertificatesHistoryComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CertificatesHistoryComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CertificatesHistoryComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
