import discord
from discord.ext import commands
from config import Config

CMD_PREFIX = "/"

intents = discord.Intents.default()
intents.message_content = True


bot = commands.Bot(command_prefix=CMD_PREFIX, intents=intents)


@bot.event
async def on_ready():
    print(f'{bot.user} is now running')


@bot.event
async def on_message(message: discord.message.Message):
    if message.author == bot.user:
        return

    if message.content.startswith(CMD_PREFIX):
        await bot.process_commands(message)


def run(token: str, config: Config):
    bot.run(token)


