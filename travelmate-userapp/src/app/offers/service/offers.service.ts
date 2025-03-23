import { Injectable } from '@angular/core';
import {Offer} from "../model/Offer";

@Injectable({
  providedIn: 'root'
})
export class OffersService {

  constructor() { }

  private offers: Offer[] = [
    { offer_id: 1, flight_number: 'LOT 503', plane: 'Boeing 737-800', departure: 'WWA', destination: 'TRN', departure_date: '2025-04-01T08:00:00', arrival_date: '2025-04-01T12:00:00', base_price: 500, airline: 'Ryanair', available_economy_seats: 4, available_business_seats: 3, available_first_class_seats: 2 },
    { offer_id: 2, flight_number: 'LOT 123', plane: 'Boeing 737-800', departure: 'WWA', destination: 'OSR', departure_date: '2025-04-02T10:00:00', arrival_date: '2025-04-02T18:00:00', base_price: 650, airline: 'Wizzair', available_economy_seats: 10, available_business_seats: 7, available_first_class_seats: 2 }
  ];

  getAllOffers(): Offer[] {
    return this.offers;
  }

  getOfferById(id: number): Offer | null {
    return this.offers.find(offer => offer.offer_id === id) || null;
  }
}
