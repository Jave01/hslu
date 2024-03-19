import os
import json

"""
Do NOT change
"""

CONFIG_FNAME = "config.json"
SETTINGS = None


class _Config:
    def __init__(
        self,
        cmd_prefix: str,
        config_fname: str = None,
        work_dir: str = None,
        help_msg: str = None,
        weather_api_key: str = "",
    ) -> None:
        self.cmd_prefix = cmd_prefix
        self.config_fname = config_fname
        self.cwd = work_dir
        self.help_msg = help_msg
        self.weather_api_key = weather_api_key

    def default_config():
        cwd = os.path.dirname(os.path.abspath(__file__))
        help_msg = "No help message available"
        return _Config("!", CONFIG_FNAME, cwd, help_msg)


with open("config.json") as f:
    global SETTINGS
    try:
        config = json.load(f)
        SETTINGS = _Config(
            config["cmd_prefix"],
            config["config_fname"],
            config["cwd"],
            config["help_msg"],
            config["weather_api_key"],
        )
    except FileNotFoundError:
        print(f"Could not find a {CONFIG_FNAME} file in the main directory.")
        exit()
    except KeyError as ke:
        print(f"Config {ke} not found in config file")
        exit()
