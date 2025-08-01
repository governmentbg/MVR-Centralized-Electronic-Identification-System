import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministratorRegistrationsComponent } from './administrator-registrations.component';

describe('AdministratorRegistrationsComponent', () => {
    let component: AdministratorRegistrationsComponent;
    let fixture: ComponentFixture<AdministratorRegistrationsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AdministratorRegistrationsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AdministratorRegistrationsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
