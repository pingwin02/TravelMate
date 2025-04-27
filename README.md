# TravelMate

## Prerequisites
- .NET 9.0
- MariaDB 11.7.2

## Setup

1. Install `dotnet-ef`
    ```bash
    dotnet tool install --global dotnet-ef
    ```

2. Create a new database in MariaDB

    Travel Mate Offers:
    ```sql
    CREATE DATABASE travelmatedb;
    ```
    
    Travel Mate Auth:
    ```sql
    CREATE DATABASE authdb;
    ```
3. Update the `appsettings.Development.json`:

    Travel Mate Offers:

   ```json
    "ConnectionStrings": {
        "DefaultDbConnection":
        "Server=localhost;Database=travelmatedb;User=root Password=yourpassword;"
    }
    ```
    Travel Mate Auth:

    ```json
    "ConnectionStrings": {
        "DefaultDbConnection":
        "Server=localhost;Database=authdb;User=root Password=yourpassword;"
    },
    "Jwt": {
        "Key": "tokenkey",
        "Issuer": "authmicroservice",
        "Audience": "mainapp"
    }
  
    ```
4. Create migrations and update the database:

    Travel Mate Offers:
    ```bash
    cd ./TravelMateBackend
    dotnet-ef migrations add InitialCreate
    dotnet-ef database update
    ```

    Travel Mate Auth:
    ```bash
    cd ./TravelMateAuthService
    dotnet-ef migrations add InitialCreate
    dotnet-ef database update
    ```
## Scraper
1. Create venv 
    
   ```bash
        cd ./scraper
        python -m venv venv
   ```
2. Activate venv
    ```bash
    source venv/bin/activate
    ```

3. Install requirements
    ```bash
    pip install -r requirements.txt
    ```
4. Set configuration in const.py

    example configuration for Rome to anywhere with 0 stops:
    ```python
    FROM = "ROM" # IATA code for Rome
    FROM_FULL = "Rome" # Full name of the city
    FROM_COUNTRY = "Italy" # country
    TO = "anywhere" # destination (iata or anywhere)
    STOPS = 0 # number of stops (0, 1, 2)

    START_URL = f"https://www.kayak.com/explore/{FROM}-{TO}?stops={STOPS}"


    DESTINATION_PATH = "results/destinations.json"
    FLIGHTS_PATH = "results/flights.json"
    AIRPORTS_PATH = "results/airports.json"
    AIRLINES_PATH = "results/airlines.json"
    PLANES_PATH = "results/planes.json"
    ```

5. Run the scraper
    ```bash
    python main.py
    ```
    > The scraper will create a folder called `results` in the `scraper` folder. This folder will contain the following files:
    - `destinations.json`: contains the destinations and their IATA codes
    - `flights.json`: contains the flights and their details
    - `airports.json`: contains the airports and their details
    - `airlines.json`: contains the airlines and their details
    - `planes.json`: contains the planes and their details

6. Generate offers

    To create parsed offers json for the database, you need to run `sql.dumper.py` file. This file will insert the above json files into the database and create `parsed_flight.json` file in the `results` folder. This file will contain the parsed offers and their details, which are also inserted into the database.

    >If you have different database name, change it in `import_json_to_sql() ` function in `sql_dumper.py` file.
    
## Insert backup data into the database
1. Download `./results` folder from files server, put it in the `scraper` folder 
2. Comment out the `generate_offers_from_flights()` function (so that `parsed_flight.json` is not replaced by new data) in `sql_dumper.py` file.

2. Run `sql_dumper.py` to insert data into the database
    ```bash
    python sql_dumper.py
    ```


