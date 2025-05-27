using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Atom
{
    public enum StorageAction
    {
        Set = 0,
        Add = 1,
        Remove = 2,
        Delete = 3
    }

    [Serializable]
    public class UserData
    {
        public string UserId;
        public string UserName;
        public string UserAvatar;

        public UserData()
        {
            UserId = "";
            UserName = "";
            UserAvatar = "";
        }
    }

    [Serializable]
    public class UserStorage
    {
        public event Action<int> OnCoinUpdated;

        public Dictionary<ItemId, Item> Items;

        public UserStorage()
        {
            Items = new Dictionary<ItemId, Item>();
        }

        public Item GetItem(ItemId id)
        {
            if (!Items.ContainsKey(id))
            {
                return null;
            }
            return Items[id];
        }

        public int GetQuantity(ItemId id)
        {
            if (!Items.ContainsKey(id))
            {
                return 0;
            }
            return Items[id].Quantity;
        }

        public void SetQuantity(ItemId id, int quantity, StorageAction action = StorageAction.Set)
        {
            if (!Items.ContainsKey(id))
            {
                return;
            }

            switch (action)
            {
                case StorageAction.Set:
                    Items[id].Quantity = quantity;
                    break;
                case StorageAction.Add:
                    Items[id].Quantity += quantity;
                    break;
                case StorageAction.Remove:
                    Items[id].Quantity = Math.Clamp(Items[id].Quantity - quantity, 0, Items[id].Quantity);
                    break;
                case StorageAction.Delete:
                    Items.Remove(id);
                    break;
            }

            if (Items[id].Id == ItemId.Coin)
            {
                OnCoinUpdated?.Invoke(Items[id].Quantity);
            }
        }

        public ItemCategory GetCategory(ItemId id)
        {
            if (!Items.ContainsKey(id))
            {
                return ItemCategory.None;
            }
            return Items[id].Category;
        }
    }

    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemCategory
    {
        None = -1,
        Item = 0,
        Currency = 1,
        Booster = 2
    }

    [Serializable]
    public enum ItemId
    {
        Coin = 1000,
        Star = 1001,
        Heart = 1002,
        Rotate = 2000,
        Hammer = 2001,
        Magnet = 2002,
        Filler = 2003,
        RemoveAds = 10000,
    }

    [Serializable]
    public class Item
    {
        public ItemId Id;
        public ItemCategory Category;
        public int Quantity;
    }
}