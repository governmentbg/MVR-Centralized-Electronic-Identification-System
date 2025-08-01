import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileEidLoginComponent } from './mobile-eid-login.component';

describe('MobileEidLoginComponent', () => {
    let component: MobileEidLoginComponent;
    let fixture: ComponentFixture<MobileEidLoginComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [MobileEidLoginComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(MobileEidLoginComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
