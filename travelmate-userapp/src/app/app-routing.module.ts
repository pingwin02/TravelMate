import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {OffersViewComponent} from "./offers/view/offers-view/offers-view.component";
import {OfferViewComponent} from "./offers/view/offer-view/offer-view.component";
import {LoginViewComponent} from "./users/view/login-view/login-view.component";
import {BookingViewComponent} from "./booking/view/booking-view/booking-view.component";
import {PaymentViewComponent} from "./payment/view/payment-view/payment-view.component";

const routes: Routes = [
  {
    path: 'offers',
    component: OffersViewComponent
  },
  {
    path: 'offer/:id',
    component: OfferViewComponent
  },
  {
    path: 'login',
    component: LoginViewComponent
  },
  {
    path: 'booking',
    component: BookingViewComponent
  },
  {
    path: 'payment',
    component: PaymentViewComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
