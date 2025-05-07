import { Component, OnInit } from '@angular/core';
import { OfferFilter } from '../../model/OfferFilter';
import { OffersService } from '../../service/offers.service';
import { OfferList } from '../../model/OfferList';

@Component({
  selector: 'app-offers-view',
  templateUrl: './offers-view.component.html',
  styleUrls: ['./offers-view.component.css'],
})
export class OffersViewComponent implements OnInit {
  private offers: OfferList[] = [];
  filteredOffers: OfferList[] = [];
  filter: OfferFilter = {
    departure: '',
    arrival: '',
    departure_date: '',
    arrival_date: '',
    airline: '',
    arrivalCity: '',
    departureCity: '',
  };
  pageOffer: number = 1;
  loading = false;
  constructor(private offersService: OffersService) {}

  ngOnInit() {
    this.loadOffers();
  }

  loadOffers() {
    this.loading = true;
    this.offersService.getAllOffers().subscribe({
      next: (data: OfferList[]) => {
        this.offers = data.sort(
          (a, b) =>
            new Date(a.departureTime).getTime() -
            new Date(b.departureTime).getTime(),
        );
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  applyFilters() {
    this.filteredOffers = this.offers.filter((offer) => {
      return (
        (!this.filter.airline ||
          offer.airlineName
            .toLowerCase()
            .includes(this.filter.airline.toLowerCase())) &&
        (!this.filter.departure ||
          offer.departureAirport
            .toLowerCase()
            .includes(this.filter.departure.toLowerCase())) &&
        (!this.filter.arrival ||
          offer.arrivalAirport
            .toLowerCase()
            .includes(this.filter.arrival.toLowerCase())) &&
        (!this.filter.departure_date ||
          new Date(offer.departureTime) >=
            new Date(this.filter.departure_date)) &&
        (!this.filter.arrival_date ||
          new Date(offer.arrivalTime) <= new Date(this.filter.arrival_date)) &&
        (!this.filter.departureCity ||
          offer.departureCity
            .toLowerCase()
            .includes(this.filter.departureCity.toLowerCase())) &&
        (!this.filter.arrivalCity ||
          offer.arrivalCity
            .toLowerCase()
            .includes(this.filter.arrivalCity.toLowerCase())) &&
        new Date(offer.departureTime) > new Date()
      );
    });
  }

  onFilterChange() {
    this.applyFilters();
  }
}
