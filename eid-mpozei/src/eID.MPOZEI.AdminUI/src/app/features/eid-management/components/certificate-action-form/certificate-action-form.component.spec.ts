import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CertificateActionFormComponent } from './certificate-action-form.component';

describe('CertificateActionFormComponent', () => {
    let component: CertificateActionFormComponent;
    let fixture: ComponentFixture<CertificateActionFormComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CertificateActionFormComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CertificateActionFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
