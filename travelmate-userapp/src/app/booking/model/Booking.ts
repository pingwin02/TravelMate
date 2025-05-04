import {BookingStatus} from "./booking-status.enum";
import {SeatType} from "./seat-type.enum";
import {PassengerType} from "./passenger-type.enum";

export interface Booking {
  Id: string;
  UserId: string;
  OfferId: string;
  PaymentId: string | null;
  Status: BookingStatus;
  ReservedUntil: string;
  SeatType: SeatType;
  PassengerName: string;
  PassengerType: PassengerType;
  CreatedAt: string;
}
