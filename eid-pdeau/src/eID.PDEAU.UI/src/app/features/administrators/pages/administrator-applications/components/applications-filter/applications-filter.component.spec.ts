import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationsFilterComponent } from './applications-filter.component';

describe('ApplicationsFilterComponent', () => {
  let component: ApplicationsFilterComponent;
  let fixture: ComponentFixture<ApplicationsFilterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApplicationsFilterComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApplicationsFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
