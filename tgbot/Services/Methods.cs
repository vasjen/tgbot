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
        private static string _login, _password;
        internal static IWebDriver GetWebDriver()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.EnableVerboseLogging = false;
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;


            ChromeOptions options = new ChromeOptions();

            options.PageLoadStrategy = PageLoadStrategy.Normal;

            options.AddArgument("--no-sandbox");
            
            options.AddArgument("--window-size=500,658");
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
                        card.link="https://www.xbox.com/tr-TR/games/store/p/"+card.uid;

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

        public static void Authentification(IWebDriver driver)
        {
            _login="Insert login from Microsoft store";
            _password="Insert password";

            driver.Url="https://account.xbox.com/account/signin";
            var Login = driver.FindElement(By.XPath("//input[@id='i0116']"));
            while (Login == null)
            {
                Thread.Sleep(2000);
            }
            driver.FindElement(By.XPath("//input[@id='i0116']")).SendKeys(_login);
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//input[@id='idSIButton9']")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//input[@id='i0118']")).SendKeys(_password);
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//input[@id='i0118']")).Submit();
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//input[@id='idSIButton9']")).Submit();
        }
        public static void BuyTheGame(TeleGramBotClass client )
        {
            var driver = GetWebDriver();
            Authentification(driver);
            driver.Url = client.link;
            Thread.Sleep(5000);
            driver.FindElement(By.XPath("(//button[contains(@class,'Nv0Hx')])[3]")).Click();

            driver.FindElement(By.XPath("//button[contains(.,'Hediye olarak')]")).Click();
           




            try
            {   Thread.Sleep(5000);
                driver.SwitchTo().Frame("purchase-sdk-hosted-iframe");
                
                driver.SwitchTo().ActiveElement().SendKeys($"{client.email}");
                driver.SwitchTo().ActiveElement().SendKeys(Keys.Tab);

                
                driver.SwitchTo().ActiveElement().SendKeys("TelegramBot");

                driver.SwitchTo().ActiveElement().SendKeys(Keys.Enter);
                Thread.Sleep(5000);

                driver.SwitchTo().ActiveElement().SendKeys(Keys.Enter);

             }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
