from selenium_scripts import init, login_user, book_offers

if __name__ == "__main__":

    driver = init()

    login_user(driver)
    book_offers(driver, 5)

    driver.quit()
