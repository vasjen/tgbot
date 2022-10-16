using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Parsing
{
    public class FindingGame
    {
        public string _field { get; set; }
        public FindingGame(string obj)
        {
            this._field = obj;
        }


        public async void FindTheGame(string _searichg)
        {
            
           
            if (_searichg.Length != 0)
            {
                ChromeOptions options = new ChromeOptions();

               
                IWebDriver driver = new ChromeDriver();

                driver.Url = @"https://www.xbox.com/tr-TR/games/all-games";
                List<GameCard> Result = new List<GameCard>();
                
             


                while (driver.FindElements(By.ClassName("m-product-placement-item")).Count == 0) //While dont exist gamecard in catalog script dont run a search and still waiting
                {
                     await Task.Delay(1000);
                    
                }



                driver.FindElement(By.XPath("//input[contains(@name,'search-field')]")).SendKeys(_searichg.ToLower());
                 await Task.Delay(1000);
                driver.FindElement(By.XPath("//button[@data-bi-id='n1c1m1r4a3'] ")).Click();



                int countOfSearchingResult = driver.FindElements(By.ClassName("c-subheading-4")).Count;
                if (countOfSearchingResult != 0)
                {

                    var _resultName = driver.FindElements(By.ClassName("c-subheading-4")); //get <div> block for extracting Title of the game.
                    var _resultPrice = driver.FindElements(By.ClassName("m-product-placement-item")); //get <div> block for extracting listed price of the game.
                    var _links = driver.FindElements(By.ClassName("gameDivLink")); //get <div> block for extracting link of the game.


                    for (int i = 0; i < countOfSearchingResult; i++)
                    {
                        
                        Result=GameCard.CreatingList(_resultName[i].Text, _links[i].GetAttribute("href"),Convert.ToDouble(_resultPrice[i].GetAttribute("data-listprice")));
                        Console.WriteLine($"|{i + 1}|\t{_resultName[i].Text} - {_resultPrice[i].GetAttribute("data-listprice")}| {_links[i].GetAttribute("href")}");
                    }
                    Console.WriteLine("Обращение через foreach  и Result ");

                    driver.Close();


                }
                else
                {
                    Console.WriteLine("Dont exist the game in catalog this enter name");
                    driver.Close();
                    
                }
            }
            else
            {
                Console.WriteLine("Seaching string is empty");
               
            }
            

        }

      /*  public string ToTelegramMessage(List<GameCard> obj)
        {
            string info = "";
            foreach (var item in obj)
            {
                info = item.title + Convert.ToString(item.price);
                return info;
            }
            return info;
        }
      */
    }
}

