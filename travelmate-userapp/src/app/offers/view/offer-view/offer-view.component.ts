import { Component, OnInit } from '@angular/core';
import { Offer } from '../../model/Offer';
import { OffersService } from '../../service/offers.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-offer-view',
  templateUrl: './offer-view.component.html',
  styleUrls: ['./offer-view.component.css'],
})
export class OfferViewComponent implements OnInit {
  offer!: Offer | null;

  constructor(
    private route: ActivatedRoute,
    private offersService: OffersService,
  ) {}

  isPastOffer(offer: Offer): boolean {
    const today = new Date();
    const offerDate = new Date(offer.departureTime);
    return offerDate < today;
  }

  ngOnInit() {
    //const offerId = Number(this.route.snapshot.paramMap.get('id'));
    this.route.params.subscribe((params) => {
      this.offersService.getOfferById(params['id']).subscribe((offer: Offer) => {
        this.offer = offer;
      });
    });
  }
}
