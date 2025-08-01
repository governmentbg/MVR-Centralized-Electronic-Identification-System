import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CenterPreviewComponent } from './center-preview.component';

describe('CenterPreviewComponent', () => {
    let component: CenterPreviewComponent;
    let fixture: ComponentFixture<CenterPreviewComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CenterPreviewComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CenterPreviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
