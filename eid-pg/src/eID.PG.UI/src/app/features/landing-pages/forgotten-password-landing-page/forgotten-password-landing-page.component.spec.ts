import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgottenPasswordLandingPageComponent } from './forgotten-password-landing-page.component';

describe('ForgottenPasswordLandingPageComponent', () => {
    let component: ForgottenPasswordLandingPageComponent;
    let fixture: ComponentFixture<ForgottenPasswordLandingPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ForgottenPasswordLandingPageComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(ForgottenPasswordLandingPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
