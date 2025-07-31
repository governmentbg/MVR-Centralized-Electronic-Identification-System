import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServicesTariffsComponent } from './services-tariffs.component';

describe('ServicesTariffsComponent', () => {
  let component: ServicesTariffsComponent;
  let fixture: ComponentFixture<ServicesTariffsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ServicesTariffsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServicesTariffsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
