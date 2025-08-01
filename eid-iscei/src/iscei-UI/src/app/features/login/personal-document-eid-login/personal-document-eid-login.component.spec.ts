import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonalDocumentEidLoginComponent } from './personal-document-eid-login.component';

describe('PersonalDocumentEidLoginComponent', () => {
    let component: PersonalDocumentEidLoginComponent;
    let fixture: ComponentFixture<PersonalDocumentEidLoginComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [PersonalDocumentEidLoginComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonalDocumentEidLoginComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
