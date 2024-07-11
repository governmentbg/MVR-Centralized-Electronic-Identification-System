import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationReferenceSearchResultsComponent } from './authorization-reference-search-results.component';

describe('AuthorizationReferenceSearchResultsComponent', () => {
    let component: AuthorizationReferenceSearchResultsComponent;
    let fixture: ComponentFixture<AuthorizationReferenceSearchResultsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuthorizationReferenceSearchResultsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuthorizationReferenceSearchResultsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
