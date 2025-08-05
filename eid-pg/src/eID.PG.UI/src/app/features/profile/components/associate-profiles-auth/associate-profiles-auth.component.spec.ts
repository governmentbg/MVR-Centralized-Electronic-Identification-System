import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssociateProfilesAuthComponent } from './associate-profiles-auth.component';

describe('AssociateProfilesAuthComponent', () => {
    let component: AssociateProfilesAuthComponent;
    let fixture: ComponentFixture<AssociateProfilesAuthComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AssociateProfilesAuthComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AssociateProfilesAuthComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
