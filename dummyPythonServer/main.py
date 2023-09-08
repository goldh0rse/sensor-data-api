import requests
import json
import time

# URL for the API endpoint
url = "http://localhost:8080/api/v1/Temperature"

# Headers
headers = {
    "Content-Type": "application/json",
}

if __name__ == '__main__':
    payloads = [{"Id": 0, "Temp": 20.0 + i} for i in range(1, 11)]
    id = 0

    while True:
        idx = id % len(payloads)

        print(f"\rSending request with ID: {id}", end="", flush=True)
        # Send the POST request
        response = requests.post(url, json=payloads[idx])

        if response.status_code == 200:
            id += 1
        else:
            print("\nError sending request")
            print(response)
            exit(-1)
        time.sleep(5)
