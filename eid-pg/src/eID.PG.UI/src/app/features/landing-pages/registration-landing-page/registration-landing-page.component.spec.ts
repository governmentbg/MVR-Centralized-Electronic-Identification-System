import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistrationLandingPageComponent } from './registration-landing-page.component';

describe('RegistrationLandingPageComponent', () => {
    let component: RegistrationLandingPageComponent;
    let fixture: ComponentFixture<RegistrationLandingPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [RegistrationLandingPageComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(RegistrationLandingPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
