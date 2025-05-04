import { Component, HostListener, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {BookingCreate} from "../../model/BookingCreate";
import {SeatType} from "../../model/seat-type.enum";
import {PassengerType} from "../../model/passenger-type.enum";
import {HttpClient} from "@angular/common/http";
import {BookingService} from "../../service/booking.service";

@Component({
  selector: 'app-booking-view',
  templateUrl: './booking-view.component.html',
  styleUrls: ['./booking-view.component.css']
})
export class BookingViewComponent {
  reservationForm: FormGroup;
  private offerId: string = '';
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private http: HttpClient,
    private bookingService: BookingService
  ) {
    this.reservationForm = this.fb.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      passenger_type: ['', Validators.required],
      seat_type: ['', Validators.required]
    });

    this.route.paramMap.subscribe(params => {
      this.offerId = params.get('id') || '';
    });
  }

  goToPayment() {
    if (this.reservationForm.invalid) return;

    const form = this.reservationForm.value;

    const booking: BookingCreate = {
      OfferId: this.offerId,
      SeatNumber: '10', //to remove
      SeatType: form.seat_type,
      PassengerName: `${form.name} ${form.surname}`,
      PassengerType: form.passenger_type
    };

    this.bookingService.createBooking(booking).subscribe({
      next: (bookingId) => {
        this.router.navigate(['/payment', bookingId]);
      },
      error: (err) => {
        console.error('Error creating booking:', err);
      }
    });
  }

}
