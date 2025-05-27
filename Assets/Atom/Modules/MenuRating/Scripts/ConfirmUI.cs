using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Athena.Common;
using Athena.Common.UI;
//using Athena.BlockBuster.UI;
//using DG.Tweening;
using TMPro;

namespace Atom
{
    public class ConfirmUI : FlexibleUIController
    {
        public System.Action onOKPressed, onCancelPressed;

        [SerializeField]
        private TextMeshProUGUI _title, _text, _buttonText;
        [SerializeField]
        private Image _image;

        protected override void OnBack()
        {
            if (IgnoredBackKey)
            {
                return;
            }

            if (UIManager.Instance.IsActiveOnTop(this))
            {
                OnCancelPressed();
            }
        }

        public void Setup(string title = null, string text = null, string buttonText = null)
        {
            if (!string.IsNullOrEmpty(title))
            {
                _title.text = title;
            }

            if (!string.IsNullOrEmpty(text))
            {
                _text.text = text;
            }

            if (!string.IsNullOrEmpty(buttonText))
            {
                _buttonText.text = buttonText;
            }

            UIAnimUtils.AssignAnimForChildButtons(this.transform);
        }

        public void SetText(string text)
        {
            if (_text == null)
            {
                return;
            }
            _text.text = text;
        }

        public void SetButtonText(string text)
        {
            if (_buttonText == null)
            {
                return;
            }
            _buttonText.text = text;
        }

        public void SetTitle(string text)
        {
            if (_title == null)
            {
                return;
            }
            _title.text = text;
        }

        public void SetImage(Sprite image)
        {
            if (_image == null)
            {
                return;
            }
            _image.overrideSprite = image;
        }

        public override void Show()
        {
            base.Show();

            onShowStarted?.Invoke();
            UIAnimUtils.PlayShowPopUpAnim(this.transform, onShowFinished);
        }

        public override void Hide()
        {
            base.Hide();

            onHideStarted?.Invoke();
            UIAnimUtils.PlayHidePopUpAnim(this.transform, onHideFinished);
        }

        public void OnCancelPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onCancelPressed?.Invoke();
        }

        public void OnOKPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onOKPressed?.Invoke();
        }

        public void SetLayer(string layer)
        {
            Canvas canvas = GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = layer;
        }
    }
}