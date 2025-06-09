import { TestBed } from '@angular/core/testing';

import { DestinationPreferenceService } from './destination-preferences.service';

describe('DestinationPreferenceService', () => {
  let service: DestinationPreferenceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DestinationPreferenceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
