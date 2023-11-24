import os


class Config:
    def __init__(self, cmd_prefix: str, config_fname: str = None, work_dir: str = None, help_msg: str = None) -> None:
        self.cmd_prefix = cmd_prefix
        self.config_fname = config_fname
        self.cwd = work_dir
        self.help_msg = help_msg

    def default_config():
        cwd = os.path.dirname(os.path.abspath(__file__))
        help_msg = "No help message available"
        return Config("pls", "conf.json", cwd, help_msg)
