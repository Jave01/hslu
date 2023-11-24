from discord.message import Message
from config import Config

CMD_PREFIX = "pls"
HELP_MSG = "Sorry, cant help you"
SAD_MSG = "....................../´¯/)\n\
....................,/¯../ \n\
.................../..../ \n\
............./´¯/'...'/´¯¯`·¸ \n\
........../'/.../..../......./¯¯\\  \n\
........('(...'...'.... ¯~/'...') \n\
.........\\\.................'...../ \n\
..........''...\\\.......... _.·´ \n\
............\\\..............( \n\
..............\\\.............\\\..."


async def handle_resp(msg: Message, config: Config) -> str:
    content = msg.content.lower()
    if content == "hi pybot" or content == "hello pybot" or content == "hey pybot":
        await msg.channel.send(f"Hi there {msg.author.mention}")

    elif content.startswith(CMD_PREFIX):
        await exec_cmd(msg, config)


async def exec_cmd(msg: Message, config: Config) -> bool:
    if not msg.content.strip().startswith(CMD_PREFIX):
        return False

    cmd_args = msg.content.strip().split()[1:]
    if cmd_args[0] == "help":
        await msg.channel.send(config.help_msg)
    elif f"{cmd_args[0]} {cmd_args[1]}" == "go away":
        await msg.channel.send(SAD_MSG)

    return True
