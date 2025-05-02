import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {Bookings} from "../model/Bookings";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  constructor(private http: HttpClient) { }

  /* get user bookings */
  getBookingsByUser(): Observable<Bookings[]>{
    return this.http.get<Bookings[]>( '/bookings/Bookings');
  }
}
