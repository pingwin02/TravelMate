import { SeatType } from './seat-type.enum';
import { PassengerType } from './passenger-type.enum';

export interface Bookings {
  OfferId: string;
  UserId: string;
  SeatNumber: string;
  SeatType: SeatType;
  PassengerName: string;
  PassengerType: PassengerType;
}
