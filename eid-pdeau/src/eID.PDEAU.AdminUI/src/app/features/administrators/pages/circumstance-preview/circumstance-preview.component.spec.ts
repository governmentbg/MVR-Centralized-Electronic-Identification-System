import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CircumstancePreviewComponent } from './circumstance-preview.component';

describe('CircumstancePreviewComponent', () => {
    let component: CircumstancePreviewComponent;
    let fixture: ComponentFixture<CircumstancePreviewComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CircumstancePreviewComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CircumstancePreviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
