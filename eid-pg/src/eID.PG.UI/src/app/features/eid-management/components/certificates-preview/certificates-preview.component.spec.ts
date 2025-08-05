import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CertificatesPreviewComponent } from './certificates-preview.component';

describe('CertificatesPreviewComponent', () => {
    let component: CertificatesPreviewComponent;
    let fixture: ComponentFixture<CertificatesPreviewComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CertificatesPreviewComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CertificatesPreviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
