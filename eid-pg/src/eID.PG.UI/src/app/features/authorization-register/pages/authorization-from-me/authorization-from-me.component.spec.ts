import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationFromMeComponent } from './authorization-from-me.component';

describe('AuthorizationFromMeComponent', () => {
    let component: AuthorizationFromMeComponent;
    let fixture: ComponentFixture<AuthorizationFromMeComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuthorizationFromMeComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuthorizationFromMeComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
