import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuditLogsListComponent } from './audit-logs-list.component';

describe('AuditLogsListComponent', () => {
    let component: AuditLogsListComponent;
    let fixture: ComponentFixture<AuditLogsListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuditLogsListComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuditLogsListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
