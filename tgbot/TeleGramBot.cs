using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using tgbot.services;

namespace tgbot
{

    internal class TeleGramBotClass
    {

        
        public long _idOfSender;
        public string _textValue;
        public int _currentposition;
        public int _idOfPhotoMessage;
        public string email;
        public string link;
        public ITelegramBotClient botClient;
        public Update update;
        public List<GameCard> gameCards;

        
        public TeleGramBotClass()
        {

        }
        
        private static string _botToken { get; set; }
        

        internal static async Task Run()
        {
            TeleGramBotClass client = new TeleGramBotClass();

            
            _botToken = "Insert API Token";
            var cts = new CancellationTokenSource();
            var botClient = new TelegramBotClient(_botToken);
            client.botClient=botClient;
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
                    cancellationToken: cts.Token);

            var me = await botClient.GetMeAsync();
                      Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            // Send cancellation request to stop bot
            cts.Cancel();
            

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {

                Methods methods = new Methods();
                client.update=update;
                //Console.WriteLine($"Message type is {update.Message.Type}");

                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                {
                   
                    if (update.CallbackQuery != null)
                    {
                        await BotCommands.GetCallback(client);
                    }
                    if (update.PreCheckoutQuery != null)
                    {
                        await BotCommands.PrechekOut(client);
                    }
                    return; }

                client._idOfSender=message.Chat.Id;

                if (message.Type == MessageType.SuccessfulPayment)
                { Console.WriteLine($"Successful buy!" +
                    $"Amount of buy:\t {message.SuccessfulPayment.TotalAmount} \n" +
                    $"Order of Payment:\n " +
                    $"Id of payment:\t {message.SuccessfulPayment.TelegramPaymentChargeId}\n" +
                    $"Id of provider:\t {message.SuccessfulPayment.ProviderPaymentChargeId}\n" +
                    $"Invoice:\t {message.SuccessfulPayment.InvoicePayload}\n" +
                    $"Code send to:\t {message.SuccessfulPayment.OrderInfo.Email}");
                    client.email=message.SuccessfulPayment.OrderInfo.Email.Clone() as string;
                    
                   Methods.BuyTheGame(client);
                }
                   




                // Only process text messages
                if (message.Text is not { } messageText)
                return;

               
                client._textValue=messageText;
               
                var chatId = message.Chat.Id;

                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
               
                if (messageText.Contains("/find"))
                {
                    
                    Message asked = await botClient.SendTextMessageAsync(chatId: chatId, text: "Whats the game are you searching?");
                    var newUpdate = await  botClient.GetUpdatesAsync();
                    if (newUpdate[newUpdate.Length-1].Message!=null)
                        client._textValue=newUpdate[newUpdate.Length-1].Message.Text;
                    while (newUpdate == null || client._textValue=="/find")
                    {
                       Thread.Sleep(1000);
                        newUpdate = await botClient.GetUpdatesAsync();
                        if (newUpdate[newUpdate.Length-1].Message!=null)
                        client._textValue=newUpdate[newUpdate.Length-1].Message.Text;
                    }  

                    Console.WriteLine($"Now MessageText is -> {client._textValue}");
                    await botClient.SendTextMessageAsync(chatId, $"Bot searching now: {client._textValue}");

                    //methods.AddPromo(methods.FindTheGames(client._textValue)
                    
                    client.gameCards =methods.FindTheGames(client._textValue);
                    await 
                        methods.ShowResultsOfFinding(client);
                   // var t =  methods.ShowResultsOfFinding(client);
                  
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

