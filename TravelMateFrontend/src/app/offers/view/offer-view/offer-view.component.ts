import { Component, OnInit, OnDestroy } from '@angular/core';
import { Offer } from '../../model/Offer';
import { OffersService } from '../../service/offers.service';
import { ActivatedRoute, Router } from '@angular/router';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-offer-view',
  templateUrl: './offer-view.component.html',
  styleUrls: ['./offer-view.component.css']
})
export class OfferViewComponent implements OnInit, OnDestroy {
  offer!: Offer | null;
  private offerId!: string;
  private hubConnection!: signalR.HubConnection;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private offersService: OffersService
  ) {}

  isPastOffer(offer: Offer): boolean {
    const today = new Date();
    const offerDate = new Date(offer.departureTime);
    return offerDate < today;
  }

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.offerId = params['id'];
      this.loadOffer();
    });

    this.initSignalR();
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection
        .invoke('LeaveOfferGroup', this.offerId)
        .then(() => console.log(`Left group for offer ${this.offerId}`))
        .catch((err) => console.error('Error leaving group:', err));
    }
  }

  private loadOffer() {
    this.offersService.getOfferById(this.offerId).subscribe((offer: Offer) => {
      this.offer = offer;
    });
  }

  private initSignalR() {
    this.hubConnection
      .start()
      .then(() => {
        this.hubConnection
          .invoke('JoinOfferGroup', this.offerId)
          .then(() => console.log(`Joined group for offer ${this.offerId}`))
          .catch((err) => console.error('Error joining group:', err));
      })
      .catch((err) => console.error('SignalR connection error:', err));

    this.hubConnection.on('OfferUpdated', (change: { oldOffer: Offer; newOffer: Offer }) => {
      if (change.newOffer.id === this.offerId) {
        this.offer = change.newOffer;
        console.log('Offer was automatically updated.');
      }
    });

    this.hubConnection.on('OfferDeleted', (deletedId: string) => {
      if (deletedId === this.offerId) {
        console.warn('This offer was deleted. Redirecting...');
        this.router.navigate(['/offers']);
      }
    });

    this.hubConnection.on('OfferPurchased',()=>{
        console.log("someone purchased this offer");
      });
  }
}
