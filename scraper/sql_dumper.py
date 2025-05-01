import json
import mysql.connector
import uuid
import os
from datetime import datetime, timedelta
import re
import random

# offers generator


def parse_duration(duration_str):
    pattern = r'(\d+)h\s*(\d+)m'
    match = re.search(pattern, duration_str)

    if match:
        hours = int(match.group(1))
        minutes = int(match.group(2))
        return timedelta(hours=hours, minutes=minutes)
    return timedelta(hours=0, minutes=0)


def generate_random_date():
    today = datetime.now()
    start_date = today - timedelta(days=30)  # last month
    end_date = today + timedelta(days=30)    # next month

    time_between_dates = end_date - start_date
    days_between_dates = time_between_dates.days
    random_days = random.randrange(days_between_dates)

    random_date = start_date + timedelta(days=random_days)

    random_hour = random.randint(0, 23)
    random_minute = random.randint(0, 59)

    return random_date.replace(hour=random_hour, minute=random_minute, second=0, microsecond=0)


def process_flight_data(flights_file, planes_file, airlines_file):
    with open(flights_file, 'r') as file:
        flights = json.load(file)
        if not isinstance(flights, list):
            flights = [flights]

    with open(planes_file, 'r') as file:
        planes = json.load(file)
        if not isinstance(planes, list):
            planes = [planes]

    with open(airlines_file, 'r') as file:
        airlines = json.load(file)
        if not isinstance(airlines, list):
            airlines = [airlines]

    offers = []

    for flight in flights:
        flight_number = flight['flight_number']
        last_space_index = flight_number.rfind(' ')
        airline_name = flight_number[:last_space_index] if last_space_index != - \
            1 else flight_number

        matching_airline = next(
            (a for a in airlines if a['name'].upper() == airline_name.upper()), None)

        matching_plane = next(
            (p for p in planes if p['name'].upper() == flight['plane_type'].upper()), None)

        if not matching_airline or not matching_plane:
            print(
                f"Warning: Could not find matching airline or plane for flight {flight['flight_number']}")
            continue

        departure_time = generate_random_date()
        duration = parse_duration(flight['flight_duration'])
        arrival_time = departure_time + duration
        available_economy_seats = random.randint(
            0, matching_plane['available_economy_seats'])
        available_business_seats = random.randint(
            0, matching_plane['available_business_seats'])
        available_first_class_seats = random.randint(
            0, matching_plane['available_first_class_seats'])

        offer = {
            'Id': str(uuid.uuid4()),
            'AirplaneId': matching_plane['id'],
            'AirlineName': matching_airline['name'],
            'DepartureAirportCode': flight['dep_code'],
            'ArrivalAirportCode': flight['arr_code'],
            'FlightNumber': flight['flight_number'],
            'DepartureTime': departure_time.strftime('%Y-%m-%d %H:%M:%S.%f')[:-3],
            'ArrivalTime': arrival_time.strftime('%Y-%m-%d %H:%M:%S.%f')[:-3],
            'BasePrice': flight["base_price"],
            'AvailableEconomySeats': available_economy_seats,
            'AvailableBusinessSeats': available_business_seats,
            'AvailableFirstClassSeats': available_first_class_seats,
            'CreatedAt': datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]
        }

        offers.append(offer)

    return offers


def save_processed_data(offers, output_file):
    os.makedirs(os.path.dirname(output_file), exist_ok=True)
    with open(output_file, 'w') as file:
        json.dump(offers, file, indent=4)
    print(f"Successfully created {output_file} with {len(offers)} offers")


def generate_offers_from_flights():
    try:
        flights_file = './results/flights.json'
        planes_file = './results/planes.json'
        airlines_file = './results/airlines.json'
        offers_output = './results/parsed_flights.json'

        offers = process_flight_data(flights_file, planes_file, airlines_file)

        save_processed_data(offers, offers_output)

    except Exception as e:
        print(f"Error: {e}")


airports_query = """
INSERT INTO Airports (`Code`, `City`, `Country`,`Name`) 
VALUES (%s, %s, %s, %s)
"""

airlines_query = """
INSERT INTO Airlines (`Name`, `IconUrl`)
VALUES (%s, %s)
"""

offers_query = """
INSERT INTO Offers (
    `Id`, `AirplaneId`, `AirlineName`, `DepartureAirportCode`, 
    `ArrivalAirportCode`, `FlightNumber`, `DepartureTime`, 
    `ArrivalTime`, `BasePrice`, `AvailableEconomySeats`, 
    `AvailableBusinessSeats`, `AvailableFirstClassSeats`, `CreatedAt`
)
VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
"""

planes_query = """
INSERT INTO Airplanes (`Id`,`Name`, `AvailableEconomySeats`, `AvailableBusinessSeats`, `AvailableFirstClassSeats`)
VALUES(%s,%s,%s,%s,%s)
"""


def import_json_to_mysql(json_file, query, fields):
    conn = mysql.connector.connect(
        host=os.getenv("DB_HOST", "localhost"),
        user=os.getenv("DB_USER", "root"),
        password=os.getenv("DB_PASSWORD", "student"),
        database=os.getenv("DB_NAME", "RSWD_188597_offersdb")
    )
    cursor = conn.cursor()
    """Import JSON data into MySQL table"""
    with open(json_file, 'r') as file:
        data = json.load(file)

    for item in data:
        try:
            values = [item.get(field) for field in fields]

            cursor.execute(query, tuple(values))
        except Exception as e:
            print(e)
            continue
    conn.commit()


import_json_to_mysql("./results/planes.json", planes_query,
                     ["id", "name", "available_economy_seats", "available_business_seats", "available_first_class_seats"])
import_json_to_mysql("./results/airlines.json",
                     airlines_query, ["name", "icon_url"])
import_json_to_mysql("./results/airports.json",
                     airports_query, ["code", "city", "country", "name"])

generate_offers_from_flights()
import_json_to_mysql("./results/parsed_flights.json", offers_query, [
    'Id', 'AirplaneId', 'AirlineName', 'DepartureAirportCode',
    'ArrivalAirportCode', 'FlightNumber', 'DepartureTime',
    'ArrivalTime', 'BasePrice', 'AvailableEconomySeats',
    'AvailableBusinessSeats', 'AvailableFirstClassSeats', 'CreatedAt'
])
