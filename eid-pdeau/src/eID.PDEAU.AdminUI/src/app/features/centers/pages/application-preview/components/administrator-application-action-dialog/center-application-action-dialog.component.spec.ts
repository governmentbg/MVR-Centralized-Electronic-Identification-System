import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CenterApplicationActionDialogComponent } from './center-application-action-dialog.component';

describe('AdministratorApplicationActionDialogComponent', () => {
    let component: CenterApplicationActionDialogComponent;
    let fixture: ComponentFixture<CenterApplicationActionDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CenterApplicationActionDialogComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CenterApplicationActionDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
