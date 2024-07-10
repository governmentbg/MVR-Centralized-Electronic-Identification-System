import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpowermentCheckBreadcrumbComponent } from './empowerments-check-breadcrumb.component';

describe('BreadcrumbComponent', () => {
    let component: EmpowermentCheckBreadcrumbComponent;
    let fixture: ComponentFixture<EmpowermentCheckBreadcrumbComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentCheckBreadcrumbComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentCheckBreadcrumbComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
