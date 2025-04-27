import { Component } from '@angular/core';
import {Router} from "@angular/router";
import {User} from "../../model/User";
import {AuthService} from "../../service/auth.service";

@Component({
  selector: 'app-login-view',
  templateUrl: './login-view.component.html',
  styleUrls: ['./login-view.component.css']
})
export class LoginViewComponent {

  username = '';
  password = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    const user: User = {
      Username: this.username,
      Password: this.password
    };
    this.authService.login(user).subscribe({
      next: () => {
        const redirectUrl = localStorage.getItem('redirectUrl');
        if (redirectUrl) {
          this.router.navigate([redirectUrl]);
          localStorage.removeItem('redirectUrl');
        } else {
          this.router.navigate(['/']);
        }
      },
      error: (error) => {
        alert(error.message);
      }
    });
  }
}
