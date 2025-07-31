import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CenterRegistrationsComponent } from './center-registrations.component';

describe('CenterRegistrationsComponent', () => {
    let component: CenterRegistrationsComponent;
    let fixture: ComponentFixture<CenterRegistrationsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CenterRegistrationsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CenterRegistrationsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
