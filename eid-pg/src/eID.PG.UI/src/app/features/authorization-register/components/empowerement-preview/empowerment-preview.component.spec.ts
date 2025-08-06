import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpowermentPreviewComponent } from './empowerment-preview.component';

describe('EmpowermentPreviewComponent', () => {
    let component: EmpowermentPreviewComponent;
    let fixture: ComponentFixture<EmpowermentPreviewComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentPreviewComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentPreviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
