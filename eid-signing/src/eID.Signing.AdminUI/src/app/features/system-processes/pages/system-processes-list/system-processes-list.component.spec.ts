import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SystemProcessesListComponent } from './system-processes-list.component';

describe('SystemProcessesListComponent', () => {
    let component: SystemProcessesListComponent;
    let fixture: ComponentFixture<SystemProcessesListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [SystemProcessesListComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(SystemProcessesListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
