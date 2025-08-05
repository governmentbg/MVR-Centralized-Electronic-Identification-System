import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationToMeComponent } from './authorization-to-me.component';

describe('AuthorizationToMeComponent', () => {
    let component: AuthorizationToMeComponent;
    let fixture: ComponentFixture<AuthorizationToMeComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuthorizationToMeComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuthorizationToMeComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
