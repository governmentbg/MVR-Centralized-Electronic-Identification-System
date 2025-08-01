import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmpowermentCheckSearchResultsComponent } from './empowerment-check-search-results.component';

describe('AuthorizationReferenceSearchResultsComponent', () => {
    let component: EmpowermentCheckSearchResultsComponent;
    let fixture: ComponentFixture<EmpowermentCheckSearchResultsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentCheckSearchResultsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentCheckSearchResultsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
