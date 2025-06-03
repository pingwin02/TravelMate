import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { OffersViewComponent } from './offers/view/offers-view/offers-view.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { OfferViewComponent } from './offers/view/offer-view/offer-view.component';
import { LoginViewComponent } from './auth/view/login-view/login-view.component';
import { BookingViewComponent } from './booking/view/booking-view/booking-view.component';
import { PaymentViewComponent } from './booking/view/payment-view/payment-view.component';
import { TokenInterceptor } from './auth/token.interceptor';
import { MyBookingsViewComponent } from './booking/view/my-bookings-view/my-bookings-view.component';
import { PaymentConfirmationViewComponent } from './booking/view/payment-confirmation-view/payment-confirmation-view.component';
import { HomeComponent } from './home/home.component';
import { EuropeanTimePipe } from './pipes/european-time.pipe';
import { DashboardComponent } from './dashboard/dashboard.component';
@NgModule({
  declarations: [
    AppComponent,
    OffersViewComponent,
    OfferViewComponent,
    LoginViewComponent,
    BookingViewComponent,
    PaymentViewComponent,
    MyBookingsViewComponent,
    PaymentConfirmationViewComponent,
    HomeComponent,
    EuropeanTimePipe,
    DashboardComponent,
  ],
  imports: [BrowserModule, AppRoutingModule, HttpClientModule, FormsModule, NgxPaginationModule, ReactiveFormsModule],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
