using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types.Payments;

namespace tgbot
{
    internal class GameCard
    { 
        private string _link, _title, _price, _photo, _uid;
        
        private int _price_USD,_intprice;
        private bool _promo=false;
        public bool promo { get { return _promo; } set { _promo = value; } }
        public string link { get { return _link; } set { _link = value; } }
        public string title { get { return _title; } set { _title = value; } }
        public string price { get { return _price; } set { _price = value; } }
        public string photo { get { return _photo; } set { _photo = value; } }
        public string uid { get { return _uid; } set { _uid = value; } }
        public int price_USD { get { return _price_USD; } set { _price_USD = value; } }
        public int intPrice { get { return _intprice; } set { _intprice = value; } }
        
        
        public GameCard()
        {

        }

     

    }
}

