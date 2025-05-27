using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Atom
{
    public class RewardData : IEquatable<RewardData>
    {
        public RewardType Type;
        public string Id;
        public int Amount;
        public int? CrossPrice;

        public RewardData() { }

        public RewardData(JObject jo)
        {
            Type = StringToRewardType(jo.SelectToken("reward_type").Value<string>());
            Id = jo.SelectToken("id").Value<string>();
            Amount = jo.SelectToken("amount").Value<int>();
            CrossPrice = jo.SelectToken("cross_price")?.Value<int>();
        }

        public enum RewardType
        {
            Booster = 1,
            Currency = 2,
            NoAds = 3,
            LimitedTimeBooster = 4,
            Moves = 5
        }

        public static RewardType StringToRewardType(string str)
        {
            if (str.CompareTo("booster") == 0)
            {
                return RewardType.Booster;
            }
            if (str.CompareTo("currency") == 0)
            {
                return RewardType.Currency;
            }
            if (str.CompareTo("no-ads") == 0)
            {
                return RewardType.NoAds;
            }
            if (str.CompareTo("time_booster") == 0)
            {
                return RewardType.LimitedTimeBooster;
            }
            if (str.CompareTo("moves") == 0)
            {
                return RewardType.Moves;
            }
            throw new System.InvalidOperationException();
        }

        public static IList<RewardData> SimplifyRewardList(IList<RewardData> rewards)
        {
            var list = new List<RewardData>();

            foreach (var rw in rewards)
            {
                if (list.Contains(rw))
                {
                    var item = list.Find(c => c.Equals(rw));
                    item.Amount += rw.Amount;
                    item.CrossPrice += rw.CrossPrice;
                }
                else
                {
                    list.Add(rw);
                }
            }
            return list;
        }

        public bool Equals(RewardData other)
        {
            return Type == other.Type && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RewardData)obj);
        }

        public override int GetHashCode()
        {
            unchecked //Ignores overflows that can (should) occur
            {
                var hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}