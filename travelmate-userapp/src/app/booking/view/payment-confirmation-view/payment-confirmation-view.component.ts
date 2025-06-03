import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentService } from '../../service/payment.service';
import { BookingService } from '../../service/booking.service';
import { BookingStatus } from '../../model/booking-status.enum';
import { Payment } from '../../model/Payment';

@Component({
  selector: 'app-payment-confirmation-view',
  templateUrl: './payment-confirmation-view.component.html',
  styleUrls: ['./payment-confirmation-view.component.css'],
})
export class PaymentConfirmationViewComponent implements OnInit {
  private paymentId: string | null;
  payment!: Payment;
  isCanceled: boolean | null = null;
  isConfirmed: boolean | null = null;
  paymentText = '';
  paymentResult: boolean | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private paymentService: PaymentService,
    private bookingService: BookingService,
  ) {
    this.paymentId = this.route.snapshot.paramMap.get('id');
  }

  ngOnInit(): void {
    if (this.paymentId) {
      this.paymentService.getPaymentById(this.paymentId).subscribe({
        next: (payment) => {
          this.payment = payment;

          this.bookingService.getBookingById(this.payment.bookingId).subscribe({
            next: (booking) => {
              if (booking.status === BookingStatus.Canceled) {
                this.isCanceled = true;
              } else if (booking.status === BookingStatus.Confirmed) {
                this.isConfirmed = true;
              } else {
                this.isConfirmed = false;
                this.isCanceled = false;

                this.paymentService.payPayment(this.payment.id).subscribe((result) => {
                  this.paymentResult = result.success;
                  this.paymentText = result.message;
                });
              }
            },
          });
        },
      });
    }
  }
}
