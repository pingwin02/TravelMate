import { TestBed } from '@angular/core/testing';

import { DeparturePreferencesService } from './departure-preferences.service';

describe('DeparturePreferencesService', () => {
  let service: DeparturePreferencesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeparturePreferencesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
