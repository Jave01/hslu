from discord.message import Message
from discord.ext.commands import Context
from . import bot
import requests
from settings import WEATHER_API_KEY
from table2ascii import table2ascii as t2a, PresetStyle, Alignment


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


@bot.event
async def on_ready():
    print(f"{bot.user} is now running")


@bot.event
async def on_message(message: Message):
    if message.author == bot.user:
        return

    if message.content.startswith(CMD_PREFIX):
        await bot.process_commands(message)


@bot.command(
    help="Get weather for a city, usage: {CMD_PREFIX}weather <city> [f/full/all]"
)
async def weather(ctx: Context, city: str):
    """Get weather for a city
    Args:
        ctx (Context): Context
        city (str): City to get weather for
    """
    await ctx.send(f"Trying to get weather for {city}")
    # example url: https://api.weatherapi.com/v1/current.json?key=f412410fc19649c4a19120258232411&q=London&aqi=no
    city = city if city else "London"
    url = f"https://api.weatherapi.com/v1/current.json?key={WEATHER_API_KEY}&q={city}&aqi=no"
    response = requests.get(url)
    if response.status_code == 200:
        data = response.json()
        mode = ctx.message.content.split()[-1].lower()
        if mode == "f" or mode == "full" or mode == "all":
            # In your command:
            loc = data["location"]
            output = t2a(
                header=[
                    "Type",
                    "Data",
                ],
                body=[
                    ["Local time", loc["localtime"]],
                    ["Temperature", f"{data['current']['temp_c']}°C"],
                    ["Condition", f"{data['current']['condition']['text']}"],
                    [
                        "Wind",
                        f"{data['current']['wind_kph']}kph - {data['current']['wind_dir']}",
                    ],
                    ["Humidity", f"{data['current']['humidity']}%"],
                ],
                style=PresetStyle.thin_compact,
                alignments=[Alignment.LEFT, Alignment.RIGHT],
                cell_padding=3,
            )
            await ctx.send(f"```\n# Data for {data['location']['name']}\n{output}\n```")
        else:
            temp_c = data["current"]["temp_c"]
            await ctx.send(f"Temperature in {city} is {temp_c}°C")
    else:
        await ctx.send(f"Could not get weather for {city}")


@bot.command(name="go")
async def go_away(ctx: Context):
    if ctx.message.content == CMD_PREFIX + "go away":
        await ctx.channel.send(SAD_MSG)
