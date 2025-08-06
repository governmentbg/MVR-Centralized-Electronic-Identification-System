import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateEmailLandingPageComponent } from './update-email-landing-page.component';

describe('UpdateEmailLandingPageComponent', () => {
    let component: UpdateEmailLandingPageComponent;
    let fixture: ComponentFixture<UpdateEmailLandingPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [UpdateEmailLandingPageComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(UpdateEmailLandingPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
