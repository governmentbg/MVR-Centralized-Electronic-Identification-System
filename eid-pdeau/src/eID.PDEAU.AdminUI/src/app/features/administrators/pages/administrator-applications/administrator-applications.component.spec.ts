import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministratorApplicationsComponent } from './administrator-applications.component';

describe('AdministratorApplicationsComponent', () => {
  let component: AdministratorApplicationsComponent;
  let fixture: ComponentFixture<AdministratorApplicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdministratorApplicationsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdministratorApplicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
