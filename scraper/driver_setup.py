from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager


def setup_driver():
    """setup for chrome webdriver"""
    options = Options()

    options.add_argument("--window-size=1920,1080")

    driver = webdriver.Chrome(service=Service(
        ChromeDriverManager().install()), options=options)
    return driver
