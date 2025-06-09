import { PassengerType } from "src/app/booking/model/passenger-type.enum";
import { SeatType } from "src/app/booking/model/seat-type.enum";

export interface OfferPreferences{
    seatType: SeatType;
    seatCount: number;
    passengerType: PassengerType;
    passengerCount: number;
}