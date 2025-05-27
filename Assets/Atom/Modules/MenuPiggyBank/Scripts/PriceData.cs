using System;
using Newtonsoft.Json.Linq;

namespace Atom
{
    public class PriceData
    {
        public CurrencyType Type;
        public int Amount;
        public int? CrossPrice;

        public PriceData() { }
        public PriceData(JObject jo)
        {
            Type = StringToCurrencyType(jo.SelectToken("price_type").Value<string>());
            Amount = jo.SelectToken("amount").Value<int>();
            CrossPrice = jo.SelectToken("cross_price")?.Value<int>();
        }

        public enum CurrencyType
        {
            Coin = 1,
            RM = 2
        }

        public static CurrencyType StringToCurrencyType(string str)
        {
            if (str.CompareTo("coin") == 0)
            {
                return CurrencyType.Coin;
            }
            if (str.CompareTo("rm") == 0)
            {
                return CurrencyType.RM;
            }
            throw new System.InvalidOperationException();
        }
    }
}