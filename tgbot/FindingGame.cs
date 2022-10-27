using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace TeleGramBot
{
    public class FindingGame
    {
        public string _field { get; set; }
        public FindingGame()
        {
          
        }
        public  delegate void UpdateString(string value);

        public GameCard[] FindTheGame(string _searichg)
        {

            ChromeOptions options = new ChromeOptions();


            IWebDriver driver = new ChromeDriver();

            driver.Url = @"https://www.xbox.com/tr-TR/games/all-games";

            if (_searichg.Length != 0)
            {
                while (driver.FindElements(By.ClassName("m-product-placement-item")).Count == 0) //While dont exist gamecard in catalog script dont run a search and still waiting
                {
                     Thread.Sleep(1000);
                    
                }



                driver.FindElement(By.XPath("//input[contains(@name,'search-field')]")).SendKeys(_searichg.ToLower());
                 Thread.Sleep(1000);
                driver.FindElement(By.XPath("//button[@data-bi-id='n1c1m1r4a3'] ")).Click();



                int countOfSearchingResult = driver.FindElements(By.ClassName("c-subheading-4")).Count;
                if (countOfSearchingResult != 0)
                {

                    var _resultName = driver.FindElements(By.ClassName("c-subheading-4")); //get <div> block for extracting Title of the game.
                    var _resultPrice = driver.FindElements(By.ClassName("m-product-placement-item")); //get <div> block for extracting listed price of the game.
                    var _links = driver.FindElements(By.ClassName("gameDivLink")); //get <div> block for extracting link of the game.
                    var _photoParent = driver.FindElements(By.ClassName("containerIMG"));
                    
                    Console.WriteLine($"Finded images: {_photoParent.Count}");

                    GameCard[] catalog = new GameCard[countOfSearchingResult];

                    for (int i = 0; i < countOfSearchingResult; i++)
                    {
                        GameCard tempGameCard = new GameCard();
                        tempGameCard.title=_resultName[i].Text;
                        tempGameCard.price=_resultPrice[i].GetAttribute("data-listprice");
                        tempGameCard.link=_links[i].GetAttribute("href");
                        var _photo = _photoParent[i].FindElement(By.ClassName("c-image")); 
                        tempGameCard.photo=  _photo.GetAttribute("src");
                        catalog[i] = tempGameCard;

                        //Result=GameCard.CreatingList(_resultName[i].Text, _links[i].GetAttribute("href"),Convert.ToDouble(_resultPrice[i].GetAttribute("data-listprice")));
                        Console.WriteLine($"|{i + 1}|\t{_resultName[i].Text} - {_resultPrice[i].GetAttribute("data-listprice")}| " +
                            $"{_links[i].GetAttribute("href")} - {_photo.GetAttribute("src")}");
                        
                    
                    }

                    Console.WriteLine($"Всего было найдено {catalog.Length} игр");
                    driver.Quit();
                    return catalog;

                }
                else
                {
                    Console.WriteLine("Dont exist the game in catalog this enter name");
                    GameCard[] emptyCatalog = new GameCard[0];
                    driver.Quit();
                    return emptyCatalog;
                   
                    

                }
               

            }
            else
            {
                Console.WriteLine("Seaching string is empty");
                GameCard[] emptyCatalog = new GameCard[0];
                return emptyCatalog;
            }

            

            

        }

     
    }
}

