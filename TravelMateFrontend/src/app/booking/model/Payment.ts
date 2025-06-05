import { PaymentStatus } from './payment-status.enum';

export interface Payment {
  id: string;
  bookingId: string;
  amount: number;
  status: PaymentStatus;
  transactionDate: string;
}
