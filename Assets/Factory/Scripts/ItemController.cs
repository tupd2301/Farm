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

        public bool isInLiquid = false;

        private void KillAllTweens()
        {
            // Kill any DOTween animations on the item icon
            _itemIcon.DOKill();

            // Kill any material tweens
            var material = GetComponent<SpriteRenderer>().material;
            material.DOKill();

            // Kill any transform tweens
            transform.DOKill();
        }

        void OnEnable()
        {
            // Reset all states
            isCollected = false;
            mergeable = false;
            isInLiquid = false;

            // Kill all active tweens
            KillAllTweens();

            // Cancel all pending invokes
            CancelInvoke(nameof(SetMergeable));
            CancelInvoke(nameof(SetGravityInAir));
            CancelInvoke(nameof(SetGravityInLiquid));

            // Reset constraints and visibility
            UnfreezeConstrain();
            _itemIcon.color = new Color(1, 1, 1, 1);
            _itemIcon.gameObject.SetActive(true);

            // Set up initial state
            Invoke(nameof(SetMergeable), 1f);
            SetGravityInAir();
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

        private void SetGravityInAir()
        {
            isInLiquid = false;
            GetComponent<Collider2D>().isTrigger = false;
            GetComponent<Rigidbody2D>().gravityScale = 0.5f;
        }

        private void SetGravityInLiquid()
        {
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<Rigidbody2D>().gravityScale = 0.1f;
            _itemIcon
                .DOColor(new Color(0, 0.5f, 1, 1), 3f)
                .SetEase(Ease.InExpo)
                .OnComplete(() =>
                {
                    Dissolve();
                });
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
            var material = GetComponent<SpriteRenderer>().material;
            material.SetFloat("_Dissolve", 1f);
            material
                .DOFloat(0f, "_Dissolve", 1f)
                .SetEase(Ease.InExpo)
                .OnComplete(() =>
                {
                    _itemIcon.DOKill();
                    _itemIcon
                        .DOColor(new Color(0, 0.5f, 1, 0), 1f)
                        .SetEase(Ease.InExpo)
                        .OnComplete(() =>
                        {
                            GameManager.Instance.CollectItem(this);
                        });
                });
        }

        void FixedUpdate()
        {
            if (!isInLiquid)
            {
                UsingRaycast();
            }
        }

        void FreezeConstrain()
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        }

        void UnfreezeConstrain()
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }

        void UsingRaycast()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(
                transform.position,
                Vector2.down,
                itemData.size * 0.6f
            );
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.CompareTag("Liquid") && !isInLiquid)
                    {
                        isInLiquid = true;
                        Invoke(nameof(SetGravityInLiquid), 2f);
                        FreezeConstrain();
                    }
                    if (hit.collider.CompareTag("Item") && !isCollected && mergeable)
                    {
                        var item = hit.collider.GetComponent<ItemController>();
                        var outputItemId = GetOutputItemId(item.itemData);
                        var outputItemData = GameManager.Instance.GetItemDataByItemID(outputItemId);
                        if (outputItemData == null)
                        {
                            return;
                        }
                        var outputItemDataCopy = new ItemData();
                        outputItemDataCopy.Copy(outputItemData);
                        // item.Dissolve();
                        item.isCollected = true;
                        GameManager.Instance.CollectItem(item);
                        outputItemDataCopy.cost =
                            (item.itemData.cost + itemData.cost) * outputItemData.cost;
                        SetItemData(outputItemDataCopy);
                        transform
                            .DOScale(transform.localScale * 1.5f, 0.1f)
                            .SetLoops(2, LoopType.Yoyo);
                        return;
                    }
                }
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            return;
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
            if (collision.gameObject.CompareTag("Liquid") && !isInLiquid)
            {
                isInLiquid = true;
                Invoke(nameof(SetGravityInLiquid), 2f);
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
