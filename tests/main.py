from selenium_scripts import init, login_user, book_offers, book_offer_timeout
import sys

def user_input():
    if len(sys.argv) > 1:
        try:
            num_of_offers = int(sys.argv[1])
            return num_of_offers
        except ValueError:
            print("Given argument is not an integer")
            return None
    else:
        print("No argument given")
        return None


if __name__ == "__main__":
    num_of_offers = user_input()

    if num_of_offers is not None:
        driver = init()

        login_user(driver)
        book_offers(driver, num_of_offers)
        book_offer_timeout(driver)

        driver.quit()
