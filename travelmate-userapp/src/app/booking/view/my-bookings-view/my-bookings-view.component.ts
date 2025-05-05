import {Component, OnInit} from '@angular/core';
import {Bookings} from "../../model/Bookings";
import {BookingService} from "../../service/booking.service";

@Component({
  selector: 'app-my-bookings-view',
  templateUrl: './my-bookings-view.component.html',
  styleUrls: ['./my-bookings-view.component.css']
})
export class MyBookingsViewComponent implements OnInit{
  bookings: Bookings[] = [];
  pageBooking: number = 1;

  constructor(private bookingService: BookingService) {}

  ngOnInit() {
    this.loadOffers();
  }

  loadOffers() {
    // this.bookingService.getBookingsByUser().subscribe((bookings: Bookings[]) => {
    //   this.bookings = bookings;
    // });
  }
}
