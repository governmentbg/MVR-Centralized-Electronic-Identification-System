import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogsViewerMainComponent } from './logs-viewer-main.component';

describe('LogsViewerMainComponent', () => {
    let component: LogsViewerMainComponent;
    let fixture: ComponentFixture<LogsViewerMainComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LogsViewerMainComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LogsViewerMainComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
