import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpowermentCheckNavbarComponent } from './empowerment-check-navbar.component';

describe('NavbarComponent', () => {
    let component: EmpowermentCheckNavbarComponent;
    let fixture: ComponentFixture<EmpowermentCheckNavbarComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentCheckNavbarComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentCheckNavbarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
