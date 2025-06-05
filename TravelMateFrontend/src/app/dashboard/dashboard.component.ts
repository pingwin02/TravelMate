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
      .withUrl('/ofertyqueries/offerHub')
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

    const compareObjects = (oldObj: any, newObj: any, prefix = '') => {
      const keys = new Set([...Object.keys(oldObj || {}), ...Object.keys(newObj || {})]);

      keys.forEach((key) => {
        const fullKey = prefix ? `${prefix}.${key}` : key;
        const oldVal = oldObj?.[key];
        const newVal = newObj?.[key];

        const isDateField = /Time|Date|createdAt|updatedAt/i.test(key);
        if (isDateField) {
          const oldDate = oldVal ? new Date(oldVal).toLocaleString() : undefined;
          const newDate = newVal ? new Date(newVal).toLocaleString() : undefined;
          if (oldDate !== newDate) {
            diffs.push({
              field: fullKey,
              oldValue: oldDate,
              newValue: newDate
            });
          }
        } else if (typeof oldVal === 'object' && oldVal !== null && typeof newVal === 'object' && newVal !== null) {
          compareObjects(oldVal, newVal, fullKey);
        } else if (oldVal !== newVal) {
          diffs.push({
            field: fullKey,
            oldValue: oldVal,
            newValue: newVal
          });
        }
      });
    };

    compareObjects(change.oldOffer, change.newOffer);

    return diffs;
  }
}
