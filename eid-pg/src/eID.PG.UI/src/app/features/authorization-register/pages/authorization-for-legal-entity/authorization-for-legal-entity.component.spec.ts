import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationForLegalEntityComponent } from './authorization-for-legal-entity.component';

describe('AuthorizationForLegalEntityComponent', () => {
    let component: AuthorizationForLegalEntityComponent;
    let fixture: ComponentFixture<AuthorizationForLegalEntityComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuthorizationForLegalEntityComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuthorizationForLegalEntityComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
