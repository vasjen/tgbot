using System;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Payments;

namespace tgbot.services
{
    internal static class BotCommands

    {
        public const string _linkToBuy = "https://www.xbox.com/tr-TR/games/store/p/";
        public const string _jsStore = "?launchStore=";
        static int ConvertPriseToUSD(string text)
        {
            int result = 0;
            if (text != null)
            {
                text=text.Replace(".", ",");
                double temp = double.Parse(text);
                Console.WriteLine($"Price in tyr: {temp}");
                temp=temp*0.054;
                Console.WriteLine($"Price in $ USD: {temp}");
                temp = Math.Round(temp, 2);
                Console.WriteLine($"Price in $ USD: {temp} after round to 0.01");
                result = Convert.ToInt32(temp*100);
                Console.WriteLine($"Final price in $ ISD int32 : {result}");
                return result;
            }
            return result;
        }

        public static InlineKeyboardMarkup CreatingButtons()
        {
            /*
            InlineKeyboardButton _prev = new InlineKeyboardButton("<<-");
            InlineKeyboardButton _next = new InlineKeyboardButton("->>");
            InlineKeyboardButton _action = new InlineKeyboardButton("Buy");
            _prev.CallbackData="Previous"; _next.CallbackData="Next";
            _action.CallbackData="invoice";
            InlineKeyboardButton[] _1stRow = new InlineKeyboardButton[1];

            _1stRow[0]= _action;


            InlineKeyboardButton[] _2ndRow = new InlineKeyboardButton[2];
            _2ndRow[0] = _prev;
            _2ndRow[1] = _next;
            */
            InlineKeyboardMarkup _menu = new(new[] { new[]
            {
                InlineKeyboardButton.WithCallbackData(text:"<<--", callbackData:"left"),
                InlineKeyboardButton.WithCallbackData(text:"-->>", callbackData:"right"),
            },
            new[] { InlineKeyboardButton.WithCallbackData(text:"Buy it", callbackData: "invoice")  } });
            return _menu;
        }
        async public static Task NavigationButtons(TeleGramBotClass client)
            {


                await client.botClient.EditMessageMediaAsync(client._idOfSender, client._idOfPhotoMessage, new InputMediaPhoto(media: $"{client.gameCards[client._currentposition].photo}"));
                await client.botClient.EditMessageCaptionAsync(client._idOfSender, client._idOfPhotoMessage, $"" +
                   $"{client._currentposition+1} of {client.gameCards.Count}\n " +
                   $"{client.gameCards[client._currentposition].title} - " +
                   $"{client.gameCards[client._currentposition].price} ₺", replyMarkup: CreatingButtons());
            }
        
        async public static Task GetCallback (TeleGramBotClass client)
        {
            if (client.update.CallbackQuery != null)
            {
                Console.WriteLine("Register callback!");
                await client.botClient.AnswerCallbackQueryAsync(callbackQueryId: client.update.CallbackQuery.Id);
                Console.WriteLine("Answer send to -> {0}\n" +
                    "User choise is {1}", client.update.CallbackQuery.Id, client.update.CallbackQuery.Data);
                if (client.update.CallbackQuery.Data.ToLower()=="invoice")
                {
                    int PriceToInvoice = ConvertPriseToUSD(client.gameCards[client._currentposition].price);

                    LabeledPrice game1 = new LabeledPrice($"{client.gameCards[client._currentposition].title}", PriceToInvoice);
                    LabeledPrice game2 = new LabeledPrice("Service fee: 1$", 100);

                    LabeledPrice[] catalog = new LabeledPrice[2];
                    catalog[0] = game1;
                    catalog[1] = game2;
                    Message sendInvoice = await client.botClient.SendInvoiceAsync(chatId: client.update.CallbackQuery.From.Id,
                                      title: $"{client.gameCards[client._currentposition].title}",
                                      description: "Redem code will be send to this email!",
                                      payload: "somePayload",
                                      providerToken: "1877036958:TEST:c0c0f684e8b1c6968e6d66a6ed77d2cd46f8be4a",
                                      //providerToken: "1877036958:TEST:c0c0f684e8b1c6968e6d66a6ed77d2cd46f8be4a",
                                      currency: "USD",
                                      prices: catalog,
                                      needEmail: true,
                                      startParameter: "exapmle",
                                      isFlexible: false,
                                      photoUrl: $"{client.gameCards[client._currentposition].photo}"
                                      );
                }
                if (client.update.CallbackQuery.Data.ToLower()=="previous")
                {

                    if (client._currentposition >0)
                    {
                        client._currentposition--;
                        await NavigationButtons(client);
                    }
                }
                if (client.update.CallbackQuery.Data.ToLower()=="next")
                {
                    if (client._currentposition <client.gameCards.Count-1)
                    {
                        client._currentposition++;
                        await NavigationButtons(client);
                    }
                }
                return;
            }

        }
        async public static Task PrechekOut(TeleGramBotClass client)
        {
            if (client.update.PreCheckoutQuery is not { } prechek)
                return;
            if (prechek !=null)
            {

                Console.WriteLine("Recieved prechekout requets!");
                await client.botClient.AnswerPreCheckoutQueryAsync(
                   preCheckoutQueryId: prechek.Id);
                Console.WriteLine("Answer sended: to {0}", prechek.Id);

                return;
            }
            return;
        }

        
    }
}
