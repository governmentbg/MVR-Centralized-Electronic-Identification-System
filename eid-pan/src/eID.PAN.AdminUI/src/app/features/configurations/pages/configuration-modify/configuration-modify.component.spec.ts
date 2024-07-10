import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationModifyComponent } from './configuration-modify.component';

describe('ConfigurationModifyComponent', () => {
    let component: ConfigurationModifyComponent;
    let fixture: ComponentFixture<ConfigurationModifyComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ConfigurationModifyComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(ConfigurationModifyComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
