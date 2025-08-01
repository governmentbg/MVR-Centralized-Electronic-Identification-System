import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthPersonalDocumentComponent } from './auth-personal-document.component';

describe('AuthPersonalDocumentComponent', () => {
    let component: AuthPersonalDocumentComponent;
    let fixture: ComponentFixture<AuthPersonalDocumentComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuthPersonalDocumentComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuthPersonalDocumentComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
