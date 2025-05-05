import { BookingStatus } from './booking-status.enum';
import { SeatType } from './seat-type.enum';
import { PassengerType } from './passenger-type.enum';

export interface Booking {
  id: string;
  userId: string;
  offerId: string;
  paymentId: string | null;
  status: BookingStatus;
  reservedUntil: string;
  seatType: SeatType;
  passengerName: string;
  passengerType: PassengerType;
  createdAt: string;
}
