using System.Threading.Tasks;




namespace tgbot
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

