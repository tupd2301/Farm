using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    [CreateAssetMenu(fileName = "ItemDataSO", menuName = "ItemDataSO")]
    public class ItemDataSO : ScriptableObject
    {
        public List<ItemData> itemDataList;

        void OnValidate()
        {
            foreach (var item in itemDataList)
            {
                item.itemId = itemDataList.IndexOf(item);
                if (!item.isCustom)
                {
                    continue;
                }
                foreach (var mergeableItem in item.mergeableItems)
                {
                    var mergeableItemData = itemDataList.Find(x =>
                        x.itemId == mergeableItem.itemId
                    );
                    if (mergeableItemData != null)
                    {
                        if (
                            mergeableItemData.mergeableItems.Find(x => x.itemId == item.itemId)
                            == null
                        )
                        {
                            mergeableItemData.mergeableItems.Add(
                                new MergeableItem()
                                {
                                    itemId = item.itemId,
                                    amount = mergeableItem.amount,
                                    outputItemId = mergeableItem.outputItemId,
                                }
                            );
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class ItemData
    {
        public int itemId;
        public int gearId;
        public string itemName;
        public string iconName;
        public float cost;
        public float size;

        public bool isCustom;

        public List<MergeableItem> mergeableItems;

        public void Copy(ItemData other)
        {
            gearId = other.gearId;
            itemId = other.itemId;
            itemName = other.itemName;
            iconName = other.iconName;
            cost = other.cost;
            size = other.size;
            mergeableItems = other.mergeableItems;
        }
    }

    [System.Serializable]
    public class MergeableItem
    {
        public int itemId;
        public int amount;
        public int outputItemId;
    }
}
