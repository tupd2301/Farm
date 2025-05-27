using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class EmailLoginUI : MonoBehaviour
    {
        public System.Action OnGoBack;
        public System.Action OnLogin;
        public System.Action OnShowRegisterUI;
        public System.Action OnForgotPassword;

        [SerializeField] private InputField _emailInputField;
        [SerializeField] private InputField _passInputField;
        [SerializeField] private Text _errorText;
        [SerializeField] private Sprite _spriteTextError;

        public void SetErrorText(string error)
        {
            _errorText.text = error;
            if (!string.IsNullOrEmpty(error))
            {
                _emailInputField.GetComponent<Image>().sprite = _spriteTextError;
                _passInputField.GetComponent<Image>().sprite = _spriteTextError;
            }
        }
        
        public string GetEmail()
        {
            return _emailInputField.text;
        }
        public string GetPassword()
        {
            return _passInputField.text;
        }
        public void Login()
        {
            OnLogin?.Invoke();
        }
        public void GoBack()
        {
            OnGoBack?.Invoke();
        }

        public void ShowRegister()
        {
            OnShowRegisterUI?.Invoke();
        }

        public void ForgotPassword()
        {
            OnForgotPassword?.Invoke();
        }
    }
}