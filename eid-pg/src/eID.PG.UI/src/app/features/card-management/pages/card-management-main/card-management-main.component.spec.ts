import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardManagementMainComponent } from './card-management-main.component';

describe('CardManagementMainComponent', () => {
    let component: CardManagementMainComponent;
    let fixture: ComponentFixture<CardManagementMainComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [CardManagementMainComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CardManagementMainComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
