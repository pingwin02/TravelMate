import { Component} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {BookingCreate} from "../../model/BookingCreate";
import {HttpClient} from "@angular/common/http";
import {BookingService} from "../../service/booking.service";
import {Offer} from "../../../offers/model/Offer";
import {OffersService} from "../../../offers/service/offers.service";

@Component({
  selector: 'app-booking-view',
  templateUrl: './booking-view.component.html',
  styleUrls: ['./booking-view.component.css']
})
export class BookingViewComponent {
  reservationForm: FormGroup;
  offer!: Offer | null;
  private offerId: string = '';
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private bookingService: BookingService,
    private offersService: OffersService
  ) {
    this.reservationForm = this.fb.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      passenger_type: ['', Validators.required],
      seat_type: ['', Validators.required]
    });

    this.route.paramMap.subscribe(params => {
      this.offerId = params.get('id') || '';
      this.offersService.getOfferById( this.offerId).subscribe((offer : Offer) => {
        this.offer =offer;
      })
    });
  }

  goToPayment() {
    if (this.reservationForm.invalid) return;

    const form = this.reservationForm.value;

    const booking: BookingCreate = {
      OfferId: this.offerId,
      SeatType: +form.seat_type,
      PassengerName: `${form.name} ${form.surname}`,
      PassengerType: +form.passenger_type
    };
    this.bookingService.createBooking(booking).subscribe({
      next: (createdBooking) => {
        const bookingId = createdBooking.id;
        this.router.navigate(['/payment', bookingId]);
      },
      error: (err) => {
        console.error('Error creating booking:', err);
      }
    });
  }

}
