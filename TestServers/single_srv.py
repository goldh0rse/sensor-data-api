import logging
from time import sleep
import requests
import datetime

port = 5057

# URL for the API endpoints
tempUrl = f'http://localhost:{port}/api/v1/Temperature'
soilUrl = f'http://localhost:{port}/api/v1/Soil'
lightUrl = f'http://localhost:{port}/api/v1/Light'

# Headers
headers = {
    "Content-Type": "application/json",
}


def sendTemperature(dt, temp):
    # payload = {"datetime": dt, "Temp": temp}
    payload = { "temp": temp }

    response = requests.post(tempUrl, json=payload, headers=headers)
    if response.status_code == 200:
        # TODO: Fix this
        logging.info("")
    else:
        logging.error(f"Error sending request: {response}")


def sendSoil(dt, temp, moisture):
    # payload = {"datetime": dt, "soil_temperature": temp,
    #            "soil_moisture": moisture}
    payload = {"soil_temperature": temp, "soil_moisture": moisture}

    response = requests.post(soilUrl, json=payload, headers=headers)
    if response.status_code == 200:
        # TODO: Fix this
        logging.info("")
    else:
        logging.error(f"Error sending request: {response}")


def sendLight(dt, lux):
    # payload = {"datetime": dt, "lux": lux}
    payload = { "lux": lux }

    response = requests.post(lightUrl, json=payload, headers=headers)
    if response.status_code == 200:
        # TODO: Fix this
        logging.info("")
    else:
        logging.error(f"Error sending request: {response}")


if __name__ == '__main__':
    id = 0
    temps = [20.0 + i for i in range(0, 15)]
    soils = [300.0 + (i*10.0) for i in range(0, 15)]
    lights = [40.0 + (i*5) for i in range(0, 15)]

    while True:
        dt = datetime.datetime.now()
        idx = id % 15
        sendTemperature(dt, temps[idx])
        # sendSoil(dt, temps[idx], soils[idx])
        # sendLight(dt, lights[idx])
        id += 1
        sleep(1)
