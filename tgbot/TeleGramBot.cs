using System;
using Parsing;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Telegram.Bot.Types.Payments;
using System.Diagnostics;
using System.Collections;
using static System.Net.WebRequestMethods;
using Telegram.Bot.Types.InlineQueryResults;

namespace TeleGramBot
{
    public class TeleGramBotClass
    {
        public TeleGramBotClass()
        {
        }
        private static string _botToken { get; set; }
        

        
        internal static async Task Run()
        {
        
            //Console.WriteLine("Insert API Token");
            _botToken = "5744464072:AAG2YTypfSV4PwWt7MlOnaB58SjvqLOlUSw";
            var cts = new CancellationTokenSource();
            var botClient = new TelegramBotClient(_botToken);



            
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
                };
                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: cts.Token
                );

          
           
            var me = await botClient.GetMeAsync();
            
            

            Console.WriteLine($"Start listening for @{me.Username}");

            
            Console.ReadLine();
            

            // Send cancellation request to stop bot
            cts.Cancel();

            
            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {

                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                {
                    if (update.PreCheckoutQuery is not { } prechek)
                        return;
                    if (prechek !=null)
                    {

                        Console.WriteLine("Recieved prechekout requets!");
                        await botClient.AnswerPreCheckoutQueryAsync(
                           preCheckoutQueryId: prechek.Id);
                        Console.WriteLine("Answer sended: to {0}", prechek.Id);
                        
                        return;


                    }
                    

                    if (update.CallbackQuery is not { } callBack)
                    { return; }
                    if (callBack != null)

                    {

                        await botClient.AnswerCallbackQueryAsync(callbackQueryId: callBack.Id);
                        await botClient.SendTextMessageAsync(callBack.From.Id, $"нажата кнопка! {callBack.Message}");
                    }


                    return;
                }
                Console.WriteLine($"Message type is {message.Type}");
                if (message.Type == MessageType.SuccessfulPayment)
                { Console.WriteLine("Successful buy!");
                  //  Console.WriteLine($"User {prechek.From.Id} bought the game\n" +
                    //       $"Details of order: Date: {DateTime.Now}\n" +
                      //     $"Gift code sent to {prechek.OrderInfo.Email}\n" +
                        //   $"OrderID: {prechek.Id}\n" +
                          // $"Amount: {prechek.TotalAmount}");
                }
                   




                // Only process text messages
                if (message.Text is not { } messageText)
                return;
                var chatId = message.Chat.Id;
                
                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                if (messageText.ToLower().Contains("want"))
                {
                    // LabeledPrice sprice = new LabeledPrice("GAme", 400); //{ Label = "Ololo", Amount = 400 };
                    //  IEnumerable<LabeledPrice> sprice = new LabeledPrice("GAme",200);
                    //PricePayments<LabeledPrice> sprice = new TeleGramBot.PricePayments<LabeledPrice>("TEST",500);
                    LabeledPrice game1 = new LabeledPrice("Game0", 200);
                    LabeledPrice game2 = new LabeledPrice("Game1", 500);

                    LabeledPrice[] catalog = new LabeledPrice[2];
                    catalog[0] = game1;
                    catalog[1] = game2;

                    await botClient.SendInvoiceAsync(chatId: message.Chat.Id,
                                              title: "Product",
                                              description: "Description",
                                              payload: "somePayload",
                                              providerToken: "1877036958:TEST:c0c0f684e8b1c6968e6d66a6ed77d2cd46f8be4a",
                                              //providerToken: "1877036958:TEST:c0c0f684e8b1c6968e6d66a6ed77d2cd46f8be4a",
                                              currency: "TRY",
                                              prices: catalog,
                                              needEmail: true,
                                              startParameter: "exapmle",
                                              isFlexible: false,
                                              photoUrl: "https://store-images.s-microsoft.com/image/apps.55934.13550335257385479.f907e8a1-c727-4bed-9e2c-94c239249dba.b5fd70da-71e5-4849-b499-25c43d8c9a25?q=90&w=177&h=265"
                                              );

                  


                }

                if (messageText.ToLower().Contains("btn"))
                {   
                    
                    var _testButton1 = new InlineKeyboardButton("First");
                    _testButton1.CallbackData ="WOW";

                    
                    var _testButton2 = new InlineKeyboardButton("Second");
                    _testButton2.CallbackData ="RLY";



                    InlineKeyboardMarkup menu = new InlineKeyboardMarkup(new[] { _testButton1, _testButton2});
                    
                    Message sentMessage_new = await botClient.SendTextMessageAsync(
                              chatId: chatId,
                              text: messageText,
                              replyMarkup: menu,
                              
                              cancellationToken: cancellationToken);

                    



                }


             




                /* if (messageText.Contains("/buy"))
                 {
                     LabeledPrice labeledPrice = new LabeledPrice("test", 140);
                     LabeledPrice[] some = new LabeledPrice[5];
                     for (int i=0;i<4; i++)
                     { some[i]=labeledPrice;
                     }
                     await botClient.SendInvoiceAsync((int)chatId, "Some title of the gmae", ".... and descryption of the current gmae", "payload", "1877036958:TEST:c0c0f684e8b1c6968e6d66a6ed77d2cd46f8be4a", "TRY", some);
                 }*/
                /*
                if (messageText.Contains("http"))
                 {

                     ChromeOptions options = new ChromeOptions();
                     options.AddArgument("--window-size=500,658");
                    //IWebDriver driver = new ChromeDriver(options);
                    //driver.Url = "https://account.xbox.com/account/signin?returnUrl=" +messageText;
                     BuyingGame instanse = new BuyingGame(messageText);
                  // instanse.Authentification(driver);
                    // instanse.BuyTheGme(driver);
                 }
                */
                //
                if (messageText.Contains("/find"))
                {
                    FindingGame findingGame = new FindingGame(messageText);

                    Message sentMessage_new = await botClient.SendTextMessageAsync(
                              chatId: chatId,
                              text: "Finding now: ",
                              cancellationToken: cancellationToken);

                    findingGame.FindTheGame(findingGame._field);
                }
                 
                // Echo received message text


            }
            
            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }

            

        }
    }

}

