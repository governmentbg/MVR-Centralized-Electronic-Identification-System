import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentSigningFormComponent } from './document-signing-form.component';

describe('DocumentSigningFormComponent', () => {
    let component: DocumentSigningFormComponent;
    let fixture: ComponentFixture<DocumentSigningFormComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [DocumentSigningFormComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(DocumentSigningFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
