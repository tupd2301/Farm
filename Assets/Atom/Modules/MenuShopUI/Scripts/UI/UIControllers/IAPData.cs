using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Atom
{
    [System.Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IAPType
    {
        None = -1,
        Consumable = 0,
        NonConsumable = 1,
        AutoRenewableSubscription = 2,
        NonRenewingSubscription = 3,
        Free = 4,
        InGame = 5
    }

    [System.Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IAPBundleType
    {
        None = -1,
        Pack = 0,
        NoAds = 1,
        Bundle = 2,
        SBundle = 3,
        GBundle = 4
    }

    [System.Serializable]
    public class IAPBundle
    {
        public string BundleId;
        public string StoreBundleId;
        public string BundleName;
        public bool IsSelling;
        public IAPType Type;
        public IAPBundleType BundleType;
        public Dictionary<string, string> Layout;
        public string Price;
        public string Currency;
        public List<Item> Payout;
        public List<string> ChildBundles;
    }

    [System.Serializable]
    public class SBundleOfferData
    {
        public string BundleId;
        public long Start = 0;
        public long End = 0;
    }
}