import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuardiansFormComponent } from './guardians-form.component';

describe('GuardiansFormComponent', () => {
    let component: GuardiansFormComponent;
    let fixture: ComponentFixture<GuardiansFormComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [GuardiansFormComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(GuardiansFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
