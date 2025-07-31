import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CenterApplicationsComponent } from './center-applications.component';

describe('CenterApplicationsComponent', () => {
    let component: CenterApplicationsComponent;
    let fixture: ComponentFixture<CenterApplicationsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CenterApplicationsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CenterApplicationsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
