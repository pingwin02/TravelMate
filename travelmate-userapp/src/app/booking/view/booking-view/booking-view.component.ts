import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingCreate } from '../../model/BookingCreate';
import { HttpClient } from '@angular/common/http';
import { BookingService } from '../../service/booking.service';
import { Offer } from '../../../offers/model/Offer';
import { OffersService } from '../../../offers/service/offers.service';
import { AuthService } from 'src/app/auth/service/auth.service';
declare var bootstrap: any;
@Component({
  selector: 'app-booking-view',
  templateUrl: './booking-view.component.html',
  styleUrls: ['./booking-view.component.css'],
})
export class BookingViewComponent {
  reservationForm: FormGroup;
  offer!: Offer | null;
  loading = false;
  private offerId: string = '';
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private bookingService: BookingService,
    private offersService: OffersService,
    private authService: AuthService,
  ) {
    this.reservationForm = this.fb.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      passenger_type: ['', Validators.required],
      seat_type: ['', Validators.required],
    });

    this.route.paramMap.subscribe((params) => {
      this.offerId = params.get('id') || '';
      this.offersService
        .getOfferById(this.offerId)
        .subscribe((offer: Offer) => {
          this.offer = offer;
        });
    });
  }

  goToPayment() {
    if (this.reservationForm.invalid) return;
    this.loading = true;
    const form = this.reservationForm.value;

    const booking: BookingCreate = {
      OfferId: this.offerId,
      SeatType: +form.seat_type,
      PassengerName: `${form.name} ${form.surname}`,
      PassengerType: +form.passenger_type,
    };
    this.bookingService.createBooking(booking).subscribe({
      next: (createdBooking) => {
        const bookingId = createdBooking.id;
        this.pollForPaymentId(bookingId);
      },
      error: (err) => {
        this.loading = false;
        console.error('Error creating booking:', err);

        if (err.status === 400) {
          this.showNoSeatsModal();
        } else if (err.status === 401) {
          this.authService.logout();
        }
      },
    });
  }

  showNoSeatsModal() {
    const modalElement = document.getElementById('noSeatsModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }
  showPaymentTimeoutModal() {
    const modalElement = document.getElementById('paymentTimeoutModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  pollForPaymentId(bookingId: string) {
    const time = 1000;
    const maxAttempts = 10;
    let attempts = 0;

    const polling = setInterval(() => {
      this.bookingService.getBookingById(bookingId).subscribe({
        next: (booking) => {
          if (booking.paymentId) {
            clearInterval(polling);
            this.loading = false;
            this.router.navigate(['/payment', bookingId]);
          } else {
            attempts++;
            if (attempts >= maxAttempts) {
              clearInterval(polling);
              this.loading = false;
              this.showPaymentTimeoutModal();
            }
          }
        },
        error: (err) => {
          clearInterval(polling);
          this.loading = false;
          console.error('Error polling for paymentId:', err);
        },
      });
    }, time);
  }
}
