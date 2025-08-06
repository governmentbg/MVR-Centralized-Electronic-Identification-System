import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EidManagementComponent } from './eid-management.component';

describe('EidManagementComponent', () => {
    let component: EidManagementComponent;
    let fixture: ComponentFixture<EidManagementComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EidManagementComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EidManagementComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
