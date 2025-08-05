import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewAuthorizationFormComponent } from './new-authorization-form.component';

describe('NewAuthorizationFormComponent', () => {
    let component: NewAuthorizationFormComponent;
    let fixture: ComponentFixture<NewAuthorizationFormComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [NewAuthorizationFormComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(NewAuthorizationFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
