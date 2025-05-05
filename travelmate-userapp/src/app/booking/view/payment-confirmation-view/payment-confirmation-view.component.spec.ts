import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentConfirmationViewComponent } from './payment-confirmation-view.component';

describe('PaymentConfirmationViewComponent', () => {
  let component: PaymentConfirmationViewComponent;
  let fixture: ComponentFixture<PaymentConfirmationViewComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PaymentConfirmationViewComponent],
    });
    fixture = TestBed.createComponent(PaymentConfirmationViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
