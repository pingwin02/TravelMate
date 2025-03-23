import { Injectable } from '@angular/core';
import {User} from "../model/User";
import {BehaviorSubject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private users: User[] = [
    {
      user_id: 1,
      name: 'Julia',
      password_hash: 'julia123',
      email: 'julia@example.com',
      phone_number: '48789123456'
    },
    {
      user_id: 2,
      name: 'Tomek',
      password_hash: 'tomek123',
      email: 'tomek@example.com',
      phone_number: '48123789456'
    },
    {
      user_id: 3,
      name: 'Damian',
      password_hash: 'damian123',
      email: 'damian@example.com',
      phone_number: '48123456789'
    },
    {
      user_id: 4,
      name: 'Robert',
      password_hash: 'robert123',
      email: 'robert@example.com',
      phone_number: '48987654321'
    }
  ];

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    const savedUser = localStorage.getItem('loggedUser');
    if (savedUser) {
      this.currentUserSubject.next(JSON.parse(savedUser));
    }
  }

  getUserByEmail(email: string): User | undefined {
    return this.users.find(user => user.email === email);
  }

  login(email: string, password: string): boolean {
    const user = this.getUserByEmail(email);
    if (user && user.password_hash === password) {
      this.currentUserSubject.next(user);
      localStorage.setItem('loggedUser', JSON.stringify(user));
      return true;
    }
    return false;
  }

  logout() {
    this.currentUserSubject.next(null);
    localStorage.removeItem('loggedUser');
  }
}
