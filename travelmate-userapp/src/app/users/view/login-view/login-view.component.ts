import { Component } from '@angular/core';
import {Router} from "@angular/router";
import {UserService} from "../../service/users.service";

@Component({
  selector: 'app-login-view',
  templateUrl: './login-view.component.html',
  styleUrls: ['./login-view.component.css']
})
export class LoginViewComponent {
  email: string = '';
  password: string = '';

  constructor(private router: Router, private userService: UserService) {}

  onSubmit() {
    if (this.userService.login(this.email, this.password)) {
      alert(`Login successful!`);
      this.router.navigate(['/']);
    } else {
      alert('Invalid email or password. Please try again.');
    }
  }
}
