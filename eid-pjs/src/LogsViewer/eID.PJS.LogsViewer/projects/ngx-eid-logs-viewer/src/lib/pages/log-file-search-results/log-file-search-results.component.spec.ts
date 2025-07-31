import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogFileSearchResultsComponent } from './log-file-search-results.component';

describe('LogFileSearchResultsComponent', () => {
    let component: LogFileSearchResultsComponent;
    let fixture: ComponentFixture<LogFileSearchResultsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LogFileSearchResultsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LogFileSearchResultsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
