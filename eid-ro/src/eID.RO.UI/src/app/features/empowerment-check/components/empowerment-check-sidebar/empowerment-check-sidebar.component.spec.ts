import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpowermentCheckSidebarComponent } from './empowerment-check-sidebar.component';

describe('SidebarComponent', () => {
    let component: EmpowermentCheckSidebarComponent;
    let fixture: ComponentFixture<EmpowermentCheckSidebarComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentCheckSidebarComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentCheckSidebarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
