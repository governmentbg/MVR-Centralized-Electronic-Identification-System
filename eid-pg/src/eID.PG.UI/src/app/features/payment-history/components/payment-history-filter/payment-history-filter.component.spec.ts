import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentHistoryFilterComponent } from './payment-history-filter.component';

describe('PaymentHistoryFilterComponent', () => {
    let component: PaymentHistoryFilterComponent;
    let fixture: ComponentFixture<PaymentHistoryFilterComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ PaymentHistoryFilterComponent ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(PaymentHistoryFilterComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
