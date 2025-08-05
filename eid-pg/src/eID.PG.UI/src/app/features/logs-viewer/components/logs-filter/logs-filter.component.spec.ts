import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogsFilterComponent } from './logs-filter.component';

describe('LogsFilterComponent', () => {
    let component: LogsFilterComponent;
    let fixture: ComponentFixture<LogsFilterComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LogsFilterComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LogsFilterComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
