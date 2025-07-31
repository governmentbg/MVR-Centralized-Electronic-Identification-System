import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentTypesFormComponent } from './document-types-form.component';

describe('DocumentTypesFormComponent', () => {
    let component: DocumentTypesFormComponent;
    let fixture: ComponentFixture<DocumentTypesFormComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [DocumentTypesFormComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(DocumentTypesFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
