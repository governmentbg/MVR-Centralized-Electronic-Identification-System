import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationsListComponent } from './configurations-list.component';

describe('ConfigurationsListComponent', () => {
    let component: ConfigurationsListComponent;
    let fixture: ComponentFixture<ConfigurationsListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ConfigurationsListComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(ConfigurationsListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
