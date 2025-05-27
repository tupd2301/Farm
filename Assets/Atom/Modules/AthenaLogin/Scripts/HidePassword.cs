using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class HidePassword : MonoBehaviour
    {
        [SerializeField]
        public Sprite _hideSprite;
        [SerializeField]
        public Sprite _unHideSprite;
        [SerializeField]
        private Image _hideImage;
        [SerializeField]
        public InputField _passwordInputField;

        private bool _hidePassword;

        // Start is called before the first frame update
        void Start()
        {
            _hidePassword = true;
        }

        public void OnHideButtonClick()
        {
            _hidePassword = !_hidePassword;
            if (_hidePassword)
            {
                _hideImage.sprite = _hideSprite;
                _hideImage.SetNativeSize();
                _passwordInputField.contentType = InputField.ContentType.Password;
                _passwordInputField.ForceLabelUpdate();
            }
            else
            {
                _hideImage.sprite = _unHideSprite;
                _hideImage.SetNativeSize();
                _passwordInputField.contentType = InputField.ContentType.Standard;
                _passwordInputField.ForceLabelUpdate();
            }
        }    
    }
}