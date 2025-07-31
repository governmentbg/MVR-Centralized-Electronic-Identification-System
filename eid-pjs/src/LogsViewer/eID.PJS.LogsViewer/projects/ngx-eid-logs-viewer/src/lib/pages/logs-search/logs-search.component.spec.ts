import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogsSearchComponent } from './logs-search.component';

describe('LogsSearchComponent', () => {
    let component: LogsSearchComponent;
    let fixture: ComponentFixture<LogsSearchComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LogsSearchComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LogsSearchComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
