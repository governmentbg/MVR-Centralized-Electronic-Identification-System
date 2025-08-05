import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EidReferencesComponent } from './eid-references.component';

describe('EidReferencesComponent', () => {
    let component: EidReferencesComponent;
    let fixture: ComponentFixture<EidReferencesComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ EidReferencesComponent ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(EidReferencesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
