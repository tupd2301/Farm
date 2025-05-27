using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common.UI;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class CalendarDateItemData
    {
        public System.DateTime Date;
        public bool IsSelected;
        public bool IsCompleted;
        public bool IsAvailable;
    }

    public class CalendarDateUI : MonoBehaviour
    {
        [SerializeField]
        private Image _selectedImage, _starImage, _bgImage;
        [SerializeField]
        private TextMeshProUGUI _dayText;
        [SerializeField]
        private Sprite _bgSprite, _bgGreySprite;

        public System.Action<CalendarDateUI> OnClickedFunc
        {
            get { return _onClickedFunc; }
            set { _onClickedFunc = value; }
        }
        private System.Action<CalendarDateUI> _onClickedFunc = null;

        private CalendarDateItemData _data;
        public void Setup(CalendarDateItemData data)
        {
            _data = data;
            _dayText.text = _data.Date.Day.ToString();
            _selectedImage.gameObject.SetActive(_data.IsSelected);
            _starImage.gameObject.SetActive(_data.IsCompleted);
            _bgImage.sprite = _data.IsAvailable ? _bgSprite : _bgGreySprite;
        }

        public System.DateTime GetDate()
        {
            return _data.Date;
        }

        public void SetSelect(bool isSelected)
        {
            _data.IsSelected = isSelected;
            _selectedImage.gameObject.SetActive(_data.IsSelected);
        }

        public void SetTextColor(Color color)
        {
            _dayText.color = color;
        }

        public void SetTextColorAlpha(float alpha)
        {
            _dayText.color = new Color(_dayText.color.r, _dayText.color.g, _dayText.color.b, alpha);
        }

        public void SetStarEnable(bool enable)
        {
            _data.IsCompleted = enable;
            _starImage.gameObject.SetActive(_data.IsCompleted);
        }

        public void OnClicked()
        {
            if (_onClickedFunc != null)
            {
                _onClickedFunc(this);
            }
        }

        public Vector3 GetStarIconWorldPos()
        {
            return _starImage.transform.position;
        }
    }
}
