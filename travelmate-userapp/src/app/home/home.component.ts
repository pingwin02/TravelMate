import { Component } from '@angular/core';
import { Router } from '@angular/router';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  offerImage = 'https://st3.depositphotos.com/1105977/32206/i/450/depositphotos_322069858-stock-photo-passengers-commercial-airplane-flying-above.jpg';
  bookingImage = 'https://st2.depositphotos.com/1177973/8097/i/450/depositphotos_80977878-stock-photo-woman-using-laptop-to-book.jpg';

  constructor(private router: Router) {}
  
    goToOffers() {
      this.router.navigate(['/offers']);
    }

    goToBookings() {
      this.router.navigate(['/my-bookings']);
  }
}
