import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilesJournalComponent } from './files-journal.component';

describe('FilesJournalComponent', () => {
    let component: FilesJournalComponent;
    let fixture: ComponentFixture<FilesJournalComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [FilesJournalComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(FilesJournalComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
