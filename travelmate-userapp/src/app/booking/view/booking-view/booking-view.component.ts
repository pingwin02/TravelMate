import { Component, HostListener, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-booking-view',
  templateUrl: './booking-view.component.html',
  styleUrls: ['./booking-view.component.css']
})
export class BookingViewComponent implements OnInit, AfterViewInit, OnDestroy {
  reservationForm: FormGroup;
  timeLeft: number = 60;
  timerTimeout: any;
  private boundHandleNavLinkClick: any;

  constructor(
    private fb: FormBuilder,
    private router: Router,
  ) {
    this.reservationForm = this.fb.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      passenger_type: ['', Validators.required],
      seat_type: ['', Validators.required]
    });

    this.boundHandleNavLinkClick = this.handleNavLinkClick.bind(this);
  }

  ngOnInit() {
    const pageAccessedByReload = this.checkForPageRefresh();

    if (pageAccessedByReload) {
      this.cancelReservation();
    } else {
      this.loadTimer();
      this.startTimer();
    }
  }

  ngAfterViewInit() {
    document.addEventListener('click', this.boundHandleNavLinkClick, true);
  }

  private handleNavLinkClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;

    const isNavLink = target.matches('a.navbar-brand, a.nav-link') ||
      target.closest('a.navbar-brand, a.nav-link');

    if (isNavLink) {
      event.preventDefault();
      event.stopPropagation();

      const confirmExit = confirm('Do you really want to cancel your reservation and move to a different view?');

      if (confirmExit) {
        const linkElement = target.closest('a') as HTMLAnchorElement;
        const targetUrl = linkElement.getAttribute('href') || linkElement.getAttribute('routerLink') || '/offers';

        this.cancelReservation(targetUrl);
      }
    }
  }

  private checkForPageRefresh(): boolean {
    if (!sessionStorage.getItem('pageInitialized')) {
      sessionStorage.setItem('pageInitialized', 'true');
      return false;
    } else {
      return true;
    }
  }

  @HostListener('window:beforeunload', ['$event'])
  confirmReload(event: BeforeUnloadEvent) {
    event.returnValue = 'If you refresh, your reservation will be canceled.';

    sessionStorage.setItem('wasRefreshed', 'true');
  }

  @HostListener('window:popstate', ['$event'])
  onBackNavigation(event: Event) {
    event.preventDefault();
    const confirmExit = confirm('Do you really want to cancel your reservation and go back?');

    if (confirmExit) {
      this.cancelReservation();
    } else {
      history.pushState(null, '', location.href);
    }
  }

  private cancelReservation(targetUrl: string = '/offers') {
    if (this.timerTimeout) {
      clearTimeout(this.timerTimeout);
    }

    document.removeEventListener('click', this.boundHandleNavLinkClick, true);

    sessionStorage.removeItem('timeLeft');
    sessionStorage.removeItem('pageInitialized');
    sessionStorage.removeItem('wasRefreshed');

    this.router.navigateByUrl(targetUrl);
  }

  startTimer() {
    const updateTimer = () => {
      this.timeLeft -= 1;

      if (this.timeLeft <= 0) {
        this.cancelReservation();
      } else {
        sessionStorage.setItem('timeLeft', this.timeLeft.toString());
        this.timerTimeout = setTimeout(updateTimer, 1000);
      }
    };

    this.timerTimeout = setTimeout(updateTimer, 1000);
  }

  loadTimer() {
    const savedTime = sessionStorage.getItem('timeLeft');
    if (savedTime) {
      this.timeLeft = parseInt(savedTime, 10);
    } else {
      this.timeLeft = 60;
    }
  }

  ngOnDestroy() {
    document.removeEventListener('click', this.boundHandleNavLinkClick, true);

    if (this.timerTimeout) {
      clearTimeout(this.timerTimeout);
    }
  }
}
