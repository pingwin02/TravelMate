import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Booking} from "../../model/Booking";
import {BookingService} from "../../service/booking.service";

@Component({
  selector: 'app-payment-view',
  templateUrl: './payment-view.component.html',
  styleUrls: ['./payment-view.component.css']
})
export class PaymentViewComponent implements OnInit, OnDestroy {
  booking!: Booking;
  timeLeft: number = 0;
  timerInterval: any;
  private bookingId: string | null;

  constructor(
    private route: ActivatedRoute,
    private bookingService: BookingService,
    private router: Router
  ) {
    this.bookingId = this.route.snapshot.paramMap.get('id');
  }

  ngOnInit(): void {
    if (this.bookingId) {
      this.bookingService.getBookingById(this.bookingId).subscribe({
        next: (booking) => {
          this.booking = booking;
          console.log('Booking object:', booking);
          this.initTimer();
        },
        error: (err) => {
          // console.error(err);
          this.router.navigate(['/offers']);
        }
      });
    }
  }

  initTimer(): void {
    if (!this.booking.ReservedUntil) {
      return;
    }

    const expiry = new Date(this.booking.ReservedUntil).getTime();
    const correctedExpiry = expiry + (2 * 60 * 60 * 1000);
    this.timeLeft = Math.floor((correctedExpiry - Date.now()) / 1000);
    console.log('Time:', this.timeLeft);

    this.timerInterval = setInterval(() => {
      if (this.timeLeft > 0) {
        this.timeLeft--;
      } else {
        clearInterval(this.timerInterval);
        // alert('Time expired. Redirecting...');
        // this.router.navigate(['/offers']);
      }
    }, 1000);
  }

  payNow(): void {
    // if (this.booking.paymentUrl) {
    //   window.location.href = this.booking.paymentUrl;
    // } else {
    //   alert('Payment URL not available.');
    // }
    this.router.navigate(['/payment-confirmation', this.bookingId]);
  }

  ngOnDestroy(): void {
    clearInterval(this.timerInterval);
  }

}
