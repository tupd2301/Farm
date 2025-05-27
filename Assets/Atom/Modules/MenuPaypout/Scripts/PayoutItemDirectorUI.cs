using UnityEngine;

namespace Atom
{
    public class PayoutItemDirectorUI : MonoBehaviour
    {
        public event System.Action OnFlyCompleted;
        public event System.Action<int> OnFlyCompletedInEachCoinItem;

        [SerializeField]
        private PayoutItemSFX _payoutItemSFX;
        [SerializeField]
        private PayoutItemFlyUI _payoutItemFly;
        [SerializeField]
        private FloatingTextUI _optionalFloatingTextUI;

        public Vector2 Target
        {
            get => _payoutItemFly.TargetObject.position;
            set
            {
                float z = _payoutItemFly.TargetObject.anchoredPosition3D.z;
                _payoutItemFly.TargetObject.position = value;

                Vector3 anchoredPosition3D = _payoutItemFly.TargetObject.anchoredPosition3D;
                anchoredPosition3D.z = z;
                _payoutItemFly.TargetObject.anchoredPosition3D = anchoredPosition3D;
            }
        }

        private void Awake()
        {
            if (_optionalFloatingTextUI != null)
            {
                _payoutItemFly.OnFlyStarted += onFlyStarted;
            }
            _payoutItemFly.OnFlyCompletedInEachItem += onFlyOnEachItemCompleted;
            _payoutItemFly.OnFlyCompleted += onFlyObjectCompleted;
        }

        private void OnDestroy()
        {
            if (_optionalFloatingTextUI != null)
            {
                _payoutItemFly.OnFlyStarted -= onFlyStarted;
            }
            _payoutItemFly.OnFlyCompletedInEachItem -= onFlyOnEachItemCompleted;
            _payoutItemFly.OnFlyCompleted -= onFlyObjectCompleted;
        }

        public void Setup(RectTransform targetObject)
        {
            _payoutItemFly.TargetObject = targetObject;
        }

        public void StartPayoutAnimation(int payoutValue, Vector3? originPos = null)
        {
            if (originPos != null)
            {
                _payoutItemFly.transform.position = (Vector3)originPos;
            }

            _payoutItemFly.gameObject.SetActive(true);
            int payoutMaxValue = 7;
            if (payoutValue < payoutMaxValue)
            {
                _payoutItemFly.CreateFlyObjects(payoutValue, payoutValue);
            }
            _payoutItemFly.CreateFlyObjects(payoutMaxValue, payoutValue);

            if (_optionalFloatingTextUI != null)
            {
                _optionalFloatingTextUI.gameObject.SetActive(true);
                _optionalFloatingTextUI.Value = $"+{payoutValue}";
            }
        }

        private void onFlyStarted()
        {
            _optionalFloatingTextUI.DoFxFloating();
        }

        private void onFlyObjectCompleted()
        {
            if (_payoutItemSFX != null)
            {
                _payoutItemSFX.PlayFlyCompleteSound();
            }
            OnFlyCompleted?.Invoke();
        }

        private void onFlyOnEachItemCompleted(int value)
        {
            if (_payoutItemSFX != null)
            {
                _payoutItemSFX.PlayFlyStartSound();
            }
            OnFlyCompletedInEachCoinItem?.Invoke(value);
        }
    }
}