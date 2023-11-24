import discord
from bot.responses import handle_resp
from config import Config


def run(token: str, config: Config):
    intents = discord.Intents.default()
    intents.message_content = True
    client = discord.Client(intents=intents)

    @client.event
    async def on_ready():
        print(f'{client.user} is now running')

    @client.event
    async def on_message(message: discord.message.Message):
        if message.author == client.user:
            return

        await handle_resp(message, config=config)

    client.run(token)
