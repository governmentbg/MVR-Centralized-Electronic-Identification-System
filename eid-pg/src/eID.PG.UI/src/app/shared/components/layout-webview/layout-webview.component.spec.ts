import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayoutWebviewComponent } from './layout-webview.component';

describe('LayoutWebviewComponent', () => {
    let component: LayoutWebviewComponent;
    let fixture: ComponentFixture<LayoutWebviewComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LayoutWebviewComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LayoutWebviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
