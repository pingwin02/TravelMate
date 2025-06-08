import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DeparturePreference {
  id: string;
  count: number;
  city: string;
  country: string;
}

@Injectable({
  providedIn: 'root'
})
export class DeparturePreferencesService {
  private apiUrl = '/rezerwacje/Preferences/departure-preferences';

  constructor(private http: HttpClient) {}

  getDeparturePreferences(): Observable<DeparturePreference[]> {
    return this.http.get<DeparturePreference[]>(this.apiUrl);
  }
}