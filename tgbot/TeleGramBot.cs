﻿using System;
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
using System.Runtime.CompilerServices;
using System.Transactions;

namespace TeleGramBot
{

    public class TeleGramBotClass
    {

        private int _idOfMessage;
        private long _idOfSender;
        private string _textValue;
        private int _currentposition;
        private GameCard[] gameCards;
        private int _idOfPhotoMessage;
        private  enum _status { first, last, normal}
        public TeleGramBotClass()
        {
        }
        private static string _botToken { get; set; }
        
        private static string _insertText { get; set; }
        
        


        internal static async Task Run()
        {
            TeleGramBotClass tgbot = new TeleGramBotClass();

            //Console.WriteLine("Insert API Token");
            _botToken = "5744464072:AAG2YTypfSV4PwWt7MlOnaB58SjvqLOlUSw";
            var cts = new CancellationTokenSource();
            var botClient = new TelegramBotClient(_botToken);
            var newbot = new TelegramBotClient(_botToken);

             int ConvertPriseToUSD(string text)
            {
                int result = 0;
                if (text != null)
                {
                    text=text.Replace(".",",");
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
            static InlineKeyboardMarkup CreatingButtons()
            {

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

                InlineKeyboardMarkup _menu = new InlineKeyboardMarkup(new[] { _1stRow, _2ndRow }); ;
                return _menu;
            }

            async Task ShowResultsOfFinding(GameCard[] obj, int position, ITelegramBotClient botclient, long chatID)
            {
               
                int count = obj.Length;

                Message showResult = await botClient.SendPhotoAsync(chatID, 
                    photo: obj[position].photo, 
                    caption: $"{position+1} of {count}\n {obj[position].title}" +
                    $" {obj[position].price} ₺",
                    replyMarkup: CreatingButtons());
                tgbot._idOfPhotoMessage=showResult.MessageId;


               // await botClient.EditMessageCaptionAsync();

                

            }
            async Task NavigationButtons()
            {

                await botClient.EditMessageMediaAsync(tgbot._idOfSender, tgbot._idOfPhotoMessage, new InputMediaPhoto(media: $"{tgbot.gameCards[tgbot._currentposition].photo}"));
                await botClient.EditMessageCaptionAsync(tgbot._idOfSender, tgbot._idOfPhotoMessage, $"" +
                   $"{tgbot._currentposition+1} of {tgbot.gameCards.Length}\n " +
                   $"{tgbot.gameCards[tgbot._currentposition].title} - " +
                   $"{tgbot.gameCards[tgbot._currentposition].price} ₺", replyMarkup: CreatingButtons());
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
                 
                GameCard[] result = null;
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                {   
                    //tgbot._idOfSender=update.Message.Chat.Id;
                    if (update.CallbackQuery != null)
                    {
                        Console.WriteLine("Register callback!");
                        await botClient.AnswerCallbackQueryAsync(callbackQueryId: update.CallbackQuery.Id);
                        Console.WriteLine("Answer send to -> {0}\n" +
                            "User choise is {1}", update.CallbackQuery.Id, update.CallbackQuery.Data);
                        if (update.CallbackQuery.Data.ToLower()=="invoice")
                        {
                            int PriceToInvoice = ConvertPriseToUSD(tgbot.gameCards[tgbot._currentposition].price);
                           
                            LabeledPrice game1 = new LabeledPrice($"{tgbot.gameCards[tgbot._currentposition].title}", PriceToInvoice);
                            LabeledPrice game2 = new LabeledPrice("Service fee: 1$", 100);

                            LabeledPrice[] catalog = new LabeledPrice[2];
                            catalog[0] = game1;
                            catalog[1] = game2;
                            Message sendInvoice=await botClient.SendInvoiceAsync(chatId: update.CallbackQuery.From.Id,
                                              title: $"{tgbot.gameCards[tgbot._currentposition].title}",
                                              description: "Redem code will be send to this email!",
                                              payload: "somePayload",
                                              providerToken: "1877036958:TEST:c0c0f684e8b1c6968e6d66a6ed77d2cd46f8be4a",
                                              //providerToken: "1877036958:TEST:c0c0f684e8b1c6968e6d66a6ed77d2cd46f8be4a",
                                              currency: "USD",
                                              prices: catalog,
                                              needEmail: true,
                                              startParameter: "exapmle",
                                              isFlexible: false,
                                              photoUrl: $"{tgbot.gameCards[tgbot._currentposition].photo}"
                                              );
                        }
                        if (update.CallbackQuery.Data.ToLower()=="previous")
                        {

                            if (tgbot._currentposition >0)
                            {
                                tgbot._currentposition--;
                                await NavigationButtons();
                            }
                        }
                        if (update.CallbackQuery.Data.ToLower()=="next")
                        {
                            if (tgbot._currentposition <tgbot.gameCards.Length-1)
                            {
                                tgbot._currentposition++;
                                await NavigationButtons();
                            }
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

    /*                if (messageText.ToLower().Contains("menu"))
                {

                               Message sentExample = await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: "https://store-images.s-microsoft.com/image/apps.55934.13550335257385479.f907e8a1-c727-4bed-9e2c-94c239249dba.b5fd70da-71e5-4849-b499-25c43d8c9a25?q=90&w=177&h=265",
                                caption: "Name of the this Game\n Price 45$",
                                replyMarkup: CreatingButtons());
                    _idOfPhotoMessage = sentExample.MessageId;





                }
                */




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



                      result = findingGame.FindTheGame(findingGame._field);
                    tgbot.gameCards= (GameCard[])result.Clone(); 

                    await botClient.SendTextMessageAsync(chatId, $"Result of searching: {result.Length} Games\n");
                    if (result.Length > 0)
                        tgbot._currentposition=0;
                    tgbot._idOfSender=chatId;
                       await ShowResultsOfFinding(tgbot.gameCards,tgbot._currentposition,botClient, tgbot._idOfSender);
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

