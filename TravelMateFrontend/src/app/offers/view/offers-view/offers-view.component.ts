import { Component, OnInit, OnDestroy } from '@angular/core';
import { OfferFilter } from '../../model/OfferFilter';
import { OffersService } from '../../service/offers.service';
import { OfferList } from '../../model/OfferList';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-offers-view',
  templateUrl: './offers-view.component.html',
  styleUrls: ['./offers-view.component.css']
})
export class OffersViewComponent implements OnInit, OnDestroy {
  private offers: OfferList[] = [];
  filteredOffers: OfferList[] = [];
  filter: OfferFilter = {
    departure: '',
    arrival: '',
    departure_date: '',
    arrival_date: '',
    airline: '',
    arrivalCity: '',
    departureCity: ''
  };
  pageOffer = 1;
  loading = false;
  private hubConnection!: signalR.HubConnection;

  constructor(private offersService: OffersService) {}

  ngOnInit(): void {
    this.loadOffers();
    this.initSignalR();
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  loadOffers(): void {
    this.loading = true;
    this.offersService.getAllOffers().subscribe({
      next: (data: OfferList[]) => {
        this.offers = data.sort((a, b) => new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime());
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    const { airline, departure, arrival, departure_date, arrival_date, departureCity, arrivalCity } = this.filter;

    this.filteredOffers = this.offers.filter((offer) => {
      const depTime = new Date(offer.departureTime);
      const arrTime = new Date(offer.arrivalTime);

      return (
        (!airline || offer.airlineName.toLowerCase().includes(airline.toLowerCase())) &&
        (!departure || offer.departureAirport.toLowerCase().includes(departure.toLowerCase())) &&
        (!arrival || offer.arrivalAirport.toLowerCase().includes(arrival.toLowerCase())) &&
        (!departure_date || depTime >= new Date(departure_date)) &&
        (!arrival_date || arrTime <= new Date(arrival_date)) &&
        (!departureCity || offer.departureCity.toLowerCase().includes(departureCity.toLowerCase())) &&
        (!arrivalCity || offer.arrivalCity.toLowerCase().includes(arrivalCity.toLowerCase())) &&
        depTime > new Date()
      );
    });

    const maxPages = Math.ceil(this.filteredOffers.length / 10);
    if (this.pageOffer > maxPages) {
      this.pageOffer = 1;
    }
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  private initSignalR(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/ofertyqueries/offerChangesHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((err) => console.error('SignalR connection error:', err));

    this.hubConnection.on('OfferAdded', (fullOffer: any) => {
      const offer = mapFullOfferToOfferList(fullOffer);
      this.offers.unshift(offer);
      this.sortOffers();
      this.applyFilters();
      console.log('Offer added and list updated');
    });

    this.hubConnection.on('OfferUpdated', (change: { oldOffer: any; newOffer: any }) => {
      const updated = mapFullOfferToOfferList(change.newOffer);
      const index = this.offers.findIndex((o) => o.id === updated.id);
      if (index !== -1) {
        this.offers[index] = updated;
        this.sortOffers();
        this.applyFilters();
        console.log('Offer updated and list refreshed');
      }
    });

    this.hubConnection.on('OfferDeleted', (deletedId: string) => {
      this.offers = this.offers.filter((o) => o.id !== deletedId);
      this.applyFilters();
      console.log('Offer deleted and list refreshed');
    });
  }

  private sortOffers(): void {
    this.offers.sort((a, b) => new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime());
  }
}

function mapFullOfferToOfferList(full: any): OfferList {
  return {
    id: full.id,
    airlineName: full.airlineName || '',
    flightNumber: full.flightNumber || '',
    departureAirport: full.departureAirportCode || '',
    arrivalAirport: full.arrivalAirportCode || '',
    departureCity: full.departureAirportCity || '',
    arrivalCity: full.arrivalAirportCity || '',
    departureTime: full.departureTime,
    arrivalTime: full.arrivalTime,
    basePrice: full.basePrice
  };
}
