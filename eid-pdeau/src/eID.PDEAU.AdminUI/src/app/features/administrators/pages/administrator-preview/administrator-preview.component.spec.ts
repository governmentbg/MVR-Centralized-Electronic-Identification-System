import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministratorPreviewComponent } from './administrator-preview.component';

describe('AdministratorPreviewComponent', () => {
    let component: AdministratorPreviewComponent;
    let fixture: ComponentFixture<AdministratorPreviewComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AdministratorPreviewComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AdministratorPreviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
