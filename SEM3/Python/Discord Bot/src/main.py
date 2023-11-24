from bot import run
import os
import json
from config import Config

# Token = MTE3NDc2NjAyNDY3MTkwMzgwNA.GHaIBN.LjcViYwGCViCP3vHPYQ0UbQzx4wb9u6VXkklO8
# invite link = https://discord.com/api/oauth2/authorize?client_id=1174766024671903804&permissions=534723815488&scope=bot
CONF_FNAME = "config.json"
HELP_MSG = """
----------- You called for help, I am your saviour -----------
- `help`                - displays this message
- `weather <city>`      - get the weather of a specific city
- `a_emoji <emoji>`     - get send an ascii emoji
- `go away`             - I'm sad 
"""


if __name__ == "__main__":
    try:
        d_path = os.path.dirname(os.path.abspath(__file__))
        f_conf_path = os.path.join(d_path, CONF_FNAME)
        with open(f_conf_path) as f:
            config = json.load(f)
        token = config['token']
        print()

    except FileNotFoundError:
        print(f"Could not find a '{CONF_FNAME}' file in the main directory.")
        exit()

    except KeyError as ke:
        print(f"Config {ke} not found in {CONF_FNAME}")
        exit()

    settings = Config.default_config()
    settings.help_msg = HELP_MSG
    run(token, settings)
