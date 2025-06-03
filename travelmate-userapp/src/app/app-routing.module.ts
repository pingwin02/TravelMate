import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OffersViewComponent } from './offers/view/offers-view/offers-view.component';
import { OfferViewComponent } from './offers/view/offer-view/offer-view.component';
import { LoginViewComponent } from './auth/view/login-view/login-view.component';
import { BookingViewComponent } from './booking/view/booking-view/booking-view.component';
import { PaymentViewComponent } from './booking/view/payment-view/payment-view.component';
import { authGuard } from './auth/auth.guard';
import { MyBookingsViewComponent } from './booking/view/my-bookings-view/my-bookings-view.component';
import { PaymentConfirmationViewComponent } from './booking/view/payment-confirmation-view/payment-confirmation-view.component';
import { HomeComponent } from './home/home.component';
import { DashboardComponent } from './dashboard/dashboard.component';
const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
  },
  {
    path: 'offers',
    component: OffersViewComponent,
  },
  {
    path: 'offer/:id',
    component: OfferViewComponent,
  },
  {
    path: 'login',
    component: LoginViewComponent,
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
  },
  {
    path: 'booking/:id',
    component: BookingViewComponent,
    canActivate: [authGuard],
  },
  {
    path: 'payment-confirmation/:id',
    component: PaymentConfirmationViewComponent,
    canActivate: [authGuard],
  },
  {
    path: 'payment/:id',
    component: PaymentViewComponent,
    canActivate: [authGuard],
  },
  {
    path: 'my-bookings',
    component: MyBookingsViewComponent,
    canActivate: [authGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
