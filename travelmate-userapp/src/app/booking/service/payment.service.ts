import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, of, throwError } from 'rxjs';
import { Payment } from '../model/Payment';
import { map } from 'rxjs/operators';
import { error } from '@angular/compiler-cli/src/transformers/util';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  constructor(private http: HttpClient) {}

  /* get payment by id */
  getPaymentById(id: string): Observable<Payment> {
    return this.http.get<Payment>(`/platnosci/Payments/${id}`);
  }

  /* pay and get payment result */

  payPayment(id: string): Observable<{ success: boolean; message: string }> {
    return this.http.get(`/platnosci/Payments/${id}/pay`, { responseType: 'text' }).pipe(
      map((responseText: string) => ({
        success: true,
        message: responseText
      })),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 400) {
          const errorMessage = typeof error.error === 'string' ? error.error : 'Failed reservation';
          return of({
            success: false,
            message: errorMessage
          });
        } else {
          return throwError(() => error);
        }
      })
    );
  }
}
