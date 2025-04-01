import json
import mysql.connector
import uuid
import os
airports_query = """
INSERT INTO airports (`Code`, `City`, `Country`,`Name`) 
VALUES (%s, %s, %s, %s)
"""

airlines_query = """
INSERT INTO airlines (`Name`, `IconUrl`)
VALUES (%s, %s)
"""

flights_query = """
INSERT INTO flights (`FlightNumber`, `Departure`, `Arrival`, `Date`, `Time`, `Duration`, `Price`, `Airline`)
VALUES (%s, %s, %s, %s, %s, %s, %s, %s)
"""

planes_query = """
INSERT INTO airplanes (`Id`, `Name`, `AvailableEconomySeats`, `AvailableBusinessSeats`, `AvailableFirstClassSeats`)
VALUES(%s,%s,%s,%s,%s)
"""

flights_query = """

"""

def import_json_to_mysql(json_file, table_name):
    conn = mysql.connector.connect(
        host=os.getenv("DB_HOST", "localhost"),
        user=os.getenv("DB_USER", "root"),
        password=os.getenv("DB_PASSWORD", "root"),
        database=os.getenv("DB_NAME", "db")
    )
    cursor = conn.cursor()
    """Import JSON data into MySQL table"""
    with open(json_file, 'r') as file:
        data = json.load(file)

    # Assuming the JSON data is a list of dictionaries
    for item in data:
        cursor.execute(planes_query, (str(uuid.uuid4()),item["name"],item["available_economy_seats"],item["available_business_seats"],item["available_first_class_seats"]))

    conn.commit()

import_json_to_mysql("./results/planes.json", "airplanes")


