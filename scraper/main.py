from driver_setup import setup_driver
from const import START_URL
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import ElementNotInteractableException
from time import sleep
import os
import json
def scrape_destinations():

    driver = setup_driver()
    
    driver.get(START_URL)
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
    results = []
    
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

        results.append({"from_city": "Warsaw",
                        "from_country": "Poland",
                        "to_country": dest_country_name,
                        "to_city":dest_city_name,
                        "route": route_text,
                        "from_city_code": route_text.split(" - ")[0],
                        "to_city_code": route_text.split(" - ")[1],
                        "link": link_href
                        })
        processed += 1
        with open("destinations.json", "w", encoding="utf-8") as f:
            json.dump(results, f, indent=4, ensure_ascii=False)
        back_button = wait.until(
                    EC.element_to_be_clickable((By.CSS_SELECTOR, ".js-crumb.Button-No-Standard-Style.crumb-title"))
                )
        back_button.click()
    
def scrape_flights(destinations_file:str):
    with open(destinations_file, "r", encoding="utf-8") as f:
        destinations = json.load(f)

    links = [destination["link"] for destination in destinations if "link" in destination]

    for link in links:
        driver = setup_driver()
        driver.get(link)
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

        flights_divs = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, "Fxw9-result-item-container")))
        print(f"Found {len(flights_divs)} flights")
        results = []

        #for flight in flights..


        driver.quit()

    

    

if __name__ == "__main__":
    scrape_destinations()
    #scrape_flights(destinations_file="destinations.json")