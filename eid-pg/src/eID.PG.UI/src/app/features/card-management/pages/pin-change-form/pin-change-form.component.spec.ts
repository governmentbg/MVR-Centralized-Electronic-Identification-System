import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PinChangeFormComponent } from './pin-change-form.component';

describe('PinChangeFormComponent', () => {
    let component: PinChangeFormComponent;
    let fixture: ComponentFixture<PinChangeFormComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [PinChangeFormComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(PinChangeFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
