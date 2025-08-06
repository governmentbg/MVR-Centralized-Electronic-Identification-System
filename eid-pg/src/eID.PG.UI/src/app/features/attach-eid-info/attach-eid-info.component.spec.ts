import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttachEidInfoComponent } from './attach-eid-info.component';

describe('AttachEidInfoComponent', () => {
    let component: AttachEidInfoComponent;
    let fixture: ComponentFixture<AttachEidInfoComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ AttachEidInfoComponent ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(AttachEidInfoComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
