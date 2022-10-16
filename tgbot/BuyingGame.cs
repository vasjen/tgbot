using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;


namespace Parsing
{
    
    public  class BuyingGame
    {
         string _link;
        string _email;

        public BuyingGame(string value)
        {
            _link = value;
        }
        

        
        public  void Authentification(IWebDriver driver)
        {

            
            
            var Login = driver.FindElement(By.XPath("//input[@id='i0116']"));
            while (Login == null)
            {
                 Thread.Sleep(2000);
            }
            driver.FindElement(By.XPath("//input[@id='i0116']")).SendKeys("vasjen.turkey@outlook.com");
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//input[@id='idSIButton9']")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//input[@id='i0118']")).SendKeys("tostUc-mydbi0-fantef");
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//input[@id='i0118']")).Submit();
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//input[@id='idSIButton9']")).Submit();
        }

        public  void BuyTheGme(IWebDriver driver)
        {
            var status = driver;

            

            Thread.Sleep(5000);
                driver.FindElement(By.XPath("(//button[contains(@class,'Nv0Hx')])[3]")).Click();





            driver.FindElement(By.XPath("//button[contains(.,'Hediye olarak satın al')]")).Click();
            Thread.Sleep(8000);

            driver.SwitchTo().Frame(2);
              
            driver.SwitchTo().ActiveElement().SendKeys("im_vast@mail.ru");

            driver.SwitchTo().ActiveElement().Submit();
            
            Thread.Sleep(2000);
            driver.SwitchTo().ActiveElement().SendKeys("from service");

            driver.SwitchTo().ActiveElement().Submit();
            Thread.Sleep(5000);



            driver.FindElement(By.XPath("//button[@id='confirmButton']")).Submit();



        }

        
    }
}

