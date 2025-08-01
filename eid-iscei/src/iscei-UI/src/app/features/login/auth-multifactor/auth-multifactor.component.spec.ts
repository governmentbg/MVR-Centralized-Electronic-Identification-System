import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthMultifactorComponent } from './auth-multifactor.component';

describe('AuthMultifactorComponent', () => {
    let component: AuthMultifactorComponent;
    let fixture: ComponentFixture<AuthMultifactorComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ AuthMultifactorComponent ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(AuthMultifactorComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
