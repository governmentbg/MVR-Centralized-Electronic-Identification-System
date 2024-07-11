import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpowermentCheckSearchComponent } from './empowerment-check-search.component';

describe('AuthorizationReferenceSearchComponent', () => {
    let component: EmpowermentCheckSearchComponent;
    let fixture: ComponentFixture<EmpowermentCheckSearchComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentCheckSearchComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentCheckSearchComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
