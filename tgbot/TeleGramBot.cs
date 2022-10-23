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
using System.Security.Cryptography.X509Certificates;
using static TeleGramBot.FindingGame;

namespace TeleGramBot
{
    
    public class TeleGramBotClass
    {
        public delegate void StatusHandler(string message);
        public event StatusHandler? Notify;
        private int _idOfMessage;
        private string _textValue;
        public TeleGramBotClass()
        {
        }
        private static string _botToken { get; set; }
        private static int _idOfPhotoMessage {get;set; }
        private static string _insertText { get; set; }

        
        internal static async Task Run()
        {
            TeleGramBotClass tgbot = new TeleGramBotClass();
            
            //Console.WriteLine("Insert API Token");
            _botToken = "5744464072:AAG2YTypfSV4PwWt7MlOnaB58SjvqLOlUSw";
            var cts = new CancellationTokenSource();
            var botClient = new TelegramBotClient(_botToken);
            var newbot = new TelegramBotClient (_botToken);

            static  InlineKeyboardMarkup GenerationMethod()
            {
                InlineKeyboardButton _prev = new InlineKeyboardButton("<<-");
                InlineKeyboardButton _next = new InlineKeyboardButton("->>");
                InlineKeyboardButton _action = new InlineKeyboardButton("Buy");
                _prev.CallbackData="Previous"; _next.CallbackData="Next";
                _action.CallbackData="BUY!!";
                InlineKeyboardButton[] _test1row = new InlineKeyboardButton[1];
                
                    _test1row[0]= new InlineKeyboardButton("BUY!!!");
                    _test1row[0].CallbackData="invoice";
                
                
                InlineKeyboardButton[] _test2row = new InlineKeyboardButton[2];
                _test2row[0] = _prev;
                _test2row[1] = _next;
                
                InlineKeyboardMarkup _menu = new InlineKeyboardMarkup(new[] {_test1row, _test2row }); ;
                return _menu;
            }

            
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
                };
            var additionalrececvierOptions = new ReceiverOptions
            {
                AllowedUpdates=new UpdateType[]
                {
                    UpdateType.Message
                }
            };
                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: cts.Token
                );

          
           
           // var me = await botClient.GetMeAsync();
            
            

           // Console.WriteLine($"Start listening for @{me.Username}");

            
            Console.ReadLine();
            

            // Send cancellation request to stop bot
            cts.Cancel();

            
            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {


                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                {

                    if (update.CallbackQuery != null)
                    {
                        Console.WriteLine("Register callback!");
                        await botClient.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id);
                        Console.WriteLine("Answer send to -> {0}\n" +
                            "User choise is {1}", update.CallbackQuery.Id, update.CallbackQuery.Data);
                        if (update.CallbackQuery.Data.ToLower()=="invoice")
                        {
                            LabeledPrice game1 = new LabeledPrice("Game0", 200);
                            LabeledPrice game2 = new LabeledPrice("Game1", 500);

                            LabeledPrice[] catalog = new LabeledPrice[2];
                            catalog[0] = game1;
                            catalog[1] = game2;
                            await botClient.SendInvoiceAsync(chatId: update.CallbackQuery.From.Id,
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
                        if (update.CallbackQuery.Data.ToLower()=="previous")
                        {

                            await botClient.EditMessageMediaAsync(chatId: update.CallbackQuery.From.Id,
                               messageId: _idOfPhotoMessage, new InputMediaPhoto(media: "https://store-images.s-microsoft.com/image/apps.34695.68182501197884443.ac728a87-7bc1-4a0d-8bc6-0712072da93c.25816f86-f27c-4ade-ae29-222661145f1f?w=200"));
                            await botClient.EditMessageCaptionAsync(chatId: update.CallbackQuery.From.Id,
                                messageId: _idOfPhotoMessage, caption: "A some different game!!!\n" +
                                "U can buy for 60$",
                                replyMarkup: GenerationMethod()
                                );
                        }
                        return;
                    }
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
                    return;
                }
                tgbot._idOfMessage=message.MessageId;
                Console.WriteLine($"Message type is {message.Type}");
                
            
                if (message.Type == MessageType.SuccessfulPayment)
                { Console.WriteLine($"Successful buy!" +
                    $"Amount of buy:\t {message.SuccessfulPayment.TotalAmount} \n" +
                    $"Order of Payment:\n " +
                    $"Id of payment:\t {message.SuccessfulPayment.TelegramPaymentChargeId}\n" +
                    $"Id of provider:\t {message.SuccessfulPayment.ProviderPaymentChargeId}\n" +
                    $"Invoice:\t {message.SuccessfulPayment.InvoicePayload}\n" +
                    $"Code send to:\t {message.SuccessfulPayment.OrderInfo.Email}");
                  //  Console.WriteLine($"User {prechek.From.Id} bought the game\n" +
                    //       $"Details of order: Date: {DateTime.Now}\n" +
                    //     $"Gift code sent to {prechek.OrderInfo.Email}\n" +
                    //   $"OrderID: {prechek.Id}\n" +
                    // $"Amount: {prechek.TotalAmount}");
                }
                   




                // Only process text messages
                if (message.Text is not { } messageText)
                return;
                tgbot._textValue=messageText;
                _insertText=messageText;
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

                if (messageText.ToLower().Contains("menu"))
                {

                               Message sentExample = await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: "https://store-images.s-microsoft.com/image/apps.55934.13550335257385479.f907e8a1-c727-4bed-9e2c-94c239249dba.b5fd70da-71e5-4849-b499-25c43d8c9a25?q=90&w=177&h=265",
                                caption: "Name of the this Game\n Price 45$",
                                replyMarkup: GenerationMethod());
                    _idOfPhotoMessage = sentExample.MessageId;





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
                    FindingGame findingGame = new FindingGame();
                    //var newUpdate = botClient.GetUpdatesAsync();

                    Message asked = await botClient.SendTextMessageAsync(chatId: chatId, text: "Whats the game are you searching?");
                    var newUpdate = await  botClient.GetUpdatesAsync();
                    tgbot._textValue=newUpdate[newUpdate.Length-1].Message.Text;
                    while (newUpdate == null || tgbot._textValue=="/find")
                    {
                       Thread.Sleep(1000);
                        newUpdate = await botClient.GetUpdatesAsync();
                        tgbot._textValue=newUpdate[newUpdate.Length-1].Message.Text;
                    }  
                        Console.WriteLine($"Current MessageText is -> {findingGame._field}");
                    
                    //}
                    findingGame._field=tgbot._textValue;
                    Console.WriteLine($"Now MessageText is -> {findingGame._field}");
                    await botClient.SendTextMessageAsync(chatId, $"Bot searching now: {tgbot._textValue}");

                    

                    //findingGame.FindTheGame(findingGame._field);
                }

                // Echo received message text


            }
           async Task AdditionalHandeofUpdates (ITelegramBotClient newbot, Update update, CancellationToken cancellation)
            {
                var up = update.Message;
                if (up is not { } message)
                    return;
                if (message.Text is not { } messageText)
                    return;
                _insertText=messageText;
                await newbot.SendTextMessageAsync(up.Chat.Id,"Message from new handler");
                

                Console.WriteLine("Workin additional handler!!");
                Console.WriteLine(_insertText);
               
                Message sentMessage_new = await botClient.SendTextMessageAsync(
                          chatId: update.Message.Chat.Id,
                          text: "Finding now: ",
                          cancellationToken: cancellation);
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

