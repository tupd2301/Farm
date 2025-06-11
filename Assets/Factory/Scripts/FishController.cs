using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Factory
{
    [System.Serializable]
    public class FishTweenParams
    {
        [Header("Movement")]
        public float moveToPositionDuration = 0.2f;
        public Ease moveToPositionEase = Ease.InOutSine;

        [Header("Item Collection")]
        public float itemCollectScaleDuration = 0.2f;
        public float itemMoveToFishDuration = 0.2f;
        public Ease itemMoveToFishEase = Ease.InOutSine;

        [Header("Fish Growth")]
        public float fishGrowScaleDuration = 0.1f;
        public float fishGrowScaleMultiplier = 1.1f;

        [Header("Death Animation")]
        public float fishDeathRotateDuration = 0.1f;
        public float fishDeathMoveXDuration = 2.5f;
        public float fishDeathRotateAngle = 30f;
        public int fishDeathRotateLoops = 6;
        public float fishDeathMoveXPosition = -15f;
        public Ease fishDeathRotateEase = Ease.InSine;
        public Ease fishDeathMoveEase = Ease.InSine;
    }

    public class FishController : MonoBehaviour
    {
        [SerializeField]
        protected FishTweenParams tweenParams;

        public Canvas _canvas;
        public Transform _fishBody;

        [SerializeField]
        protected SpriteRenderer _hpBarMask;

        [SerializeField]
        protected SpriteRenderer _spriteRenderer;

        public SpriteMask _spriteMask;
        public FishState state = FishState.Idle;
        public FishConfig fishConfig;
        public float currentTotalTickValue = 0;

        public RectTransform _confuseVFX;

        public Transform hpBar;

        public Vector3 targetPosition;

        public Tween _moveTween;

        protected int _indexSprite;

        protected ItemController _currentTargetItem;

        protected bool _lockTarget = false;

        public void Awake()
        {
            _canvas = GetComponentInChildren<Canvas>();
            _currentTargetItem = null;
            if (_canvas != null)
            {
                _canvas.worldCamera = GameManager.Instance.mainCamera;
            }
        }

        public void KillAllTween()
        {
            _moveTween.Kill();
        }

        public void SetSprite(int index)
        {
            _indexSprite = index;
            _spriteRenderer.sprite = fishConfig.sprites[index].sprite;
        }

        public void Init(FishConfig fishConfig, int index)
        {
            _hpBarMask.sortingOrder = 100 + index;
            hpBar.GetComponent<SpriteRenderer>().sortingOrder = 100 + index + 1;
            _spriteMask.backSortingOrder = 100 + index;
            _spriteMask.frontSortingOrder = 100 + index + 1;
            this.fishConfig = fishConfig;
            currentTotalTickValue = 0;
            state = FishState.Moving;
            SetSprite(0);
            targetPosition = transform.position;
            Move();
            currentTotalTickValue = fishConfig.fishCurrencyValue * 0.5f;
            _spriteRenderer.material.SetFloat("_SwaySpeed", 1);
            _spriteRenderer.material.SetColor("_Color", new Color32(255, 255, 255, 255));
            _currentTargetItem = null;
            _hpBarMask.gameObject.SetActive(true);

            UpdateHpBar();
            if (fishConfig.isBoss)
            {
                return;
            }
            Invoke(nameof(DecreaseHPByTime), 1f);
        }

        public void UpdateHpBar()
        {
            float percent01 = currentTotalTickValue / fishConfig.fishCurrencyValue;
            hpBar.localScale = new Vector3(percent01, 1, 1);
            if (percent01 > 0.3f)
            {
                hpBar.GetComponent<SpriteRenderer>().color = new Color32(160, 200, 0, 255);
                SetSprite(0);
            }
            else
            {
                hpBar.GetComponent<SpriteRenderer>().color = new Color32(210, 50, 20, 255);
                // SetSprite(1);
            }
        }

        public virtual void DecreaseHPByTime()
        {
            currentTotalTickValue -=
                fishConfig.fishCurrencyValue * fishConfig.percentDecrease / 100;
            UpdateHpBar();
            if (currentTotalTickValue < 0)
            {
                state = FishState.Dead;
                _moveTween.Kill();
                FlipWithDirection(new Vector3(-1, -1, 1));
                _hpBarMask.gameObject.SetActive(false);
                _spriteRenderer.material.DOFade(0, 3f);
                transform
                    .DOLocalMoveY(5, 3f)
                    .OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                _spriteRenderer.material.SetFloat("_SwaySpeed", 0);
                FishManager.Instance.CheckWinLose();
            }
            if (state == FishState.Dead)
            {
                return;
            }
            Invoke(nameof(DecreaseHPByTime), 1f);
        }

        public virtual async Task FindTarget()
        {
            if (state != FishState.Moving || fishConfig.isBoss)
            {
                return;
            }
            // Use raycast to find items in range
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 2.5f, Vector2.zero);
            // Find the closest collectible item
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Item"))
                {
                    ItemController item = hit.collider.GetComponent<ItemController>();
                    if (item.transform.position.y > -1f)
                    {
                        continue;
                    }
                    if (
                        item != null
                        && item.isInLiquid
                        && !item.isCollected
                        && item.gameObject.activeSelf
                    )
                    {
                        if (Vector3.Distance(transform.position, item.transform.position) < 1f)
                        {
                            Debug.Log("Collect Item");
                            if (
                                item.gameObject.activeSelf == false
                                || string.IsNullOrEmpty(item.itemData.itemName)
                            )
                            {
                                return;
                            }
                            _fishBody
                                .DOScale(
                                    _fishBody.localScale * tweenParams.fishGrowScaleMultiplier,
                                    tweenParams.fishGrowScaleDuration
                                )
                                .SetLoops(2, LoopType.Yoyo);
                            item.isCollected = true;
                            GameManager.Instance.CollectItem(item);
                            currentTotalTickValue += item.itemData.cost;
                            UpdateHpBar();
                            CheckFull();
                            _lockTarget = false;
                        }
                        else
                        {
                            if (_lockTarget)
                            {
                                if (_currentTargetItem != null && _currentTargetItem != item)
                                {
                                    continue;
                                }
                            }
                            _currentTargetItem = item;
                            targetPosition = item.transform.position;
                            _lockTarget = true;
                            Move();
                        }
                        break;
                    }
                    else
                    {
                        _currentTargetItem = null;
                    }
                }
            }
        }

        public virtual void CheckFull()
        {
            if (currentTotalTickValue >= fishConfig.fishCurrencyValue)
            {
                state = FishState.Full;
                _moveTween.Kill();
                FlipWithDirection(new Vector3(-1, 1, 1));
                transform.DOLocalMove(
                    new Vector3(-15, 0, 0),
                    Vector2.Distance(transform.localPosition, new Vector3(-15, 0, 0)) / 2
                );
                _spriteRenderer.material.SetFloat("_SwaySpeed", 3);
                var happyVFX = PoolSystem.Instance.GetObject("HappyVFX");
                happyVFX.transform.localPosition = transform.position;
                happyVFX.SetActive(true);
                PoolSystem.Instance.ReturnObject(happyVFX, "HappyVFX", 1f);

                var coin = PoolSystem.Instance.GetObject("Coin");
                coin.transform.position = transform.position;
                coin.SetActive(true);
                coin.GetComponent<CoinController>().value = fishConfig.dropCoinValue;

                CancelInvoke(nameof(DecreaseHPByTime));
                FishManager.Instance.CheckWinLose();
            }
            else
            {
                targetPosition = transform.position;
                Move();
            }
        }

        public virtual void Update()
        {
            if (state == FishState.Moving)
            {
                FindTarget();
            }
        }

        public virtual void OnClick()
        {
            if (state == FishState.Moving)
            {
                targetPosition = transform.position;
                Move();
                Debug.Log("OnClick");
                Confuse();
            }
        }

        public void Confuse()
        {
            System.Random random = new System.Random();
            int randomInt = random.Next(0, 100);
            if (randomInt < 50)
            {
                _confuseVFX.transform.DOComplete();
                _confuseVFX.transform.localScale = Vector3.zero;
                _confuseVFX.gameObject.SetActive(true);
                _confuseVFX
                    .transform.DOScale(1, 0.5f)
                    .SetEase(Ease.OutBack)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        _confuseVFX.gameObject.SetActive(false);
                    });
            }
        }

        public virtual void Move()
        {
            if (state == FishState.Dead || state == FishState.Full)
            {
                return;
            }
            _moveTween.Kill();
            _moveTween = null;
            if (targetPosition == transform.position)
            {
                System.Random random = new System.Random();
                float randomX = random.Next(-30, 30) * 0.1f;
                float randomY = random.Next(-40, 0) * 0.1f;
                targetPosition = new Vector3(randomX, randomY, 0);
            }
            float distance = Vector3.Distance(transform.position, targetPosition);
            float time = distance * 1f;
            Vector3 direction = targetPosition - transform.position;
            Vector3 outputDirection = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
            FlipWithDirection(outputDirection);
            _moveTween = transform
                .DOMove(targetPosition, time)
                .SetEase(tweenParams.moveToPositionEase)
                .OnComplete(() =>
                {
                    if (state == FishState.Moving)
                    {
                        _lockTarget = false;
                        Move();
                    }
                });
        }

        public void FlipWithDirection(Vector3 direction)
        {
            //using transform
            _fishBody.localScale = direction;
        }
    }

    [System.Serializable]
    public class FishSpriteByHP
    {
        public int percentHP;
        public Sprite sprite;
    }

    public enum FishState
    {
        Idle,
        Moving,
        Attacking,
        Dead,
        Full,
        Confused,
    }
}
