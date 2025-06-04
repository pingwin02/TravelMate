import { Component, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';

interface OfferChange {
  type: 'added' | 'updated' | 'deleted';
  oldOffer?: any;
  newOffer?: any;
}

interface OfferDifference {
  field: string;
  oldValue: any;
  newValue: any;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  private hubConnection!: signalR.HubConnection;
  public recentChanges: OfferChange[] = [];

  ngOnInit(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/oferty/offerHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((err) => console.error('SignalR error:', err));

    this.hubConnection.on('OfferAdded', (offer) => {
      this.pushChange({ type: 'added', newOffer: offer });
    });

    this.hubConnection.on('OfferUpdated', (change: OfferChange) => {
      this.pushChange({
        type: 'updated',
        oldOffer: change.oldOffer,
        newOffer: change.newOffer
      });
    });

    this.hubConnection.on('OfferDeleted', (id) => {
      this.pushChange({ type: 'deleted', oldOffer: { id } });
    });
  }

  private pushChange(change: OfferChange) {
    console.log('New change received:', change);
    this.recentChanges.unshift(change);
    if (this.recentChanges.length > 10) {
      this.recentChanges.pop();
    }
  }

  public getDifferences(change: OfferChange): OfferDifference[] {
    if (!change.oldOffer || !change.newOffer) return [];

    const diffs: OfferDifference[] = [];
    const oldObj = change.oldOffer;
    const newObj = change.newOffer;

    const fieldsToCompare = [
      { key: 'flightNumber', label: 'Flight Number' },
      { key: 'departureTime', label: 'Departure Time' },
      { key: 'arrivalTime', label: 'Arrival Time' },
      { key: 'basePrice', label: 'Base Price' },
      { key: 'availableEconomySeats', label: 'Economy Seats' },
      { key: 'availableBusinessSeats', label: 'Business Seats' },
      { key: 'availableFirstClassSeats', label: 'First Class Seats' }
    ];

    for (const field of fieldsToCompare) {
      const oldVal = oldObj[field.key];
      const newVal = newObj[field.key];

      if (field.key === 'departureTime' || field.key === 'arrivalTime') {
        if (oldVal !== newVal) {
          diffs.push({
            field: field.label,
            oldValue: new Date(oldVal).toLocaleString(),
            newValue: new Date(newVal).toLocaleString()
          });
        }
      } else if (oldVal !== newVal) {
        diffs.push({
          field: field.label,
          oldValue: oldVal,
          newValue: newVal
        });
      }
    }

    return diffs;
  }
}
