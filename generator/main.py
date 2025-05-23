import requests
import time
import random
from datetime import datetime, timedelta

BASE_URL = "http://localhost:18859/oferty/offers"


def fetch_offers():
    response = requests.get(BASE_URL)
    response.raise_for_status()
    return response.json()


def update_offer_partial(offer):
    response = requests.get(f"{BASE_URL}/60fe9f55-1345-442c-9a63-5295621e8b01")
    response.raise_for_status()
    full_offer = response.json()

    original_departure = datetime.fromisoformat(full_offer["departureTime"].replace("Z", ""))
    original_arrival = datetime.fromisoformat(full_offer["arrivalTime"].replace("Z", ""))
    flight_duration = original_arrival - original_departure

    new_departure = datetime.utcnow() + timedelta(days=random.randint(1, 30))
    new_arrival = new_departure + flight_duration
    new_price = max(50, int(full_offer["basePrice"] * random.uniform(0.9, 1.1)))
    available_economy = random.randint(0, 150)
    available_business = random.randint(0, 30)
    available_first = random.randint(0, 10)

    full_offer.update(
        {
            "basePrice": new_price,
            "departureTime": new_departure.isoformat() + "Z",
            "arrivalTime": new_arrival.isoformat() + "Z",
            "availableEconomySeats": available_economy,
            "availableBusinessSeats": available_business,
            "availableFirstClassSeats": available_first,
        }
    )

    put_response = requests.put(BASE_URL, json=full_offer)
    if put_response.ok:
        print(
            f"[{datetime.now()}] Updated offer {full_offer['flightNumber']} — New price: {new_price}, "
            f"Departure: {full_offer['departureTime']}, Economy seats: {available_economy}"
        )
    else:
        print(
            f"[{datetime.now()}] Error updating offer {full_offer['id']}: {put_response.status_code} {put_response.text}"
        )


def main_loop():
    offers = fetch_offers()
    if not offers:
        print("No offers found.")
        return

    print("Starting update loop...")
    while True:
        offer = random.choice(offers)
        update_offer_partial(offer)
        time.sleep(3)


if __name__ == "__main__":
    main_loop()
