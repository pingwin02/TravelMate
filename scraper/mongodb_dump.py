from pymongo import MongoClient
import requests
from datetime import datetime

# MongoDB connection details
MONGO_URI = "mongodb://admin:password@localhost:27017"
DATABASE_NAME = "TravelMateOfferQueryDatabase"
COLLECTION_NAME = "Offers"

# API endpoints
BASE_URL = "http://localhost:5000/api/Offers"

# Function to fetch all offers
def fetch_all_offers():
    response = requests.get(BASE_URL)
    response.raise_for_status()  # Raise an error if the request fails
    return response.json()

# Function to fetch a specific offer by ID
def fetch_offer_by_id(offer_id):
    response = requests.get(f"{BASE_URL}/{offer_id}")
    response.raise_for_status()  # Raise an error if the request fails
    return response.json()

# Function to convert API response to OfferDto format with Id as the key
def convert_to_offer_dto(offer):
    return {
        "_id": offer["id"],  # Set Id as the key (_id)
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
        "DepartureTime": datetime.fromisoformat(offer["departureTime"]),
        "ArrivalTime": datetime.fromisoformat(offer["arrivalTime"]),
        "BasePrice": offer["basePrice"],
        "AvailableEconomySeats": offer["availableEconomySeats"],
        "AvailableBusinessSeats": offer["availableBusinessSeats"],
        "AvailableFirstClassSeats": offer["availableFirstClassSeats"],
        "CreatedAt": datetime.fromisoformat(offer["createdAt"]),
    }

# Function to save offers to MongoDB
def save_offers_to_mongodb(offers):
    client = MongoClient(MONGO_URI)
    db = client[DATABASE_NAME]
    collection = db[COLLECTION_NAME]

    # Insert offers into MongoDB
    for offer in offers:
        try:
            collection.insert_one(offer)  # Insert each offer
        except Exception as e:
            print(f"Error inserting offer with Id {offer['_id']}: {e}")

    print(f"Inserted {len(offers)} offers into MongoDB.")
    client.close()

# Main function
def main():
    try:
        # Step 1: Fetch all offers
        all_offers = fetch_all_offers()
        print(f"Fetched {len(all_offers)} offers.")

        # Step 2: Fetch details for each offer and convert to OfferDto
        converted_offers = []
        for offer in all_offers:
            offer_id = offer["id"]
            detailed_offer = fetch_offer_by_id(offer_id)
            offer_dto = convert_to_offer_dto(detailed_offer)
            converted_offers.append(offer_dto)

        # Step 3: Save converted offers to MongoDB
        save_offers_to_mongodb(converted_offers)

    except Exception as e:
        print(f"An error occurred: {e}")

# Run the script
if __name__ == "__main__":
    main()