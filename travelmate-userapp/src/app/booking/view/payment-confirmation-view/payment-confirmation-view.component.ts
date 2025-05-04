import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, NavigationStart, Router} from "@angular/router";

@Component({
  selector: 'app-payment-confirmation-view',
  templateUrl: './payment-confirmation-view.component.html',
  styleUrls: ['./payment-confirmation-view.component.css']
})
export class PaymentConfirmationViewComponent  implements OnInit {
  success: boolean = true;

  constructor(private route: ActivatedRoute, private router: Router) {
  }

  ngOnInit(): void {
  }

  goToOffers() {
    this.router.navigate(['/offers']);
  }
}
