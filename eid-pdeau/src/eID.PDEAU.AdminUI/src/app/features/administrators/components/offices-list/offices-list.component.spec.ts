import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OfficesListComponent } from './offices-list.component';

describe('OfficesListComponent', () => {
  let component: OfficesListComponent;
  let fixture: ComponentFixture<OfficesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OfficesListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OfficesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
