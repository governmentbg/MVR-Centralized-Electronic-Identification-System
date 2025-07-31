import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpowermentCheckSearchResultDetailsComponent } from './empowerment-check-search-result-details.component';

describe('AuthorizationReferenceSearchResultDetailsComponent', () => {
    let component: EmpowermentCheckSearchResultDetailsComponent;
    let fixture: ComponentFixture<EmpowermentCheckSearchResultDetailsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentCheckSearchResultDetailsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentCheckSearchResultDetailsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
