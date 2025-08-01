import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LegalRepresentativesListComponent } from './legal-representatives-list.component';

describe('LegalRepresentativesListComponent', () => {
    let component: LegalRepresentativesListComponent;
    let fixture: ComponentFixture<LegalRepresentativesListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LegalRepresentativesListComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LegalRepresentativesListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
