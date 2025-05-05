import { Injectable } from '@angular/core';
import {BehaviorSubject, catchError, Observable, tap, throwError} from "rxjs";
import {User} from "../model/User";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private token = 'auth_token';
  private user_name = 'user_name';
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient, private router: Router) {}

  login(user: User): Observable<any> {
    return this.http.post<any>('auth/auth/login', user).pipe(
      tap(response => {
        if (response.token) {
          sessionStorage.setItem(this.token, response.token);
          sessionStorage.setItem(this.user_name, user.Username);
          this.loggedIn.next(true);
        }
      }),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return throwError(() => new Error('Invalid username or password.'));
        }
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    sessionStorage.removeItem(this.token);
    sessionStorage.removeItem(this.user_name);
    this.loggedIn.next(false);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  private hasToken(): boolean {
    return !!sessionStorage.getItem(this.token);
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.token);
  }

  getUsername(): string | null  {
    return sessionStorage.getItem(this.user_name);
  }
}
