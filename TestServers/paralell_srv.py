import logging
import multiprocessing
import requests
import time

# URL for the API endpoints
tempUrl = "http://localhost:8080/api/v1/Temperature"
soilUrl = "http://localhost:8080/api/v1/Soil"
lightUrl = "http://localhost:8080/api/v1/Light"

# Headers
headers = {
    "Content-Type": "application/json",
}


def listener_configurer():
    root = logging.getLogger()
    h = logging.handlers.RotatingFileHandler(
        'log_multiprocessing.log', 'a', 10000, 5)
    formatter = logging.Formatter(
        '%(asctime)s - %(levelname)s - %(processName)s - %(message)s')
    h.setFormatter(formatter)
    root.addHandler(h)


def listener_process(queue):
    listener_configurer()
    while True:
        try:
            record = queue.get()
            if record is None:  # We send this as a sentinel to tell the listener to quit.
                break
            logger = logging.getLogger(record.name)
            # No level or filter logic applied - just do it!
            logger.handle(record)
        except Exception:
            import sys
            import traceback
            print('Whoops! Problem:', file=sys.stderr)
            traceback.print_exc(file=sys.stderr)


def worker_configurer(queue):
    h = logging.handlers.QueueHandler(queue)
    root = logging.getLogger()
    root.addHandler(h)
    root.setLevel(logging.INFO)


def sendTemperature(queue):
    worker_configurer(queue)
    payloads = [{"Temp": 20.0 + i} for i in range(1, 11)]
    id = 0

    while True:
        idx = id % len(payloads)

        logging.info(f"Sending Temperature request #: {id+1}")

        # Send the POST request
        response = requests.post(tempUrl, json=payloads[idx], headers=headers)

        if response.status_code == 200:
            id += 1
        else:
            logging.error(f"Error sending request: {response}")
            exit(-1)
        time.sleep(5)


def sendMoisture(queue):
    worker_configurer(queue)
    payloads = [{"MoistureLvl": 20.0 + i} for i in range(1, 11)]
    id = 0

    while True:
        idx = id % len(payloads)

        logging.info(f"Sending Moisture request #: {id+1}")

        # Send the POST request
        response = requests.post(
            soilUrl, json=payloads[idx], headers=headers)

        if response.status_code == 200:
            id += 1
        else:
            logging.error(f"Error sending request: {response}")
            exit(-1)
        time.sleep(5)


if __name__ == '__main__':
    queue = multiprocessing.Queue(-1)
    listener = multiprocessing.Process(target=listener_process, args=(queue,))
    listener.start()

    temperatureProcess = multiprocessing.Process(
        target=sendTemperature, args=(queue,))
    moistureProcess = multiprocessing.Process(
        target=sendMoisture, args=(queue,))

    temperatureProcess.start()
    moistureProcess.start()

    temperatureProcess.join()
    moistureProcess.join()

    # Tell the listener to stop
    queue.put_nowait(None)
    listener.join()
