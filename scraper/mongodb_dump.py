from pymongo import MongoClient
import requests
from datetime import datetime
from datetime import timezone, timedelta

MONGO_URI = "mongodb://root:student@localhost:27017"
DATABASE_NAME = "RSWD_188597_offersquerydb"
COLLECTION_NAME = "Offers"

BASE_URL = "http://localhost:5000/api/Offers"


def fetch_all_offers():
    response = requests.get(BASE_URL)
    response.raise_for_status()
    return response.json()


def fetch_offer_by_id(offer_id):
    response = requests.get(f"{BASE_URL}/{offer_id}")
    response.raise_for_status()
    return response.json()


def convert_to_offer_dto(offer):
    utc_plus_2 = timezone(timedelta(hours=2))
    return {
        "_id": offer["id"],
        "AirplaneName": offer["airplane"]["name"],
        "AirlineName": offer["airline"]["name"],
        "AirlineIconUrl": offer["airline"]["iconUrl"],
        "DepartureAirportCode": offer["departureAirport"]["code"],
        "DepartureAirportName": offer["departureAirport"]["name"],
        "DepartureAirportCity": offer["departureAirport"]["city"],
        "DepartureAirportCountry": offer["departureAirport"]["country"],
        "ArrivalAirportCode": offer["arrivalAirport"]["code"],
        "ArrivalAirportName": offer["arrivalAirport"]["name"],
        "ArrivalAirportCity": offer["arrivalAirport"]["city"],
        "ArrivalAirportCountry": offer["arrivalAirport"]["country"],
        "FlightNumber": offer["flightNumber"],
        "DepartureTime": datetime.fromisoformat(offer["departureTime"]).replace(tzinfo=utc_plus_2),
        "ArrivalTime": datetime.fromisoformat(offer["arrivalTime"]).replace(tzinfo=utc_plus_2),
        "BasePrice": offer["basePrice"],
        "AvailableEconomySeats": offer["availableEconomySeats"],
        "AvailableBusinessSeats": offer["availableBusinessSeats"],
        "AvailableFirstClassSeats": offer["availableFirstClassSeats"],
        "CreatedAt": datetime.fromisoformat(offer["createdAt"]).replace(tzinfo=utc_plus_2),
    }


def save_offers_to_mongodb(offers):
    client = MongoClient(MONGO_URI)
    db = client[DATABASE_NAME]
    collection = db[COLLECTION_NAME]

    for offer in offers:
        try:
            collection.insert_one(offer)
        except Exception as e:
            print(f"Error inserting offer with Id {offer['_id']}: {e}")

    print(f"Inserted {len(offers)} offers into MongoDB.")
    client.close()


def main():
    try:
        all_offers = fetch_all_offers()
        print(f"Fetched {len(all_offers)} offers.")

        converted_offers = []
        for offer in all_offers:
            offer_id = offer["id"]
            detailed_offer = fetch_offer_by_id(offer_id)
            offer_dto = convert_to_offer_dto(detailed_offer)
            converted_offers.append(offer_dto)

        save_offers_to_mongodb(converted_offers)

    except Exception as e:
        print(f"An error occurred: {e}")


if __name__ == "__main__":
    main()
