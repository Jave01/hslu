key = "6A5D5913C0ADA5C6F0C9801ACF2DAF2C"
import requests


# http://api.steampowered.com/<interface name>/<method name>/v<version>/?key=<api key>&format=<format>

a = requests.get('http://api.steampowered.com')

print(a)