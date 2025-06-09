import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DestinationPreference {
  id: string;
  count: number;
  city: string;
  country: string;
}
export interface EnumCount {
  type: string;
  count: number;
}

export interface OfferPreferencesSummary {
  seatTypeCounts: EnumCount[];
  passengerTypeCounts: EnumCount[];
}
@Injectable({
  providedIn: 'root'
})
export class DestinationPreferenceService {
  private apiUrl = '/rezerwacje/Preferences';

  constructor(private http: HttpClient) {}

  getDestinationPreferences(): Observable<DestinationPreference[]> {
    return this.http.get<DestinationPreference[]>(this.apiUrl + '/destination-preferences');
  }

  getOfferPreferences(): Observable<OfferPreferencesSummary> {
    return this.http.get<OfferPreferencesSummary>(this.apiUrl + '/offer-preferences');
  }
}
