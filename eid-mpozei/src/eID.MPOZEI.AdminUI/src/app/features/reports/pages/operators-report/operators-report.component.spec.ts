import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorsReportComponent } from './operators-report.component';

describe('OperatorsReportComponent', () => {
    let component: OperatorsReportComponent;
    let fixture: ComponentFixture<OperatorsReportComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [OperatorsReportComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(OperatorsReportComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
