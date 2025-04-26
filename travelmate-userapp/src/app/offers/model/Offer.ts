import {Airplane} from "../../models/Airplane";
import {Airline} from "../../models/Airline";
import {Airport} from "../../models/Airport";

export interface Offer {
  id: string;
  airplane: Airplane;
  airline: Airline;
  departureAirport: Airport;
  arrivalAirport: Airport;
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  basePrice: number;
  availableEconomySeats: number;
  availableBusinessSeats: number;
  availableFirstClassSeats: number;
  createdAt: string;
}
