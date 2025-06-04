import { Injectable } from '@angular/core';
import { Offer } from '../model/Offer';
import { OfferList } from '../model/OfferList';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class OffersService {
  constructor(private http: HttpClient) {}

  /* get offers list */
  getAllOffers(): Observable<OfferList[]> {
    return this.http.get<OfferList[]>('/oferty/Offers');
  }

  /* get offer by id */
  getOfferById(id: string): Observable<Offer> {
    return this.http.get<Offer>('/oferty/Offers/' + id);
  }
}
