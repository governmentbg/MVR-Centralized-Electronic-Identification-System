import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministratorApplicationActionDialogComponent } from './administrator-application-action-dialog.component';

describe('AdministratorApplicationActionDialogComponent', () => {
  let component: AdministratorApplicationActionDialogComponent;
  let fixture: ComponentFixture<AdministratorApplicationActionDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdministratorApplicationActionDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdministratorApplicationActionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
