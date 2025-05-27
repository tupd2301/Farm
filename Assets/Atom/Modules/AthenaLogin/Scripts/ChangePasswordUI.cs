using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class ChangePasswordUI : MonoBehaviour
    {
        public System.Action OnGoBack;
        public System.Action OnConfirm;

        [SerializeField] private InputField _idInputField;
        [SerializeField] private InputField _emailInputField;
        [SerializeField] private Text _logoText;
        [SerializeField] private Text _buttonText;
        private string _emailAccount;
        private bool _isInteractable;

        public string TextEmail
        {
            get
            {
                if (_isInteractable)
                {
                    return _emailInputField.text;
                }
                else
                {
                    return _emailAccount;
                }
            }
        }

        public void ShowEmailAccount(string emailAccount, bool isInteractable)
        {
            _emailAccount = emailAccount;
            _isInteractable = isInteractable;
            _emailInputField.interactable = isInteractable;
            
            if (emailAccount.Contains("@"))
            {
                string[] split = emailAccount.Split(new char[] { '@' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1)
                {
                    int lengthEmail = split[0].Length;
                    if (lengthEmail > 2)
                    {
                        _emailInputField.text = split[0].Substring(0, 3) + "*****";
                    }
                    else
                    {
                        _emailInputField.text = split[0] + "*****";
                    }
                    _emailInputField.text += "@" + split[1];
                }
            }
            if (isInteractable)
            {
                _emailInputField.text = string.Empty;
            }
            if (isInteractable)
            {
                _logoText.text = "FORGOT PASSWORD";
                _buttonText.text = "CONTINUE";
            }
            else
            {
                _logoText.text = "CHANGE PASSWORD";
                _buttonText.text = "CHANGE PASSWORD";
            }
        }

        public void Confirm()
        {
            OnConfirm?.Invoke();
        }
        public void GoBack()
        {
            OnGoBack?.Invoke();
        }
    }
}