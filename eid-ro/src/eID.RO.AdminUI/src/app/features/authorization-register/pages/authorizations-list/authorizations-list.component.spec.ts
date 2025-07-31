import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationsListComponent } from './authorizations-list.component';

describe('AuthorizationsListComponent', () => {
    let component: AuthorizationsListComponent;
    let fixture: ComponentFixture<AuthorizationsListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuthorizationsListComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuthorizationsListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
