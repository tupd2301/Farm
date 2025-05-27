using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class MultiplierIndicator : MonoBehaviour
{
    public event System.Action OnPatrollingAtMultiplier;

    public int PatrolValue;

    [SerializeField]
    private List<int> _multipliers;
    private Sequence _sequence;

    [SerializeField]
    private IndicatorAnimator _indicator;
    [SerializeField]
    private IndicatorLayout _indicatorLayout;
    [SerializeField]
    private float _patrolDuration = 1f;

    private void OnDestroy()
    {
        StopSequence();

        _indicator.OnIndicatorPatrolling -= updateOnPatrol;
    }

    public void Setup(List<int> multipliers)
    {
        if (multipliers == null)
        {
            Debug.LogError($"{nameof(multipliers)} is Null!!");
            return;
        }
        _multipliers = multipliers;

        _indicatorLayout.Setup(multipliers);

        _indicator.OnIndicatorPatrolling += updateOnPatrol;

        float indicatorHeightDistance = getIndicatorHeight();
        doPatrolAnimation(indicatorHeightDistance);
    }

    public void ContinuePatrol()
    {
        _indicator.ContinuePatrol();
    }

    public void StopSequence()
    {
        if (_sequence != null && _sequence.IsPlaying())
        {
            _sequence.Kill();
        }
    }

    public void Stop()
    {
        _indicator.StopPatrol();
        //setIndicatorAtMiddlePosition();
    }

    private void updateOnPatrol(Vector2 position)
    {
        IndicatorInfo indicatorInfo = _indicatorLayout.GetIndicatorInfoByPosition(position);
        if(indicatorInfo != null)
        {
            PatrolValue = indicatorInfo.Value;
            OnPatrollingAtMultiplier?.Invoke();
        }
    }

    private void doPatrolAnimation(float indicatorHeightDistance)
    {
        Vector3 minPosition = getMinIndicatorMinPosition(indicatorHeightDistance);
        minPosition.x -= indicatorHeightDistance / 2;
        Vector3 maxPosition = getMaxIndicatorMaxPosition(indicatorHeightDistance);
        maxPosition.x += indicatorHeightDistance / 2;

        _indicator.DoPatrolAnimation(minPosition, maxPosition, _patrolDuration);
    }

    private void setIndicatorAtMiddlePosition()
    {
        IndicatorInfo info = _indicatorLayout.GetIndicatorInfoByPosition(_indicator.AnchoredPosition);
        _indicator.SetIndicatorPositionX(info.AnchoredPositon.x);
    }

    private Vector3 getMaxIndicatorMaxPosition(float indicatorHeightDistance)
    {
        Vector3 maxPosition = _indicatorLayout.GetMultiplierPositionByIndex(_multipliers.Count - 1);
        maxPosition.y += indicatorHeightDistance;
        return maxPosition;
    }

    private Vector3 getMinIndicatorMinPosition(float indicatorHeightDistance)
    {
        Vector3 minPosition = _indicatorLayout.GetMultiplierPositionByIndex(0);
        minPosition.y += indicatorHeightDistance;
        return minPosition;
    }

    private float getIndicatorHeight()
    {
        Vector2 sizeDelta = _indicatorLayout.GetMultiplierSizeDeltaByIndex(0);
        float indicatorHaftHeight = _indicator.SizeDelta.y / 2f;
        float haftHeight = sizeDelta.y / 2;
        return indicatorHaftHeight + haftHeight;
    }
}
