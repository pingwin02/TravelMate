import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DeparturePreference {
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
export class DeparturePreferencesService {
  private apiUrl = '/rezerwacje/Preferences';

  constructor(private http: HttpClient) {}

  getDeparturePreferences(): Observable<DeparturePreference[]> {
    return this.http.get<DeparturePreference[]>(this.apiUrl + '/departure-preferences');
  }

  getOfferPreferences(): Observable<OfferPreferencesSummary> {
    return this.http.get<OfferPreferencesSummary>(this.apiUrl + '/offer-preferences');
  }
}