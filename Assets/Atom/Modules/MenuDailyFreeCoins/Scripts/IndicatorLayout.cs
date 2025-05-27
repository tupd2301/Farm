using System.Collections.Generic;
using UnityEngine;

public class IndicatorLayout : MonoBehaviour
{
    private RectTransform _rectTransform;
    private List<IndicatorInfo> _listMultiplierObject = new List<IndicatorInfo>();

    [SerializeField]
    private ChildHorizontalExtender _extender;
    [SerializeField]
    private ListValueSpritePair _listMultiplierDefines;
    [SerializeField]
    private IndicatorInfo _multiplierPrefab;

    public Vector2 GetMultiplierPositionByIndex(int index)
    {
        return _listMultiplierObject[index].RectTransform.anchoredPosition;
    }

    public Vector2 GetMultiplierSizeDeltaByIndex(int index)
    {
        return _listMultiplierObject[index].SizeDelta;
    }

    public IndicatorInfo GetIndicatorInfoByPosition(Vector2 position)
    {
        foreach (IndicatorInfo indicatorInfo in _listMultiplierObject)
        {
            if (isPositionInIndicatorSection(position, indicatorInfo))
            {
                return indicatorInfo;
            }
        }
        return null;
    }

    private bool isPositionInIndicatorSection(Vector2 position, IndicatorInfo indicatorInfo)
    {
        return position.x >= indicatorInfo.RectTransform.anchoredPosition.x - (indicatorInfo.SizeDelta.x / 2) &&
            position.x <= indicatorInfo.RectTransform.anchoredPosition.x + (indicatorInfo.SizeDelta.x / 2);
    }

    public void Setup(List<int> multipliers)
    {
        _rectTransform = GetComponent<RectTransform>();
        
        Vector2 childScale = _extender.GetChildScale(multipliers.Count);


        foreach (int multiplier in multipliers)
        {
            IndicatorInfo multiplierIndicateInfo = getMultiplierObject();
            Sprite sprite = _listMultiplierDefines.GetSprite(multiplier);
            multiplierIndicateInfo.Setup(multiplier, sprite);
            setScale(childScale.x, multiplierIndicateInfo);
            setPositionToFirstInActiveImage(multiplierIndicateInfo);
        }
    }

    private void setScale(float width, IndicatorInfo multiplierIndicateInfo)
    {
        multiplierIndicateInfo.SizeDelta = new Vector2(width, multiplierIndicateInfo.SizeDelta.y);
    }

    private void setPositionToFirstInActiveImage(IndicatorInfo multiplierIndicateInfo)
    {
        int index = _listMultiplierObject.IndexOf(multiplierIndicateInfo);

        float haftWidth = _rectTransform.sizeDelta.x / 2;

        float multiplierIndicateHaftWidth = multiplierIndicateInfo.SizeDelta.x / 2;
        float distance = multiplierIndicateInfo.SizeDelta.x * index;
        float startPosition = -haftWidth + multiplierIndicateHaftWidth;
        float currentPosition = startPosition + distance;
        multiplierIndicateInfo.AnchoredPositon = new Vector2(currentPosition, multiplierIndicateInfo.AnchoredPositon.y);
    }

    private IndicatorInfo getMultiplierObject()
    {
        IndicatorInfo multiplierIndicateInfo = Instantiate(_multiplierPrefab, transform);
        multiplierIndicateInfo.SetActive(true);
        _listMultiplierObject.Add(multiplierIndicateInfo);
        return multiplierIndicateInfo;
    }
}