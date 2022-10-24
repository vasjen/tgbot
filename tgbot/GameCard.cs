using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types.Payments;

namespace TeleGramBot
{
    public class GameCard
    { 
        public string link, title, price, photo;
        private double price_int;
        public int price_USD;
        
        static List<GameCard> instanse = new List<GameCard>();
        public GameCard()
        {

        }
        public GameCard(string Name, string Desc, string price)
        {
            this.title = Name;
            this.link = Desc;
            this.price = price;
        }
        public GameCard(string Name, int price)
        {
            this.title = Name;
            
            this.price_int = price;
        }
        
        

          
    }
}

