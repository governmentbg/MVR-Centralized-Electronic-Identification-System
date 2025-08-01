import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CertificateHistoryComponent } from './certificate-history.component';

describe('CertificateHistoryComponent', () => {
    let component: CertificateHistoryComponent;
    let fixture: ComponentFixture<CertificateHistoryComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CertificateHistoryComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CertificateHistoryComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
