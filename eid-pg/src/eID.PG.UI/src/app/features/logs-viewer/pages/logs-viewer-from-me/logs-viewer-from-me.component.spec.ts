import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogsViewerFromMeComponent } from './logs-viewer-from-me.component';

describe('LogsViewerFromMeComponent', () => {
    let component: LogsViewerFromMeComponent;
    let fixture: ComponentFixture<LogsViewerFromMeComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LogsViewerFromMeComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LogsViewerFromMeComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
