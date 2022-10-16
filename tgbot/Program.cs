using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Args;
using System.Threading;
using Parsing;



namespace TeleGramBot
{
    class Program
    { 
        private static string _botToken { get; set; }

        
        static async Task Main(string[] args)
        {
            await TeleGramBotClass.Run();
        }


    }
}

