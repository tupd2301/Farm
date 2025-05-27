using UnityEngine;
using DG.Tweening;

namespace Atom
{
    public class PayoutItemFlyUI : MonoBehaviour
    {
        private const float FLY_TIME = 0.15f;
        private const float SCALE_TIME = 0.05f;
        private const float FLY_ITEM_APPEAR_TIME = 0.8f;
        private const float DELAY_TIME = 0.15f;
        private const float PARTICLE_SCALE_TIME = 0.1f;

        public event System.Action OnFlyCompleted;
        public event System.Action OnFlyStarted;
        public event System.Action<int> OnFlyCompletedInEachItem;

        public RectTransform TargetObject;
        public RectTransform ContainerRect;

        private Sequence _sequence;
        private Sequence _moveSequence;
        private Sequence _targetScaleSequence;
        private float _scale;
        private int _lastValue;

        [SerializeField]
        private RectTransform _flyPrefab;

        public void CreateFlyObjects(int count, int value)
        {
            ContainerRect.localScale = Vector2.zero;
            startExplodeAnimation(count);
            int divided = value / count;

            int remain = value - (divided * count);
            _lastValue = divided + remain;
            setupFlyFx(divided);
        }

        private void Awake()
        {
            _scale = TargetObject.localScale.x;
        }

        private void startExplodeAnimation(int count)
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(ContainerRect.DOScale(1f, FLY_ITEM_APPEAR_TIME).
                SetEase(Ease.OutQuint)).
                OnStart(() => OnFlyStarted?.Invoke());
            spawnFlyObjectAtRandomPosition(count);
        }

        private void setupFlyFx(int divided)
        {
            _moveSequence = DOTween.Sequence();
            startFlyForAllFlyObject(divided);
            _sequence.Append(_moveSequence).SetDelay(DELAY_TIME);
        }

        private void startFlyForAllFlyObject(int divided)
        {
            for (int i = 0; i < ContainerRect.childCount; i++)
            {
                Transform child = ContainerRect.GetChild(i);
                System.Action onFlyObjectReachTarget = setupOnFlyObjectCallBack(divided, i, child);
                startMoveToTarget(child, onFlyObjectReachTarget);
            }
        }

        private System.Action setupOnFlyObjectCallBack(int divided, int i, Transform child)
        {
            System.Action onFlyObjectReachTarget = () =>
            {
                doTargetFx();
                setupFlyObject(divided, child);
            };
            if (i == ContainerRect.childCount - 1)
            {
                onFlyObjectReachTarget = () =>
                {
                    doTargetFx(onAllFlyObjectReachTarget);
                    setupFlyObject(_lastValue, child);
                };
            }

            return onFlyObjectReachTarget;
        }

        private void startMoveToTarget(Transform child, System.Action onObjectReachTarget)
        {
            _moveSequence.Join(child.DOMove(TargetObject.transform.position, FLY_TIME).
                            SetDelay(FLY_TIME).
                            OnComplete(() =>
                            {
                                onObjectReachTarget?.Invoke();

                            }));
        }

        private void onAllFlyObjectReachTarget()
        {
            OnFlyCompleted?.Invoke();
        }

        private void spawnFlyObjectAtRandomPosition(int count)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float bonusAngle = Mathf.PI * 2 / count;
            for (int i = 0; i < count; i++)
            {
                RectTransform particleObject = Instantiate(_flyPrefab, ContainerRect);
                particleObject.anchoredPosition = getRandomPositionInContainer(angle);
                particleObject.sizeDelta = Vector2.zero;
                particleObject.DOScale(getRandomFlyObjectSize(), PARTICLE_SCALE_TIME);
                particleObject.gameObject.SetActive(true);
                angle += bonusAngle;
            }
        }

        private void setupFlyObject(int divided, Transform child)
        {
            child.gameObject.SetActive(false);
            TargetObject.gameObject.SetActive(true);
            OnFlyCompletedInEachItem?.Invoke(divided);
        }

        private Vector2 getRandomPositionInContainer(float angle)
        {
            float radius = ContainerRect.rect.size.x / 4;
            float randomRadius = Random.Range(ContainerRect.rect.size.x / 6, radius);

            float x = Mathf.Sin(angle) * randomRadius;
            float y = Mathf.Cos(angle) * randomRadius;

            return new Vector2(x, y);
        }

        private Vector2 getRandomFlyObjectSize()
        {
            float minX = _flyPrefab.localScale.x;

            float maxX = _flyPrefab.localScale.x * 1.2f;

            float y = _flyPrefab.localScale.x - _flyPrefab.localScale.y;
            y = Mathf.Abs(y);

            float randomX = Random.Range(minX, maxX);
            return new Vector2(randomX, randomX + y);
        }

        private void doTargetFx(System.Action onFxCompleted = null)
        {
            killSequence(_targetScaleSequence);
            startTargetScaleAnimation();
            if (onFxCompleted != null)
            {
                _targetScaleSequence.OnComplete(onFxCompleted.Invoke);
            }
        }

        private void startTargetScaleAnimation()
        {
            _targetScaleSequence = DOTween.Sequence();
            TargetObject.localScale = new Vector2(_scale, _scale);
            _targetScaleSequence.Append(TargetObject.DOScale(_scale + 0.2f, SCALE_TIME));
            _targetScaleSequence.Append(TargetObject.DOScale(_scale, SCALE_TIME));
        }

        private void killSequence(Sequence sequence)
        {
            if (sequence == null)
            {
                return;
            }
            if (sequence.IsActive() && sequence.IsPlaying())
            {
                sequence.Kill();
            }
        }
    }
}