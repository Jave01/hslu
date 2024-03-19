import discord
from discord.ext import commands
from config import Config


intents = discord.Intents.default()
intents.message_content = True


bot = commands.Bot(intents=intents)


def run(token: str, settings: Config):
    bot.command_prefix = settings.cmd_prefix
    bot.help_command = commands.DefaultHelpCommand(no_category="Commands")
    bot.run(token)
