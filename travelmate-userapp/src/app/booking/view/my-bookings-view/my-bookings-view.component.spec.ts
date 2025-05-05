import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyBookingsViewComponent } from './my-bookings-view.component';

describe('MyBookingsViewComponent', () => {
  let component: MyBookingsViewComponent;
  let fixture: ComponentFixture<MyBookingsViewComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MyBookingsViewComponent],
    });
    fixture = TestBed.createComponent(MyBookingsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
