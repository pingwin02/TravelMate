from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager
import os


def setup_driver():
    """setup for chrome webdriver"""
    options = Options()

    options.add_argument("--window-size=1920,1080")

    driver = webdriver.Chrome(service=Service(
        ChromeDriverManager().install()), options=options)
    return driver


def init():
    if not os.path.exists("./results"):
        os.makedirs("./results")

    if not os.path.exists("./results/destinations.json"):
        with open("./results/destinations.json", "w") as f:
            f.write("[]")

    if not os.path.exists("./results/airlines.json"):
        with open("./results/airlines.json", "w") as f:
            f.write("[]")

    if not os.path.exists("./results/airports.json"):
        with open("./results/airports.json", "w") as f:
            f.write("[]")

    if not os.path.exists("./results/planes.json"):
        with open("./results/planes.json", "w") as f:
            f.write("[]")

    if not os.path.exists("./results/flights.json"):
        with open("./results/flights.json", "w") as f:
            f.write("[]")
