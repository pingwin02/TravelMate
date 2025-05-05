import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Booking} from "../../model/Booking";
import {BookingService} from "../../service/booking.service";
import {BookingStatus} from "../../model/booking-status.enum";
import {SeatTypeLabels} from "../../model/seat-type.enum";
import {PassengerTypeLabels} from "../../model/passenger-type.enum";
import {PaymentService} from "../../service/payment.service";
import {Payment} from "../../model/Payment";
import {Offer} from "../../../offers/model/Offer";
import {OffersService} from "../../../offers/service/offers.service";

@Component({
  selector: 'app-payment-view',
  templateUrl: './payment-view.component.html',
  styleUrls: ['./payment-view.component.css']
})
export class PaymentViewComponent implements OnInit, OnDestroy {
  booking!: Booking;
  offer!: Offer;
  payment!: Payment;
  timeLeft: number = 0;
  timerInterval: any;
  private bookingId: string | null;
  isCanceled: boolean = false;
  isConfirmed: boolean = false;
  seatTypeLabels = SeatTypeLabels
  passengerTypeLabels = PassengerTypeLabels

  constructor(
    private route: ActivatedRoute,
    private bookingService: BookingService,
    private paymentService: PaymentService,
    private offersService: OffersService,
    private router: Router
  ) {
    this.bookingId = this.route.snapshot.paramMap.get('id');
  }

  ngOnInit(): void {
    if (this.bookingId) {
      this.bookingService.getBookingById(this.bookingId).subscribe({
        next: (booking) => {
          this.booking = booking;
          this.offersService.getOfferById(this.booking.offerId).subscribe((offer : Offer) => {
            this.offer = offer;
            if (this.booking.status === BookingStatus.Canceled) {
              this.isCanceled = true;
            } else if (this.booking.status === BookingStatus.Confirmed) {
              this.isConfirmed = true;
            } else {
              this.isCanceled = false;
              this.isCanceled = false;
              this.paymentService.getPaymentById(this.booking.paymentId!).subscribe({
                next: (payment) => {
                  this.payment = payment
                }
              });
              this.initTimer();
            }
          })
        },
        error: (err) => {
          // console.error(err);
          this.router.navigate(['/offers']);
        }
      });
    }
  }

  initTimer(): void {
    if (!this.booking.reservedUntil) {
      return;
    }

    const expiry = new Date(this.booking.reservedUntil).getTime();
    const correctedExpiry = expiry + (2 * 60 * 60 * 1000);
    this.timeLeft = Math.floor((correctedExpiry - Date.now()) / 1000);

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
    this.router.navigate(['/payment-confirmation', this.booking.paymentId]);
  }

  ngOnDestroy(): void {
    clearInterval(this.timerInterval);
  }
}
