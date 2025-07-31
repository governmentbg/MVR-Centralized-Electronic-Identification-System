import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterFormPreviewComponent } from './register-form-preview.component';

describe('RegisterFormPreviewComponent', () => {
    let component: RegisterFormPreviewComponent;
    let fixture: ComponentFixture<RegisterFormPreviewComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [RegisterFormPreviewComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(RegisterFormPreviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
