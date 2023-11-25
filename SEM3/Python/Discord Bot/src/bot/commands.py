from discord.message import Message
from discord.ext.commands import Context
from . import bot
import requests
from settings import WEATHER_API_KEY

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


@bot.command()
async def help(ctx: Context):
    await ctx.send(HELP_MSG)


@bot.command()
async def weather(ctx: Context, city: str):
    await ctx.send(f"Trying to get weather for {city}")
    # example url: https://api.weatherapi.com/v1/current.json?key=f412410fc19649c4a19120258232411&q=London&aqi=no
    city = city if city else "London"
    url = f"https://api.weatherapi.com/v1/current.json?key={WEATHER_API_KEY}&q={city}&aqi=no"
    response = requests.get(url)
    if response.status_code == 200:
        data = response.json()
        temp_c = data['current']['temp_c']
        await ctx.send(f"Temperature in {city} is {temp_c}°C")
    else:
        await ctx.send(f"Could not get weather for {city}")



