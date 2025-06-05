import { Airplane } from '../../models/Airplane';
import { Airline } from '../../models/Airline';
import { Airport } from '../../models/Airport';

export interface Offer {
  id: string;
  airplaneName: string;
  airlineName: string;
  airlineIconUrl: string;
  departureAirportCode: string;
  departureAirportName?: string;
  departureAirportCity: string;
  departureAirportCountry?: string;
  arrivalAirportCode: string;
  arrivalAirportName?: string;
  arrivalAirportCity: string;
  arrivalAirportCountry?: string;
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  basePrice: number;
  availableEconomySeats: number;
  availableBusinessSeats: number;
  availableFirstClassSeats: number;
  createdAt: string;
}
