import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServicesTariffsFormComponent } from './services-tariffs-form.component';

describe('ServicesTariffsFormComponent', () => {
  let component: ServicesTariffsFormComponent;
  let fixture: ComponentFixture<ServicesTariffsFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ServicesTariffsFormComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServicesTariffsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
