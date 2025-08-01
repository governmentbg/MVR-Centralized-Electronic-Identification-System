import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdministrationLoginComponent } from './administration-login.component';

describe('AdministrationLoginComponent', () => {
    let component: AdministrationLoginComponent;
    let fixture: ComponentFixture<AdministrationLoginComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AdministrationLoginComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AdministrationLoginComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
