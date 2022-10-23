using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Telegram.Bot.Types.Payments;

namespace TeleGramBot
{
    public class GameCard
    { 
        public string link, title;
        private int price_int;
        public double price;
        static List<GameCard> instanse = new List<GameCard>();
        public GameCard()
        {

        }
        public GameCard(string Name, string Desc, double price)
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

        internal static List<GameCard> CreatingList(string name, string link, double price)
        {
            GameCard gameCard = new GameCard(name, link, price);
            instanse.Add(gameCard);
            return instanse;
        }

          
    }
}

