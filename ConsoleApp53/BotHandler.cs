using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Useful_bot
{
    public class BotHandler
    {

        private string BotToken;
        JsonMethods json = new JsonMethods();
        public BotHandler(string Token)
        {
            BotToken = Token;
        }

        public async Task BotHandle()
        {
            var client = new TelegramBotClient(BotToken);

            using CancellationTokenSource cts = new CancellationTokenSource();

            ReceiverOptions reseiverOption = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandkePollingErrorAsync,
                receiverOptions: reseiverOption,
                cancellationToken: cts.Token
                );
            var me = await client.GetMeAsync();
            Console.WriteLine($"Start listeneing for @{me.Username}");
            Console.ReadLine();
            cts.Cancel();

        }



        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            Message message;
            string messageText = "";
            Contact? contact = null;
            if (update.Message != null)
            {
                message = update.Message;
            }
            else
            {
                return;
            }

            if (message.Text != null)
            {
                messageText = message.Text;
            }
            else if (message.Contact != null)
            {
                contact = message.Contact;
            }
            else
            {
                return;
            }


            var chatId = message.Chat.Id;

            json.Create(new User()
            {
                chatID = chatId,
                phoneNumber = "",
            });

            if (contact != null)
            {
                try
                {
                    Console.WriteLine($"Phone number: {contact.PhoneNumber}");
                    json.Update(chatId, contact.PhoneNumber);
                }
                catch
                {
                    return;
                }
            }
            if (messageText == "/getme")
            {
                string[] secondInfo = json.Read(chatId).Split(":");
                await client.SendTextMessageAsync(
                   chatId: chatId,
                   cancellationToken: cancellationToken,
                   text: $"Your chat Id = {secondInfo[0]}\nYour phone Number (if you send)= {secondInfo[1]}"
                   );
                return;
            }
            if (messageText == "/getall")
            {
                List<User> allInfo = json.GetAll();
                string allInfoString = "";
                foreach (User user in allInfo)
                {
                    allInfoString += $"Userid: {user.chatID}\nPhone: {user.phoneNumber}\n";
                }
                await client.SendTextMessageAsync(
                   chatId: chatId,
                   cancellationToken: cancellationToken,
                   text: $"{allInfoString}"
                   );
                return;
            }
            if (messageText == "/start")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(
                      new List<KeyboardButton[]>()
                       {
                        new KeyboardButton[]
                        {
                            KeyboardButton.WithRequestContact("Send my phone number"),
                        }

                       })
                {
                    ResizeKeyboard = true
                };

                await client.SendTextMessageAsync(
                    chatId: chatId,
                    cancellationToken: cancellationToken,
                    text: "Assalomu Aleykum",
                    replyMarkup: replyKeyboard
                    );
                return;
            }

            if (json.IsPhoneNumberNull(chatId) == false && contact == null)
            {
                await client.SendTextMessageAsync(
                   chatId: chatId,
                   cancellationToken: cancellationToken,
                   text: "Please send your Phone number"
                   );
                return;
            }
        }

        public Task HandkePollingErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken cancellationToken)
        {
            var ErrorMeassage = ex switch
            {
                ApiRequestException apiEx
                => $"Telegram API Error:\n[{apiEx.ErrorCode}]\n[{apiEx.Message}]",
                _ => ex.ToString()
            }; ;
            Console.WriteLine(ErrorMeassage);
            return Task.CompletedTask;
        }
    }
}
