import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CenterStatusActionDialogComponent } from './center-status-action-dialog.component';

describe('CenterStatusActionDialogComponent', () => {
    let component: CenterStatusActionDialogComponent;
    let fixture: ComponentFixture<CenterStatusActionDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CenterStatusActionDialogComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CenterStatusActionDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
