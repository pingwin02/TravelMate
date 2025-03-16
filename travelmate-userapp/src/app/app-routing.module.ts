import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {OffersViewComponent} from "./offers/view/offers-view/offers-view.component";

const routes: Routes = [
  {
    path: 'offers',
    component: OffersViewComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
