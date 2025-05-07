from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from const import WEB_URL, TIMEOUT
import time
import random


def init():
    driver = webdriver.Chrome()

    driver.get(WEB_URL)

    return driver


def login_user(driver):
    wait = WebDriverWait(driver, TIMEOUT)

    login_button = wait.until(EC.element_to_be_clickable((By.LINK_TEXT, "Log in")))
    login_button.click()

    wait.until(EC.visibility_of_element_located((By.NAME, "username"))).send_keys("admin")
    driver.find_element(By.NAME, "password").send_keys("admin")
    driver.find_element(By.CSS_SELECTOR, "button[type='submit']").click()
    time.sleep(1)


def make_reservation(driver, wait):
    offers_link = wait.until(EC.element_to_be_clickable((By.LINK_TEXT, "Offers")))
    offers_link.click()

    cards = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "card")))

    random_card = random.choice(cards)
    driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", random_card)
    time.sleep(1)

    more_info_button = random_card.find_element(By.CSS_SELECTOR, "button.btn.btn-primary")
    more_info_button.click()

    book_button = wait.until(EC.element_to_be_clickable((By.LINK_TEXT, "Book")))
    book_button.click()

    wait.until(EC.presence_of_element_located((By.ID, "name"))).send_keys("Jan")
    driver.find_element(By.ID, "surname").send_keys("Kowalski")

    driver.find_element(By.ID, "passenger_type").click()
    passenger_type_options = driver.find_elements(By.CSS_SELECTOR, "#passenger_type option")

    random_passenger_type = random.choice(passenger_type_options)
    random_passenger_type.click()

    driver.find_element(By.ID, "seat_type").click()
    seat_type_options = driver.find_elements(By.CSS_SELECTOR, "#seat_type option")

    random_seat_type = random.choice(seat_type_options)
    random_seat_type.click()

    next_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Next')]")
    next_button.click()


def book_offers(driver, num_of_offers):
    wait = WebDriverWait(driver, TIMEOUT)

    for i in range(num_of_offers):
        make_reservation(driver, wait)

        pay_now = wait.until(EC.element_to_be_clickable((By.XPATH, "//button[contains(text(), 'Pay Now')]")))
        pay_now.click()

        wait.until(EC.presence_of_element_located((By.CLASS_NAME, "container")))
        time.sleep(2)


def book_offer_timeout(driver):
    wait = WebDriverWait(driver, TIMEOUT)

    make_reservation(driver, wait)

    time.sleep(60)
    driver.find_element(By.LINK_TEXT, "Offers").click()
    driver.back() 
    time.sleep(2)
