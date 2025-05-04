import {SeatType} from "./seat-type.enum";
import {PassengerType} from "./passenger-type.enum";

export interface BookingCreate{
  OfferId: string;
  SeatType: SeatType;
  PassengerName: string;
  PassengerType: PassengerType;

}
