using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Athena.Common;
using TMPro;

namespace Atom
{
    public class IAPCompletedUI : ConfirmUI
    {
        public const string TAG_KEY = "Tag";

        [SerializeField]
        private RectTransform _itemContent;
        [SerializeField]
        private ItemUI[] _items;

        private IAPBundle _bundle;
        public IAPBundle Bundle { get { return _bundle; } }

        public void SetupItems(IAPBundle bundle)
        {
            _bundle = bundle;

            int coinIndex = 0;
            string iconKey = Global.GameConfig.IAP_LAYOUT_KEY_PACK_ICON;
            if (_bundle.Layout.ContainsKey(iconKey))
            {
                string coinIcon = _bundle.Layout[iconKey];
                coinIndex = coinIcon switch
                {
                    "s" => 0,
                    "m" => 1,
                    "l" => 2,
                    _ => 0
                };
            }
            if (_bundle.Payout != null && _bundle.Payout.Count > 0)
            {
                List<Item> items = _bundle.Payout;

                float width = _itemContent.sizeDelta.x;
                float height = _itemContent.sizeDelta.y;
                int itemCount = (int)Mathf.Min(items.Count, _items.Length);
                int maxItemInRow = 3;
                int maxRow = 3;
                int rowCount = Mathf.FloorToInt(Mathf.Clamp((itemCount - 1) / maxItemInRow + 1, 1, maxRow));
                Debug.Log("rowCount = " + rowCount);
                float deltaY = rowCount > 1 ? height / (rowCount) : 0;
                float deltaX = width / (maxItemInRow);
                int firstRowCount = itemCount % maxItemInRow;
                for (int i = 0; i< _items.Length; i++)
                {
                    int index = i < firstRowCount ? i : i + (maxItemInRow - firstRowCount);
                    int x = index % maxItemInRow;
                    int y = index / maxItemInRow;

                    Debug.Log("x = " + x);
                    Debug.Log("y = " + y);
                    int itemInRow = i < firstRowCount ? firstRowCount : maxItemInRow;
                    float posX = (x - (itemInRow - 1) * 0.5f) * deltaX;
                    float posY = ((rowCount - 1)* 0.5f - y) * deltaY;

                    Debug.Log("posX = " + posX);
                    Debug.Log("posY = " + posY);
                    if (i >= items.Count)
                    {
                        _items[i].gameObject.SetActive(false);
                        continue;
                    }
                    _items[i].gameObject.SetActive(true);
                    _items[i].SetupSmallIcon(items[i], "x{0:N0}");
                    if (items[i].Id == ItemId.Coin)
                    {
                        //_items[i].SetIconImage(ResourceManager.Instance.GetCoinPackSpriteByIndex(coinIndex));
                    }
                    RectTransform rt = _items[i].GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(posX, posY);
                }
            }
        }
    }
}