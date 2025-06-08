import { Component, OnInit, NgZone, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { DeparturePreferencesService, DeparturePreference } from './service/departure-preferences.service';
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

  preferences: DeparturePreference[] = [];
  loading = true;
  error: string | null = null;
  hubConnection!: signalR.HubConnection;
  
  constructor(
    private preferencesService: DeparturePreferencesService,
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    this.preferencesService.getDeparturePreferences().subscribe({
      next: (data) => {
        this.preferences = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load preferences';
        this.loading = false;
      }
    });

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/rezerwacje/preferencesHub') 
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch((err) => console.error('SignalR error:', err));

    this.hubConnection.on('ReceivePreferencesUpdate', (data) => {
      this.ngZone.run(() => {
        this.preferences = Array.isArray(data.result) ? data.result : [];
      });
    });
  }




  getPercentage(count: number): number {
    const max = Math.max(...this.preferences.map(p => p.count));
    return max > 0 ? (count / max) * 100 : 0;
  }

  trackById(index: number, item: DeparturePreference): string {
    return item.id;
  }

  getTotalCountries(): number {
    const uniqueCountries = new Set(this.preferences.map(p => p.country));
    return uniqueCountries.size;
  }

  getTotalDestinations(): number {
    return this.preferences.length;
  }

  getTotalBookings(): number {
    return this.preferences.reduce((sum, pref) => sum + pref.count, 0);
  }

  getTopDestination(): DeparturePreference {
    if (this.preferences.length === 0) return {} as DeparturePreference;
    return [...this.preferences].sort((a, b) => b.count - a.count)[0];
  }

  sortedPreferences(): DeparturePreference[] {
    return [...this.preferences].sort((a, b) => b.count - a.count);
  }

  getCountryData(): CountryCount[] {
    // Group by country and sum the counts
    const countryMap = this.preferences.reduce((acc, curr) => {
      if (!acc[curr.country]) {
        acc[curr.country] = 0;
      }
      acc[curr.country] += curr.count;
      return acc;
    }, {} as Record<string, number>);
    
    // Convert to array of objects
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
    
    const max = Math.max(...countries.map(c => c.count));
    return max > 0 ? (count / max) * 100 : 0;
  }

  getTopCountry(): { country: string, count: number } {
    const countries = this.getCountryData();
    if (countries.length === 0) return { country: '', count: 0 };
    
    const topCountry = countries.sort((a, b) => b.count - a.count)[0];
    return { country: topCountry.name, count: topCountry.count };
  }
}