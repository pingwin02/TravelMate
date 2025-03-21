import {Component, OnInit} from '@angular/core';
import {Offer} from "../../model/Offer";
import {OfferFilter} from "../../model/OfferFilter";

@Component({
  selector: 'app-offers-view',
  templateUrl: './offers-view.component.html',
  styleUrls: ['./offers-view.component.css']
})
export class OffersViewComponent implements OnInit {
  offers: Offer[] = [];
  filteredOffers: Offer[] = [];
  filter: OfferFilter = {
    departure: '',
    destination: '',
    departure_date: '',
    arrival_date: '',
    airline: '',
  };
  page: number = 1;

  constructor() {}

  ngOnInit() {
    this.loadOffers();
  }

  loadOffers() {
    this.offers = [
      { offer_id: 1, flight_number: 'LOT 503', plane: 'Boeing 737-800',
        departure: 'WWA', destination: 'TRN', departure_date: '2025-04-01T08:00:00', arrival_date: '2025-04-01T12:00:00', base_price: 500,  airline: 'Ryanair',
        available_economy_seats: 4, available_business_seats: 3, available_first_class_seats: 2},
      { offer_id: 2, flight_number: 'LOT 123', plane: 'Boeing 737-800',
        departure: 'WWA', destination: 'OSR', departure_date: '2025-04-02T10:00:00', arrival_date: '2025-04-02T18:00:00', base_price: 650,  airline: 'Wizzair',
        available_economy_seats: 10, available_business_seats: 7, available_first_class_seats: 2}
    ];
    this.applyFilters();
  }

  applyFilters() {
    this.filteredOffers = this.offers.filter(offer => {
      return (
        (!this.filter.airline || offer.airline.toLowerCase().includes(this.filter.airline.toLowerCase())) &&
        (!this.filter.departure || offer.departure.toLowerCase().includes(this.filter.departure.toLowerCase())) &&
        (!this.filter.destination || offer.destination.toLowerCase().includes(this.filter.destination.toLowerCase())) &&
        (!this.filter.departure_date || new Date(offer.departure_date) >= new Date(this.filter.departure_date)) &&
        (!this.filter.arrival_date || new Date(offer.arrival_date) <= new Date(this.filter.arrival_date))
      );
    });
  }

  onFilterChange() {
    this.applyFilters();
  }
}
