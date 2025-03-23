import {Component, OnInit} from '@angular/core';
import {Offer} from "../../model/Offer";
import {OfferFilter} from "../../model/OfferFilter";
import {OffersService} from "../../service/offers.service";

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

  constructor(private offersService: OffersService) {}

  ngOnInit() {
    this.loadOffers();
  }

  loadOffers() {
    this.offers = this.offersService.getAllOffers();
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
