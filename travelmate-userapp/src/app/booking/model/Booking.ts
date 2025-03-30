export interface Booking {
  booking_id: number;
  user_id: number;
  offer_id: number;
  status: string;
  seat_type: number;
  passenger_name: string;
  passenger_type: string;
}
