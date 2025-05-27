using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Atom
{
    public class DailyFreeCoinsConfig
    {
        public int CoinCount;
        public List<int> LuckyMultipliers;
        public string RemainDescription;
        public string RemainLastDescription;
    }
}
