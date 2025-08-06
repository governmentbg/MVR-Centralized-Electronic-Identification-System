import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationRegisterComponent } from './authorization-register.component';

describe('AuthorizationRegisterComponent', () => {
    let component: AuthorizationRegisterComponent;
    let fixture: ComponentFixture<AuthorizationRegisterComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuthorizationRegisterComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuthorizationRegisterComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
