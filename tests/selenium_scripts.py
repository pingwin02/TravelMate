from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException, NoSuchElementException
from faker import Faker
from const import WEB_URL, TIMEOUT
import time
import random
import re


def init():
    driver = webdriver.Chrome()
    driver.maximize_window()
    driver.get(WEB_URL)

    return driver


def login_user(driver):
    wait = WebDriverWait(driver, TIMEOUT)

    login_button = wait.until(EC.element_to_be_clickable((By.LINK_TEXT, "Log in")))
    time.sleep(0.5)
    login_button.click()

    wait.until(EC.visibility_of_element_located((By.NAME, "username"))).send_keys("admin")
    time.sleep(0.5)
    driver.find_element(By.NAME, "password").send_keys("admin")
    time.sleep(0.5)
    driver.find_element(By.CSS_SELECTOR, "button[type='submit']").click()
    time.sleep(1)


def get_random_offer_from_random_page(driver, wait):
    wait.until(EC.presence_of_element_located((By.CLASS_NAME, "card")))

    try:
        page_links = driver.find_elements(By.CSS_SELECTOR, "pagination-controls .ngx-pagination a")

        if page_links:
            numeric_pages = []
            for link in page_links:
                try:
                    page_text = link.text.strip()
                    innerHTML = link.get_attribute("innerHTML") or ""

                    is_numeric = False
                    if page_text.isdigit():
                        is_numeric = True
                    elif any(char.isdigit() for char in innerHTML):
                        numbers = re.findall(r"\d+", innerHTML)
                        if numbers:
                            is_numeric = True

                    if is_numeric and link.is_displayed() and link.is_enabled():
                        numeric_pages.append(link)

                except Exception:
                    continue

            if len(numeric_pages) > 1:
                other_page_links = []

                for link in numeric_pages:
                    try:
                        classes = link.get_attribute("class") or ""
                        parent_classes = ""
                        try:
                            parent = link.find_element(By.XPATH, "..")
                            parent_classes = parent.get_attribute("class") or ""
                        except:
                            pass

                        if not (
                            "active" in classes.lower()
                            or "current" in classes.lower()
                            or "active" in parent_classes.lower()
                            or "current" in parent_classes.lower()
                        ):
                            other_page_links.append(link)
                    except:
                        other_page_links.append(link)

                pages_to_choose_from = other_page_links if other_page_links else numeric_pages

                if pages_to_choose_from:
                    random_page_link = random.choice(pages_to_choose_from)
                    driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", random_page_link)
                    time.sleep(2)
                    random_page_link.click()
                    time.sleep(2)
                    wait.until(EC.presence_of_element_located((By.CLASS_NAME, "card")))

    except (NoSuchElementException, Exception):
        pass

    cards = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "card")))

    if not cards:
        raise Exception("No offer cards found on the page")

    random_card = random.choice(cards)
    driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", random_card)
    time.sleep(1)

    return random_card


def make_reservation(driver, wait):
    offers_link = wait.until(EC.element_to_be_clickable((By.LINK_TEXT, "Offers")))
    offers_link.click()

    try:
        wait.until(EC.invisibility_of_element_located((By.CLASS_NAME, "spinner-border")))
    except TimeoutException:
        pass

    try:
        random_card = get_random_offer_from_random_page(driver, wait)
    except Exception as e:
        print(f"Error selecting random offer: {e}")
        return False

    try:
        more_info_button = random_card.find_element(By.CSS_SELECTOR, "button.btn.btn-primary")
        time.sleep(0.5)
        more_info_button.click()
    except NoSuchElementException:
        print("More Info button not found on selected card")
        return False

    try:
        faker = Faker("pl_PL")

        book_button = wait.until(EC.element_to_be_clickable((By.LINK_TEXT, "Book")))
        time.sleep(0.5)
        book_button.click()

        wait.until(EC.presence_of_element_located((By.ID, "name"))).send_keys(faker.first_name())
        time.sleep(0.5)
        driver.find_element(By.ID, "surname").send_keys(faker.last_name())
        time.sleep(0.5)

        driver.find_element(By.ID, "passenger_type").click()
        time.sleep(0.5)
        passenger_type_options = driver.find_elements(By.CSS_SELECTOR, "#passenger_type option")

        if passenger_type_options:
            random_passenger_type = random.choice(passenger_type_options)
            time.sleep(0.5)
            random_passenger_type.click()

        no_seats_elements = driver.find_elements(By.XPATH, "//p[contains(text(), 'No seats available')]")

        if no_seats_elements:
            print("No seats available for this offer")
            return False
        else:
            driver.find_element(By.ID, "seat_type").click()
            seat_type_options = driver.find_elements(By.CSS_SELECTOR, "#seat_type option")

            if seat_type_options:
                random_seat_type = random.choice(seat_type_options)
                time.sleep(0.5)
                random_seat_type.click()

            next_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Next')]")
            time.sleep(0.5)
            next_button.click()

            return True

    except Exception as e:
        print(f"Error during booking process: {e}")
        return False


def book_offers(driver, num_of_bookings):
    wait = WebDriverWait(driver, TIMEOUT)

    for i in range(num_of_bookings):
        try:
            reservation_completed = make_reservation(driver, wait)

            if reservation_completed:
                pay_now = wait.until(EC.element_to_be_clickable((By.XPATH, "//button[contains(text(), 'Pay Now')]")))
                time.sleep(1)
                pay_now.click()

                wait.until(EC.presence_of_element_located((By.CLASS_NAME, "container")))
                time.sleep(2)

            else:
                print(f"Booking failed - no seats available or other issue")

        except Exception as e:
            print(f"Booking failed with error: {e}")
            try:
                driver.get(WEB_URL)
                time.sleep(2)
            except:
                pass


def book_offer_timeout(driver):
    wait = WebDriverWait(driver, TIMEOUT)

    make_reservation(driver, wait)

    time.sleep(65)
    pay_now = wait.until(EC.element_to_be_clickable((By.XPATH, "//button[contains(text(), 'Pay Now')]")))
    pay_now.click()
    time.sleep(2)
