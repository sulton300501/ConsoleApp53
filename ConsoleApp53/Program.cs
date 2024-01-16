using Telegram.Bot;
using Useful_bot;
class Project
{
    static async Task Main()
    {
        const string botToken = "6009545507:AAHGV5owArZrDktnbbXGGc8fZErdQDz_tzU";
        BotHandler handler = new(botToken);
        try
        {
            await handler.BotHandle();
        }
        catch
        {
            await handler.BotHandle();
        }
    }
}