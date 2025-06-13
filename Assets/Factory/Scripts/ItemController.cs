using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Tween moveTween;

        public AnimationCurve leafDropCurve;

        public bool isInWater = false;

        private void KillAllTweens()
        {
            // Kill any DOTween animations on the item icon
            _itemIcon.DOKill();

            // Kill any material tweens
            var material = GetComponent<SpriteRenderer>().material;
            material.DOKill();

            // Kill any transform tweens
            transform.DOKill();

            // Kill any move tween
            moveTween.Kill();
        }

        void OnEnable()
        {
            // Reset all states
            isCollected = false;
            mergeable = false;
            isInLiquid = false;
            isInWater = false;

            // Kill all active tweens
            KillAllTweens();

            // Cancel all pending invokes
            CancelInvoke(nameof(SetMergeable));
            CancelInvoke(nameof(SetGravityInAir));
            CancelInvoke(nameof(SetGravityInLiquid));
            CancelInvoke(nameof(CollectItem));

            // Reset constraints and visibility
            UnfreezeConstrain();
            _itemIcon.color = new Color(1, 1, 1, 1);
            _itemIcon.gameObject.SetActive(true);
            SetGravityInAir();
        }

        void OnDisable()
        {
            KillAllTweens();
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
            GetComponent<Rigidbody2D>().gravityScale = 1f;
        }

        private async Task SetGravityInLiquid()
        {
            await Task.Delay(2000);
            GetComponent<Rigidbody2D>().gravityScale = 0f; // Reduced gravity for slower fall
            GetComponent<Collider2D>().isTrigger = true;

            // Kill any existing move tween
            moveTween?.Kill();
            transform
                .DORotate(new Vector3(0, 0, 90), 4f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
            await transform
                .DOLocalMoveY(-1.5f, 2f)
                .OnComplete(() =>
                {
                    isInWater = true;
                }).AsyncWaitForCompletion();
            if (itemData.dropType == DropType.Leaf)
            {
                await SetLeafDrop();
            }
            else if (itemData.dropType == DropType.Standard)
            {
                SetStandardDrop();
            }
        }

        public async Task SetStandardDrop()
        {
            moveTween = null;
            transform
                .DOLocalMoveY(-7.5f, 24f / itemData.dropSpeed)
                .OnComplete(async () =>
                {
                    await Task.Delay(1000);
                    await CollectItem();
                });
        }

        public async Task SetLeafDrop()
        {
            bool moveLeft = Random.Range(0, 2) == 0;

            // Create the zigzag sequence
            moveTween = transform
                .DOLocalMoveY(-7.5f, 15f / itemData.dropSpeed)
                .OnComplete(async () =>
                {
                    await Task.Delay(1000);
                    await CollectItem();
                });
            for (int i = 0; i < 5; i++)
            {
                if (transform.localPosition.y <= -7)
                {
                    moveTween.Kill();
                    await Task.Delay(1000);
                    await CollectItem();
                    return;
                }
                await transform
                    .DOLocalMoveX(transform.localPosition.x + (moveLeft ? -0.5f : 0.5f), 1.5f)
                    .SetLoops(2, LoopType.Yoyo)
                    .AsyncWaitForCompletion();
                moveLeft = !moveLeft;
            }
        }

        public async Task CollectItem()
        {
            if (isCollected)
            {
                return;
            }
            await Task.Delay(2000);
            if (isCollected)
            {
                return;
            }
            isCollected = true;
            await _itemIcon.DOFade(0, 1f).AsyncWaitForCompletion();
            GameManager.Instance.CollectItem(this);
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
            if (transform.position.y <= -5 && GetComponent<Collider2D>().isTrigger)
            {
                GetComponent<Rigidbody2D>().gravityScale = 0f;
                GetComponent<Collider2D>().isTrigger = false;
                CollectItem();
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
                        SetGravityInLiquid();
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
