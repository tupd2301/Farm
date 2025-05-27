using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Factory
{
    public class ItemController : MonoBehaviour
    {
        public bool mergeable = false;
        public bool isCollected = false;

        [SerializeField]
        private SpriteRenderer _itemIcon;

        public ItemData itemData;

        void OnEnable()
        {
            isCollected = false;
            mergeable = false;
            _itemIcon.gameObject.SetActive(true);
            Invoke(nameof(SetMergeable), 1f);
        }

        void Update()
        {
            if (transform.position.y < -10)
            {
                GameManager.Instance.CollectItem(this);
            }
        }

        private void SetMergeable()
        {
            mergeable = true;
        }

        public void SetItemData(ItemData itemData, float cost = 0)
        {
            this.itemData.Copy(itemData);
            if (cost > 0)
            {
                this.itemData.cost = cost;
            }
            UpdateItem();
        }

        public void UpdateItem()
        {
            if (itemData == null)
            {
                return;
            }
            var sprite = Resources.Load<Sprite>("Sprites/" + itemData.iconName);
            if (sprite != null)
            {
                _itemIcon.sprite = sprite;
            }
            transform.localScale = Vector3.one * itemData.size;
        }

        public void Dissolve()
        {
            GetComponent<Collider2D>().enabled = false;
            var material = GetComponent<SpriteRenderer>().material;
            material.SetFloat("_Dissolve", 1f);
            _itemIcon.gameObject.SetActive(false);
            material
                .DOFloat(0f, "_Dissolve", 0.4f)
                .OnComplete(() =>
                {
                    GameManager.Instance.CollectItem(this);
                });
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (transform.localScale.x >= 0.24 * 4)
            {
                return;
            }
            if (!mergeable || itemData == null)
            {
                return;
            }
            if (collision.gameObject.CompareTag("Item"))
            {
                var item = collision.gameObject.GetComponent<ItemController>();
                var outputItemId = GetOutputItemId(item.itemData);
                var outputItemData = GameManager.Instance.GetItemDataByItemID(outputItemId);
                if (outputItemData == null)
                {
                    return;
                }
                var outputItemDataCopy = new ItemData();
                outputItemDataCopy.Copy(outputItemData);
                item.Dissolve();
                outputItemDataCopy.cost =
                    (item.itemData.cost + itemData.cost) * outputItemData.cost;
                SetItemData(outputItemDataCopy);
                transform.DOScale(transform.localScale * 1.5f, 0.1f).SetLoops(2, LoopType.Yoyo);
            }
        }

        public int GetOutputItemId(ItemData checkItemData)
        {
            if (checkItemData == null)
            {
                return -1;
            }
            foreach (var mergeableItem in this.itemData.mergeableItems)
            {
                if (mergeableItem.itemId == checkItemData.itemId)
                {
                    return mergeableItem.outputItemId;
                }
            }
            return -1;
        }
    }
}
