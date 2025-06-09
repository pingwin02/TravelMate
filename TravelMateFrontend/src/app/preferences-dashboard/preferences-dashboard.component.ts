import { Component, OnInit, NgZone } from '@angular/core';
import {
  DestinationPreferenceService,
  DestinationPreference,
  EnumCount
} from './service/destination-preferences.service';
import * as signalR from '@microsoft/signalr';

interface CountryCount {
  name: string;
  count: number;
}

@Component({
  selector: 'app-preferences-dashboard',
  templateUrl: './preferences-dashboard.component.html',
  styleUrls: ['./preferences-dashboard.component.css']
})
export class PreferencesDashboardComponent implements OnInit {
  preferences: DestinationPreference[] = [];
  seatTypeCounts: EnumCount[] = [];
  passengerTypeCounts: EnumCount[] = [];
  loading = true;
  error: string | null = null;
  hubConnection!: signalR.HubConnection;

  constructor(
    private preferencesService: DestinationPreferenceService,
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    this.preferencesService.getDestinationPreferences().subscribe({
      next: (data) => {
        this.preferences = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load preferences';
        this.loading = false;
      }
    });

    this.preferencesService.getOfferPreferences().subscribe({
      next: (data) => {
        this.seatTypeCounts = data.seatTypeCounts;
        this.passengerTypeCounts = data.passengerTypeCounts;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load preferences';
        this.loading = false;
      }
    });

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/rezerwacje/preferencesHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((err) => console.error('SignalR error:', err));

    this.hubConnection.on('ReceiveDestinationPreferencesUpdate', (data) => {
      this.ngZone.run(() => {
        this.preferences = Array.isArray(data.result) ? data.result : [];
      });
    });

    this.hubConnection.on('ReceiveOfferPreferencesUpdate', (data) => {
      this.ngZone.run(() => {
        this.seatTypeCounts = data.result.seatTypeCounts;
        this.passengerTypeCounts = data.result.passengerTypeCounts;
      });
    });
  }

  getPercentage(count: number): number {
    const max = Math.max(...this.preferences.map((p) => p.count), 0);
    return max > 0 ? (count / max) * 100 : 0;
  }

  trackById(index: number, item: DestinationPreference): string {
    return item.id;
  }

  getTotalCountries(): number {
    return new Set(this.preferences.map((p) => p.country)).size;
  }

  getTotalDestinations(): number {
    return this.preferences.length;
  }

  getTotalBookings(): number {
    return this.preferences.reduce((sum, pref) => sum + pref.count, 0);
  }

  getMaxCount(arr: EnumCount[]): number {
    return arr.length ? Math.max(...arr.map((x) => x.count)) : 1;
  }

  getCountryData(): CountryCount[] {
    const countryMap = this.preferences.reduce(
      (acc, curr) => {
        acc[curr.country] = (acc[curr.country] || 0) + curr.count;
        return acc;
      },
      {} as Record<string, number>
    );

    return Object.entries(countryMap).map(([name, count]) => ({
      name,
      count
    }));
  }

  sortedCountries(): CountryCount[] {
    return this.getCountryData().sort((a, b) => b.count - a.count);
  }

  getCountryPercentage(count: number): number {
    const countries = this.getCountryData();
    if (countries.length === 0) return 0;
    const max = Math.max(...countries.map((c) => c.count));
    return max > 0 ? (count / max) * 100 : 0;
  }

  getTopCountry(): { country: string; count: number } {
    const countries = this.getCountryData();
    if (countries.length === 0) return { country: '', count: 0 };
    const topCountry = countries[0];
    return { country: topCountry.name, count: topCountry.count };
  }
}
