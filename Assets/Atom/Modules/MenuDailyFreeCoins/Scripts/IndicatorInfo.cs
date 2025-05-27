using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IndicatorInfo : MonoBehaviour
{
	public Image Image;
	public TextMeshProUGUI TextMesh;

    private int _value;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public RectTransform RectTransform
    {
        get
        {
            if(_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    public Vector2 Position
    {
        get => transform.position;
    }

    public Vector2 AnchoredPositon
    {
        get => RectTransform.anchoredPosition;
        set => RectTransform.anchoredPosition = value;
    }

    public Vector2 SizeDelta
    {
        get => RectTransform.sizeDelta;
        set => RectTransform.sizeDelta = value;
    }

    public bool ActiveSelf
    {
        get => gameObject.activeSelf;
    }

    public Sprite Sprite
    {
        get => Image.sprite;
        set => Image.sprite = value;
    }

    public int Value
    {
        get => _value;
    }

    public void Setup(int value, Sprite sprite)
    {
        Image.sprite = sprite;
        TextMesh.text = $"x{value}";
        _value = value;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}

