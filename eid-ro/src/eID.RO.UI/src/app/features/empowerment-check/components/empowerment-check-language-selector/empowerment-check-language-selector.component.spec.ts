import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmpowermentCheckLanguageSelectorComponent } from './empowerment-check-language-selector.component';

describe('EmpowermentCheckLanguageSelectorComponent', () => {
    let component: EmpowermentCheckLanguageSelectorComponent;
    let fixture: ComponentFixture<EmpowermentCheckLanguageSelectorComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [EmpowermentCheckLanguageSelectorComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EmpowermentCheckLanguageSelectorComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
