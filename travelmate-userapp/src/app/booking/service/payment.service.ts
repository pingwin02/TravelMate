import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {Payment} from "../model/Payment";

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  constructor(private http: HttpClient) { }

  /* get payment by id */
  getPaymentById(id: string): Observable<Payment> {
    return this.http.get<Payment>(`/platnosci/Payments/${id}`)
  }
}
