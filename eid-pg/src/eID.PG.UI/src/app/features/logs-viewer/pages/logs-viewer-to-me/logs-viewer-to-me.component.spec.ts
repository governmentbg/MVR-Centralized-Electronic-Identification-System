import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogsViewerToMeComponent } from './logs-viewer-to-me.component';

describe('LogsViewerToMeComponent', () => {
    let component: LogsViewerToMeComponent;
    let fixture: ComponentFixture<LogsViewerToMeComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LogsViewerToMeComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LogsViewerToMeComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
