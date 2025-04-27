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
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  private currentUserSubject = new BehaviorSubject<User | null>(this.getUserFromToken());

  constructor(private http: HttpClient, private router: Router) {}

  login(user: User): Observable<any> {
    return this.http.post<any>('auth/auth/login', user).pipe(
      tap(response => {
        if (response.token) {
          localStorage.setItem(this.token, response.token);
          this.loggedIn.next(true);
          this.currentUserSubject.next({ Username: user.Username, Password: user.Password });
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
    localStorage.removeItem(this.token);
    this.loggedIn.next(false);
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.token);
  }

  getCurrentUser(): Observable<User | null> {
    return this.currentUserSubject.asObservable();
  }

  private getUserFromToken(): User | null {
    const token = this.getToken();
    if (!token) return null;
    const decodedToken = JSON.parse(atob(token.split('.')[1]));
    return decodedToken ? { Username: decodedToken.sub, Password: '' } : null;
  }
}
