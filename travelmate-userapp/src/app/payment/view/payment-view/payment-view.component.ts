import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, NavigationStart, Router} from "@angular/router";

@Component({
  selector: 'app-payment-view',
  templateUrl: './payment-view.component.html',
  styleUrls: ['./payment-view.component.css']
})
export class PaymentViewComponent implements OnInit {
  success: boolean;

  constructor(private route: ActivatedRoute, private router: Router) {
    this.success = this.route.snapshot.paramMap.get('success') === 'true';
  }

  ngOnInit(): void {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart && event.navigationTrigger === 'popstate') {
        this.router.navigate(['/offers']);
      }
    });
  }

  goToOffers() {
    this.router.navigate(['/offers']);
  }
}
