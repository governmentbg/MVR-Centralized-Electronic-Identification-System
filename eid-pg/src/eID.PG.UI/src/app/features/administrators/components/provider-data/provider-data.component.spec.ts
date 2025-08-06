import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProviderDataComponent } from './provider-data.component';

describe('ProviderDataComponent', () => {
    let component: ProviderDataComponent;
    let fixture: ComponentFixture<ProviderDataComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ProviderDataComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(ProviderDataComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
