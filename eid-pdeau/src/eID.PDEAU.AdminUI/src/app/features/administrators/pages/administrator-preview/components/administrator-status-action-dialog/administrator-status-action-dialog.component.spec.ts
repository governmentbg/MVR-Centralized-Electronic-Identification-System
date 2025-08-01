import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministratorStatusActionDialogComponent } from './administrator-status-action-dialog.component';

describe('AdministratorStatusActionDialogComponent', () => {
    let component: AdministratorStatusActionDialogComponent;
    let fixture: ComponentFixture<AdministratorStatusActionDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AdministratorStatusActionDialogComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AdministratorStatusActionDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
