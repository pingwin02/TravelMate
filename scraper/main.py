from driver_setup import setup_driver
import const
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import ElementNotInteractableException
from time import sleep
import os
import json
import re
def scrape_destinations():

    destination_results = []
    airport_results = []
    
    driver = setup_driver()
    
    driver.get(const.START_URL)
    wait = WebDriverWait(driver, 10)

    cookie_button = wait.until(
                EC.element_to_be_clickable((By.CSS_SELECTOR, 
                "button[class*='RxNS'][class*='RxNS-mod-stretch'][class*='RxNS-mod-variant-outline']"
                "[class*='RxNS-mod-theme-base'][class*='RxNS-mod-shape-default']"
                "[class*='RxNS-mod-spacing-base'][class*='RxNS-mod-size-small']"))
            )
    print("Clicking cookie acceptance button...")
    cookie_button.click()
    sleep(2)  
    
    
    more_destinations_button = driver.find_element(By.XPATH, "//button[contains(@id, 'showMoreButton')]")

    while more_destinations_button:
        sleep(1)
        print("Clicking 'more destinations' button...")
        driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", more_destinations_button)
        try:
            more_destinations_button.click()
        except ElementNotInteractableException:
            break
        sleep(1)
        more_destinations_button = driver.find_element(By.XPATH, "//button[contains(@id, 'showMoreButton')]")

    driver.execute_script("window.scrollTo(0, 0);")



    print("Finding all destination divs...")



    destination_divs = wait.until(
        EC.presence_of_all_elements_located((By.CSS_SELECTOR, "button.Button-No-Standard-Style._id7._iae._h-Y._iam._TS"))
    )
    
    if not destination_divs:
        return
        
    print(f"Found {len(destination_divs)} destination divs")

    processed = 0
    max = len(destination_divs)

    while processed < max:
        destination_divs = wait.until(
        EC.presence_of_all_elements_located((By.CSS_SELECTOR, "button.Button-No-Standard-Style._id7._iae._h-Y._iam._TS"))
    )
        driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", destination_divs[processed])
        sleep(1)
        dest_city_name = destination_divs[processed].find_element(By.CLASS_NAME, "_ib0._igh._ial._1O._iaj.City__Name").text
        dest_country_name = destination_divs[processed].find_element(By.CLASS_NAME, "_iC8._1W._ib0._iYh._igh.Country__Name").text
        print(f"Processing destination {processed + 1} of {max}: {dest_country_name}, {dest_city_name}")
        destination_divs[processed].click()
        sleep(2)
        title_element = wait.until(
                    EC.presence_of_element_located((By.CSS_SELECTOR, ".clickout-box-title"))
                )
        route_text = title_element.text.strip()
        link_element = wait.until(
            EC.presence_of_element_located((By.CSS_SELECTOR, "a.explore-clickout-button"))
        )
        link_href = link_element.get_attribute("href")

        print(f"Found route: {route_text}")
        print(f"Link: {link_href}")

        destination_results.append({"from_city": const.FROM_FULL,
                        "from_country": const.FROM_COUNTRY,
                        "to_country": dest_country_name,
                        "to_city":dest_city_name,
                        "route": route_text,
                        "from_city_code": route_text.split(" - ")[0],
                        "to_city_code": route_text.split(" - ")[1],
                        "link": link_href
                        })
        
        if(route_text.split(" - ")[0] not in [airport["code"] for airport in airport_results]):
            airport_results.append({"city": const.FROM_FULL,
                        "country": const.FROM_COUNTRY,
                        "code": route_text.split(" - ")[0]
                        })
        if(route_text.split(" - ")[1] not in [airport["code"] for airport in airport_results]):
            airport_results.append({"city": dest_city_name,
                        "country": dest_country_name,
                        "code": route_text.split(" - ")[1]
                        })

        processed += 1
        with open(const.DESTINATION_PATH, "w", encoding="utf-8") as f:
            json.dump(destination_results, f, indent=4, ensure_ascii=False)

        with open(const.AIRPORTS_PATH, "w", encoding="utf-8") as f:
            json.dump(airport_results, f, indent=4, ensure_ascii=False)
        back_button = wait.until(
                    EC.element_to_be_clickable((By.CSS_SELECTOR, ".js-crumb.Button-No-Standard-Style.crumb-title"))
                )
        back_button.click()
    
def scrape_flights(destinations_file:str, airports_file:str=None, airlines_file:str=None, planes_file:str=None):
    with open(destinations_file, "r", encoding="utf-8") as f:
        try:
            destinations = json.load(f)
        except json.JSONDecodeError:
            destinations = []
    
    if airports_file:
        with open(airports_file, "r", encoding="utf-8") as f:
            try:
                airports = json.load(f)
            except json.JSONDecodeError:
                airports = []

    if airlines_file:
        with open(airlines_file, "r", encoding="utf-8") as f:
            try:
                airlines = json.load(f)
            except json.JSONDecodeError:
                airlines = []

    if planes_file:
        with open(planes_file, "r", encoding="utf-8") as f:
            try:
                planes = json.load(f)
            except json.JSONDecodeError:
                planes = []

    links = [destination["link"] for destination in destinations if "link" in destination]

    for destination in destinations:
        driver = setup_driver()
        driver.get(destination["link"])
        wait = WebDriverWait(driver, 20)

        cookie_button = wait.until(
                EC.element_to_be_clickable((By.CSS_SELECTOR, 
                "button[class*='RxNS'][class*='RxNS-mod-stretch'][class*='RxNS-mod-variant-outline']"
                "[class*='RxNS-mod-theme-base'][class*='RxNS-mod-shape-default']"
                "[class*='RxNS-mod-spacing-base'][class*='RxNS-mod-size-small']"))
            )
        print("Clicking cookie acceptance button...")
        cookie_button.click()
        sleep(2) 

        try:
            flights = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, "div.nrc6-main")))
            print(f"Found {len(flights)} flights")
        except:
            driver.quit()
            continue


        
        results = []
        processed = 0
        max = len(flights)
        while processed < max:
            try:
                flights = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, "div.nrc6-main")))
                driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", flights[processed])
                flights[processed].click()
                sleep(1)

                flight_numbers = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, "div.nAz5-carrier-text")))
                print(len(flight_numbers))
                dep_flight_number, arr_flight_number = flight_numbers[0].text, flight_numbers[1].text

                plane_types = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, ".z6uD.z6uD-mod-theme-neutral.z6uD-mod-variant-outline.z6uD-mod-layout-inline.z6uD-mod-text-align-center.z6uD-mod-size-large.z6uD-mod-padding-x-xsmall"
                )))
                dep_plane_type, arr_plane_type = plane_types[0].text, plane_types[1].text

                carrier_icons = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, ".nAz5-carrier-icon img")))

                dep_carrier_icon_url, arr_carrier_icon_url = carrier_icons[0].get_attribute("src"), carrier_icons[1].get_attribute("src")
                dep_carrier_lane_name, arr_carrier_lane_name = carrier_icons[0].get_attribute("alt"), carrier_icons[1].get_attribute("alt")

                flight_time = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, ".X3K_-leg-duration")))
                dep_flight_duration, arr_flight_duration = flight_time[0].text, flight_time[1].text

                print(f"Departure flight number: {dep_flight_number}")
                print(f"Departure plane type: {dep_plane_type}")
                print(f"Arrival flight number: {arr_flight_number}")
                print(f"Arrival plane type: {arr_plane_type}")
                print(f"Departure carrier icon url: {dep_carrier_icon_url}")
                print(f"Arrival carrier icon url: {arr_carrier_icon_url}")
                print(f"Departure carrier name: {dep_carrier_lane_name}")
                print(f"Arrival carrier name: {arr_carrier_lane_name}")
                print(f"Departure flight duration: {dep_flight_duration}")
                print(f"Arrival flight duration: {arr_flight_duration}")


                if planes_file:
                    for plane_type in plane_types:
                        if plane_type.text not in [plane["name"] for plane in planes]:
                            planes.append({"name": plane_type.text})
                    with open(planes_file, "w", encoding="utf-8") as f:
                        json.dump(planes, f, indent=4, ensure_ascii=False)

                if airlines_file:
                    for carrier_icon in carrier_icons:
                        airline_found = False
                        for airline in airlines:
                            if airline["name"] == carrier_icon.get_attribute("alt"):
                                airline["icon_url"] = carrier_icon.get_attribute("src")
                                airline_found = True
                        if not airline_found:
                            airlines.append({
                                "name": carrier_icon.get_attribute("alt"),
                                "icon_url": carrier_icon.get_attribute("src")
                            })
                    
                    with open(airlines_file, "w", encoding="utf-8") as f:
                        json.dump(airlines, f, indent=4, ensure_ascii=False)
                        
                if airports_file:
                    airport_names = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, ".g16k-station")))
                    for airport_name in airport_names:
                        match = re.search(r"\((.*?)\)", airport_name.text)
                        if not match:
                            continue
                        code = match.group(1)
                        
                        for airport in airports:
                            if airport["code"] == code:
                                airport["city"] = airport_name.text.partition(" ")[0]
                                airport["name"] = airport_name.text
                                airport_found = True

                        if not airport_found:
                            airports.append({
                                "code": code,
                                "city": airport_name.partition(" ")[0],
                                "name": airport_name.text
                            })
                    
                    results.append({
                        "dep_code": destination["from_city_code"],
                        "arr_code": destination["to_city_code"],
                        "flight_number": dep_flight_number,
                        "flight_duration": dep_flight_duration,
                        "plane_type": dep_plane_type,
                        })
                    
                    results.append({
                        "dep_code": destination["to_city_code"],
                        "arr_code": destination["from_city_code"],
                        "flight_number": arr_flight_number,
                        "flight_duration": arr_flight_duration,
                        "plane_type": arr_plane_type,
                    })




                with open(airports_file, "w", encoding="utf-8") as f:
                    json.dump(airports, f, indent=4, ensure_ascii=False)

                with open(const.FLIGHTS_PATH, "w", encoding="utf-8") as f:
                    json.dump(results, f, indent=4, ensure_ascii=False)


                flights[processed].click()
                processed+=1
            except:
                print("Error processing flight")
                processed+=1
                continue



            sleep(1)
            

            




        driver.quit()

    

    

if __name__ == "__main__":
 

    #scrape_destinations()
    scrape_flights(destinations_file=const.DESTINATION_PATH, airports_file=const.AIRPORTS_PATH, airlines_file=const.AIRLINES_PATH, planes_file=const.PLANES_PATH)