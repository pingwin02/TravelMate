import { Component } from '@angular/core';
import {User} from "./users/model/User";
import {UserService} from "./users/service/users.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'travelmate-userapp';
  currentUser: User | null = null;

  constructor(private userService: UserService) {
    this.userService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  logout() {
    this.userService.logout();
  }
}
