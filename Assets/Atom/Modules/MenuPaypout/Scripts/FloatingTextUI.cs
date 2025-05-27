using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Atom
{
    [RequireComponent(typeof(RectTransform))]
    public class FloatingTextUI : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Sequence _sequence;
        private Vector2 _startPosition;

        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Ease _fadeInEase = Ease.Linear;
        [SerializeField]
        private float _fadeInDuration = 1f;

        public string Value
        {
            get => _text.text;
            set => _text.text = value;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startPosition = _rectTransform.anchoredPosition;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            killSequence();
        }

        public void DoFxFloating()
        {
            killSequence();

            gameObject.SetActive(true);
            float y = resetFloatingTextAtStart();
            startFloatingAnimation(y);
        }

        private float resetFloatingTextAtStart()
        {
            Color color = _text.color;
            color.a = 0;
            _text.color = color;
            _rectTransform.anchoredPosition = _startPosition;
            return _rectTransform.position.y;
        }

        private void startFloatingAnimation(float y)
        {
            _sequence = DOTween.Sequence();
            _rectTransform.localScale = Vector2.zero;

            _text.DOFade(1f, _fadeInDuration / 4);
            _sequence.Append(_rectTransform.DOScale(1f, _fadeInDuration / 2).SetEase(_fadeInEase));
            _sequence.Append(_rectTransform.DOMoveY(y + 5, 4f));
            _sequence.Join(_text.DOFade(0, _fadeInDuration / 2).SetDelay(_fadeInDuration / 2).
                OnComplete(() => gameObject.SetActive(false)));
        }

        private void killSequence()
        {
            if (_sequence != null && _sequence.IsPlaying())
            {
                _sequence.Kill();
            }
        }
    }
}