using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ChildHorizontalExtender : MonoBehaviour
{
    private RectTransform _rectTransform;

    public Vector2 GetChildScale(int estimateChildCount)
    {
        _rectTransform = GetComponent<RectTransform>();

        return CalculateChildScale(estimateChildCount);
    }

    private Vector2 CalculateChildScale(int estimateChildCount)
    {
        Vector2 sizeDelta = _rectTransform.sizeDelta;
        float childWidth = sizeDelta.x / estimateChildCount;
        float childHeight = sizeDelta.y / estimateChildCount;
        return new Vector2(childWidth, childHeight);
    }
}
