import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {HttpClientModule} from "@angular/common/http";
import { OffersViewComponent } from './offers/view/offers-view/offers-view.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { OfferViewComponent } from './offers/view/offer-view/offer-view.component';
import { LoginViewComponent } from './users/view/login-view/login-view.component';
import { BookingViewComponent } from './booking/view/booking-view/booking-view.component';

@NgModule({
  declarations: [
    AppComponent,
    OffersViewComponent,
    OfferViewComponent,
    LoginViewComponent,
    BookingViewComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    NgxPaginationModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
