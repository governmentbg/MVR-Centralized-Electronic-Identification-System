import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpowermentsFilterComponent } from './empowerments-filter.component';

describe('TableFilterComponent', () => {
    let component: EmpowermentsFilterComponent;
    let fixture: ComponentFixture<EmpowermentsFilterComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentsFilterComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentsFilterComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
