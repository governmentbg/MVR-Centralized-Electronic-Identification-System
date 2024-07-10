import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationReferenceSearchResultDetailsComponent } from './authorization-reference-search-result-details.component';

describe('AuthorizationReferenceSearchResultDetailsComponent', () => {
    let component: AuthorizationReferenceSearchResultDetailsComponent;
    let fixture: ComponentFixture<AuthorizationReferenceSearchResultDetailsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AuthorizationReferenceSearchResultDetailsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AuthorizationReferenceSearchResultDetailsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
