import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PreferencesDashboardComponent } from './preferences-dashboard.component';

describe('PreferencesDashboardComponent', () => {
  let component: PreferencesDashboardComponent;
  let fixture: ComponentFixture<PreferencesDashboardComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PreferencesDashboardComponent]
    });
    fixture = TestBed.createComponent(PreferencesDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
