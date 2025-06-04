import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Bookings } from '../model/Bookings';
import { HttpClient } from '@angular/common/http';
import { BookingCreate } from '../model/BookingCreate';
import { Booking } from '../model/Booking';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  constructor(private http: HttpClient) {}

  /* get user bookings */
  getBookingsByUser(): Observable<Booking[]> {
    return this.http.get<Booking[]>('/rezerwacje/Bookings');
  }

  /* create a booking */
  createBooking(booking: BookingCreate): Observable<any> {
    return this.http.post('/rezerwacje/Bookings/create', booking);
  }

  /* get booking by id */
  getBookingById(id: string): Observable<Booking> {
    return this.http.get<Booking>(`/rezerwacje/Bookings/${id}`);
  }
}
