import {Component, OnInit} from '@angular/core';
import {BookingService} from "../../service/booking.service";
import {OffersService} from "../../../offers/service/offers.service";
import {Offer} from "../../../offers/model/Offer";
import {Booking} from "../../model/Booking";
import {SeatTypeLabels} from "../../model/seat-type.enum";
import {PassengerTypeLabels} from "../../model/passenger-type.enum";
import {BookingStatus} from "../../model/booking-status.enum";

@Component({
  selector: 'app-my-bookings-view',
  templateUrl: './my-bookings-view.component.html',
  styleUrls: ['./my-bookings-view.component.css']
})
export class MyBookingsViewComponent implements OnInit {
  bookings: Booking[] = [];
  offersMap: Map<string, Offer> = new Map();

  seatTypeLabels = SeatTypeLabels;
  passengerTypeLabels = PassengerTypeLabels;
  bookingStatusLabels: Record<BookingStatus, string> = {
    [BookingStatus.Pending]: 'Pending',
    [BookingStatus.Confirmed]: 'Confirmed',
    [BookingStatus.Canceled]: 'Canceled'
  };

  pageBooking: number = 1;

  constructor(private bookingService: BookingService, private offersService: OffersService) {}

  ngOnInit() {
    this.loadOffers();
  }

  loadOffers() {
    this.bookingService.getBookingsByUser().subscribe((bookings: Booking[]) => {
      this.bookings = bookings;

      const offerIds = Array.from(new Set(bookings.map(b => b.offerId)));

      offerIds.forEach(id => {
        this.offersService.getOfferById(id).subscribe((offer: Offer) => {
          this.offersMap.set(id, offer);
        });
      });
    });
  }

  getOffer(offerId: string): Offer | undefined {
    return this.offersMap.get(offerId);
  }
}
