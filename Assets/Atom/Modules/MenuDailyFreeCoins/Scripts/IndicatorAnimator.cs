using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class IndicatorAnimator : MonoBehaviour
{
    public event System.Action<float> OnIndicatorStartPatrol;
    public event System.Action<Vector2> OnIndicatorPatrolling;

    public AnimationCurve AnimationCurve;

    private RectTransform _rectTransform;
    private Sequence _sequence;
    private Sequence _highLightSequence;
    private float _startY;

    private Vector2 _lastPosition;
    private Vector2 _moveDirection;

    [SerializeField]
    private RectTransform _mainRectTransform;
    [SerializeField]
    private Image _highLightImage;
    [SerializeField]
    private float _duration = 0.8f;
    [SerializeField]
    private float _height = 20f;
    [Range(0, 1)]
    [SerializeField]
    private float _maxHighLightValue = 0.5f;

    public RectTransform RecTransform
    {
        get => _rectTransform;
    }

    public Vector2 SizeDelta
    {
        get
        {
            if(_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform.sizeDelta;
        }
        set => _rectTransform.sizeDelta = value;
    }

    public Vector2 AnchoredPosition
    {
        get => _rectTransform.anchoredPosition;
        set => _rectTransform.anchoredPosition = value;
    }

    private void Awake()
    {
        _startY = _mainRectTransform.anchoredPosition.y;
    }

    private void OnDestroy()
    {
        killSequence(_sequence);
        killSequence(_highLightSequence);
    }

    public void DoIdleAnimation()
    {
        killSequence(_sequence);
        float highY = _startY + _height;
        resetPositionY();
        startIdleAnimation(highY);
    }

    public void DoPatrolAnimation(Vector2 minPosition, Vector2 maxPosition, float duration)
    {
        _sequence = DOTween.Sequence();

        minPosition.y += _startY;
        if(_lastPosition == Vector2.zero)
        {
            _rectTransform.anchoredPosition = minPosition;
        }
        float patrolDuration = startPartrolAnimation(minPosition, maxPosition, duration);
        OnIndicatorStartPatrol?.Invoke(patrolDuration);
    }

    public void StopPatrol()
    {
        if(_sequence != null && _sequence.IsPlaying())
        {
            _sequence.Pause();
        }
    }

    public void ContinuePatrol()
    {
        _sequence.Play();
    }

    public void SetIndicatorPositionX(float positionX)
    {
        Vector2 position = new Vector2(positionX, _rectTransform.anchoredPosition.y);
        _rectTransform.anchoredPosition = position;
    }

    public void DoGlowAnimation()
    {
        killSequence(_highLightSequence);
        _highLightImage.color = new Color(1, 1, 1, 0);
        startHightLightAnimation();
    }

    private void startHightLightAnimation()
    {
        _highLightSequence = DOTween.Sequence();
        _highLightSequence.Append(_highLightImage.DOFade(_maxHighLightValue, _duration)).
            Append(_highLightImage.DOFade(0, _duration).SetEase(Ease.Linear));
        _highLightSequence.SetLoops(-1);
    }

    private void startIdleAnimation(float highY)
    {
        _sequence = DOTween.Sequence();
        _sequence.Append(_mainRectTransform.DOAnchorPosY(highY, _duration)).
            Append(_mainRectTransform.DOAnchorPosY(_startY, _duration).SetEase(Ease.Linear));

        _sequence.SetLoops(-1);
    }

    private float startPartrolAnimation(Vector2 minPosition, Vector2 maxPosition, float duration)
    {
        float patrolDuration = duration / 2;
        _sequence.Append(_rectTransform.DOAnchorPos(maxPosition, patrolDuration).SetEase(AnimationCurve));
        _sequence.Append(_rectTransform.DOAnchorPos(minPosition, patrolDuration).SetEase(AnimationCurve));

        _lastPosition = _rectTransform.anchoredPosition;
        _sequence.SetLoops(-1);
        _sequence.OnUpdate(onPatrolling);
        return patrolDuration;
    }

    private void onPatrolling()
    {
        OnIndicatorPatrolling?.Invoke(_rectTransform.anchoredPosition);
        updatePatrolDirection();
    }

    private void updatePatrolDirection()
    {
        _moveDirection = _rectTransform.anchoredPosition - _lastPosition;
        _lastPosition = _rectTransform.anchoredPosition;
    }

    private void killSequence(Sequence sequence)
    {
        if (sequence != null && sequence.IsPlaying())
        {
            sequence.Kill();
        }
    }

    private void resetPositionY()
    {
        Vector2 position = _mainRectTransform.anchoredPosition;
        _mainRectTransform.anchoredPosition = new Vector2(position.x, _startY);
    }
}
