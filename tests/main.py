from selenium_scripts import init, login_user, book_offers, book_offer_timeout
import sys


def user_input():
    if len(sys.argv) > 1:
        try:
            num_of_bookings = int(sys.argv[1])
            return num_of_bookings
        except ValueError:
            print("Given argument is not an integer")
            return None
    else:
        print("No argument given")
        return None


if __name__ == "__main__":
    num_of_bookings = user_input()

    if num_of_bookings is not None:
        driver = init()

        login_user(driver)
        book_offers(driver, num_of_bookings)
        book_offer_timeout(driver)

        driver.quit()
