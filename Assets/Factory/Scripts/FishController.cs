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
        private FishTweenParams tweenParams;

        public Canvas _canvas;
        public Transform _fishBody;
        public SpriteRenderer _spriteRenderer;
        public List<Sprite> _sprites;
        public FishState state = FishState.Idle;
        public FishConfig fishConfig;
        public float currentTotalTickValue = 0;

        public RectTransform _confuseVFX;

        public Slider _slider;

        public void Awake()
        {
            _canvas = GetComponentInChildren<Canvas>();
            _canvas.worldCamera = GameManager.Instance.mainCamera;
            _slider = _canvas.GetComponentInChildren<Slider>();
            _slider.value = 0;
        }

        public void SetSprite(int index)
        {
            _spriteRenderer.sprite = _sprites[index];
        }

        public void Init(FishConfig fishConfig)
        {
            this.fishConfig = fishConfig;
            currentTotalTickValue = 0;
            _slider.maxValue = fishConfig.fishCurrencyValue;
            _slider.value = 0;
            state = FishState.Moving;
            SetSprite(Random.Range(0, _sprites.Count));
            MoveRandom();
        }

        public void MoveTo(Vector3 position)
        {
            transform
                .DOMove(position, tweenParams.moveToPositionDuration)
                .SetEase(tweenParams.moveToPositionEase);
        }

        public void FindTarget()
        {
            if (state != FishState.Moving)
            {
                return;
            }
            //using raycast
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 2f, Vector2.zero);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Item"))
                    {
                        ItemController item = hit.collider.GetComponent<ItemController>();
                        if (item.isInLiquid && !item.isCollected)
                        {
                            item.isCollected = true;
                            item.transform.DOScale(0.5f, tweenParams.itemCollectScaleDuration)
                                .SetLoops(2, LoopType.Yoyo);
                            item.transform.DOMove(
                                    transform.position,
                                    tweenParams.itemMoveToFishDuration
                                )
                                .SetEase(tweenParams.itemMoveToFishEase)
                                .OnComplete(() =>
                                {
                                    if (
                                        item.gameObject.activeSelf == false
                                        || string.IsNullOrEmpty(item.itemData.itemName)
                                    )
                                    {
                                        return;
                                    }
                                    _fishBody
                                        .DOScale(
                                            _fishBody.localScale
                                                * tweenParams.fishGrowScaleMultiplier,
                                            tweenParams.fishGrowScaleDuration
                                        )
                                        .SetLoops(2, LoopType.Yoyo);
                                    item.isCollected = true;
                                    GameManager.Instance.CollectItem(item);
                                    currentTotalTickValue += item.itemData.cost;
                                    _slider.value = currentTotalTickValue;
                                });
                            break;
                        }
                    }
                }
            }
            if (currentTotalTickValue >= fishConfig.fishCurrencyValue)
            {
                state = FishState.Dead;
                //using sequence
                Sequence sequence = DOTween.Sequence();
                var happyVFX = PoolSystem.Instance.GetObject("HappyVFX");
                happyVFX.transform.localPosition = transform.position;
                happyVFX.SetActive(true);
                PoolSystem.Instance.ReturnObject(happyVFX, "HappyVFX", 1f);

                var coin = PoolSystem.Instance.GetObject("Coin");
                coin.transform.position = transform.position;
                coin.SetActive(true);
                coin.GetComponent<CoinController>().value = fishConfig.dropCoinValue;
                PoolSystem.Instance.ReturnObject(coin, "Coin", 10f);

                sequence.Append(
                    _fishBody
                        .DOLocalRotate(
                            new Vector3(0, 0, tweenParams.fishDeathRotateAngle),
                            tweenParams.fishDeathRotateDuration,
                            RotateMode.FastBeyond360
                        )
                        .SetEase(tweenParams.fishDeathRotateEase)
                        .SetLoops(tweenParams.fishDeathRotateLoops, LoopType.Yoyo)
                        .OnComplete(() =>
                        {
                            FlipWithDirection(new Vector3(-1, 1, 1));
                        })
                );
                sequence
                    .Append(
                        transform
                            .DOLocalMoveX(
                                tweenParams.fishDeathMoveXPosition,
                                tweenParams.fishDeathMoveXDuration
                            )
                            .SetEase(tweenParams.fishDeathMoveEase)
                    )
                    .OnStart(() =>
                    {
                        state = FishState.Dead;
                        FlipWithDirection(new Vector3(-1, 1, 1));
                    });
                sequence.OnComplete(() =>
                {
                    _fishBody
                        .DOScale(0, 0f)
                        .SetEase(tweenParams.fishDeathMoveEase)
                        .OnComplete(() =>
                        {
                            transform.localPosition = new Vector3(
                                tweenParams.fishDeathMoveXPosition,
                                0,
                                0
                            );
                            FishManager.Instance.CheckWinLose();
                        });
                });
                sequence.Play();
                return;
            }
        }

        void Update()
        {
            if (state == FishState.Moving)
            {
                FindTarget();
            }
            if (
                state == FishState.Dead
                && _fishBody.localScale.x == 0
                && transform.localPosition != new Vector3(tweenParams.fishDeathMoveXPosition, 0, 0)
            )
            {
                transform.localPosition = new Vector3(tweenParams.fishDeathMoveXPosition, 0, 0);
            }
        }

        public void OnClick()
        {
            if (state == FishState.Moving)
            {
                MoveRandom();
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

        public void MoveRandom()
        {
            if (state == FishState.Dead)
            {
                return;
            }
            // Kill any existing movement tween before starting a new one
            transform.DOKill(false);

            System.Random random = new System.Random();
            int randomX = random.Next(-4, 4);
            int randomY = random.Next(-2, 2);
            float distance = Vector3.Distance(transform.position, new Vector3(randomX, randomY, 0));
            float time = distance / 2;
            Vector3 randomPosition = new Vector3(randomX, randomY, 0);
            Vector3 direction = randomPosition - transform.position;
            Vector3 outputDirection = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
            FlipWithDirection(outputDirection);
            transform
                .DOLocalMove(randomPosition, time)
                .SetEase(tweenParams.moveToPositionEase)
                .OnComplete(() =>
                {
                    if (state == FishState.Moving)
                    {
                        MoveRandom();
                    }
                });
        }

        public void FlipWithDirection(Vector3 direction)
        {
            //using transform
            _fishBody.localScale = direction;
        }
    }

    public enum FishState
    {
        Idle,
        Moving,
        Attacking,
        Dead,
    }
}
