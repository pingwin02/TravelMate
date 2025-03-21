export interface Offer{
  offer_id : number;
  flight_number: string;
  plane: string;
  departure: string
  destination: string;
  departure_date : string;
  arrival_date: string;
  base_price: number;
  airline: string;
  available_economy_seats: number;
  available_business_seats: number;
  available_first_class_seats: number;
}
