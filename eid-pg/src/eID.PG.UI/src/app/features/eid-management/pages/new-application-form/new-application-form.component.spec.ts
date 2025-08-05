import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewApplicationFormComponent } from './new-application-form.component';

describe('NewApplicationFormComponent', () => {
    let component: NewApplicationFormComponent;
    let fixture: ComponentFixture<NewApplicationFormComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [NewApplicationFormComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(NewApplicationFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
