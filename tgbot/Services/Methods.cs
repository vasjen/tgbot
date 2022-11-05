using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace tgbot.services
{
    internal class Methods

    {   
        internal static IWebDriver GetWebDriver()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.EnableVerboseLogging = false;
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;


            ChromeOptions options = new ChromeOptions();

            options.PageLoadStrategy = PageLoadStrategy.Normal;

            options.AddArgument("--no-sandbox");
           // options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-crash-reporter");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-in-process-stack-traces");
            options.AddArgument("--disable-logging");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--log-level=3");


            IWebDriver driver = new ChromeDriver(options);
            
            return driver;
        }

        public List<GameCard> FindTheGames(string _searching)
        {
            List<GameCard> ResultList = new List<GameCard>();
            var driver = GetWebDriver();
            if (_searching.Length != 0)
            {
                driver.Navigate().GoToUrl("https://xbox.com/tr-TR/games/all-games");
                while (driver.FindElements(By.ClassName("m-product-placement-item")).Count == 0) //While dont exist gamecard in catalog script dont run a search and still waiting
                {
                    Thread.Sleep(1000);
                    // Waiting a catalog loading
                }

                driver.FindElement(By.XPath("//input[contains(@name,'search-field')]")).SendKeys(_searching.ToLower());
                Thread.Sleep(1000);
                driver.FindElement(By.XPath("//button[@data-bi-id='n1c1m1r4a3'] ")).Click();


                // Count of a games was found 
                int countOfSearchingResult = driver.FindElements(By.ClassName("c-subheading-4")).Count;
                if (countOfSearchingResult != 0)
                {

                    var _resultName = driver.FindElements(By.ClassName("c-subheading-4")); //get <div> block for extracting Title of the game.
                    var _resultPrice = driver.FindElements(By.ClassName("m-product-placement-item")); //get <div> block for extracting listed price of the game.
                    var _links = driver.FindElements(By.ClassName("gameDivLink")); //get <div> block for extracting link of the game and label with the price of the game.
                    var _photoParent = driver.FindElements(By.ClassName("containerIMG"));


                    //Saving all the games card in List<>.
                    for (int i = 0; i < countOfSearchingResult; i++)
                    {
                        GameCard card = new GameCard();
                        card.promo = false;
                        card.title = _resultName[i].Text;

                        card.photo=_photoParent[i].FindElement(By.ClassName("c-image")).GetAttribute("src");
                        card.uid = _resultPrice[i].GetAttribute("data-bigid");


                        try
                        {
                            card.price=_links[i].FindElement(By.ClassName("textpricenew")).Text.TrimStart('₺');

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{card.title} -> Prob dont have a label with price");
                            Console.WriteLine(ex.Message);
                        }

                        ResultList.Add(card);

                    }

                    //Now removing all a cards with incorrect price.
                    for (int i = 0; i<ResultList.Count; i++)
                    {
                        if ((ResultList[i].price==null | ResultList[i].price=="0,00")==true)
                        {
                            ResultList.Remove(ResultList[i]);
                            i--;
                        }

                    }

                  

                }

            }
            driver.Quit();
            return ResultList;
        }
       
        public async Task ShowResultsOfFinding(TeleGramBotClass client)
        {

            int currentposition = 0; 
            if (client.gameCards.Count != 0)
            {
                
                Message showResult = await client.botClient.SendPhotoAsync(client.update.Message.From.Id,
                    photo: client.gameCards[currentposition].photo,
                    caption: $"{currentposition+1} of {client.gameCards.Count}\n {client.gameCards[currentposition].title}" +
                    $"for  ${Math.Round(Convert.ToDouble(client.gameCards[currentposition].price)*0.054,2)}",
                    replyMarkup: BotCommands.CreatingButtons());
                client._idOfPhotoMessage=showResult.MessageId;
            }
            else
            {
                Message noRessult = await client.botClient.SendTextMessageAsync(client.update.Message.From.Id, $"Bot can't find the game");
            }

        }
         
    }
}
