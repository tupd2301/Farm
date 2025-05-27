using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Atom
{
    public class PiggyBankConfig
    {
        public readonly int UnlockLevel;
        public readonly IList<PiggyBankLevelData> Levels;
        public readonly IList<PiggyBankModeRewardData> Modes;
        public readonly IList<string> StageDescriptions;

        public PiggyBankConfig(string json)
        {
            var jo = JObject.Parse(json);
            UnlockLevel = jo.SelectToken("unlock_level").Value<int>();
            //Levels = jo.ParseJObjectList("levels",
            //    level => new PiggyBankLevelData(level.ToObject<JObject>()));
            //Modes = jo.ParseJObjectList("modes",
            //    mode => new PiggyBankModeRewardData(mode.ToObject<JObject>()));
            //StageDescriptions = jo.ParseJObjectList("stage_descriptions", stageDescription => stageDescription.Value<string>());
        }
    }

    public class PiggyBankModeRewardData
    {
        public readonly string ModeId;
        public readonly IList<int> CoinRewardPerStars;

        public PiggyBankModeRewardData(JObject jo)
        {
            ModeId = jo.SelectToken("mode_id").Value<string>();
            //CoinRewardPerStars = jo.ParseJObjectList("coin_reward_per_stars",
            //    coinRewardPerStar => coinRewardPerStar.Value<int>());
        }
    }

    public class PiggyBankLevelData
    {
        public readonly int Level;
        public readonly string ProductId;
        public readonly string Name;
        public readonly int MaxCoin;
        public readonly int PurchasablePercent;

        public PiggyBankLevelData(JObject jo)
        {
            Level = jo.SelectToken("level").Value<int>();
            ProductId = jo.SelectToken("product_id").Value<string>();
            Name = jo.SelectToken("name").Value<string>();
            MaxCoin = jo.SelectToken("max_coin").Value<int>();
            PurchasablePercent = jo.SelectToken("purchasable_percent").Value<int>();
        }
    }
}
