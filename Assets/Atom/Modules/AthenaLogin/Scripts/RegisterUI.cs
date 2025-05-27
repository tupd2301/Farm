using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class RegisterUI : MonoBehaviour
    {

        public System.Action OnGoBack;
        public System.Action OnRegister;
        public System.Action OnPasswordClick;
        public System.Action OnEmailClick;
        [SerializeField] private InputField _idInputField;
        [SerializeField] private InputField _passwordInputField;
        [SerializeField] private InputField _confirmPasswordInputField;
        [SerializeField] private InputField _emailInputField;
        [SerializeField] private GameObject _errorGameObject;
        [SerializeField] private GameObject _emailErrorGameObject;
        [SerializeField] private Text _errorText;
        [SerializeField] private Text _emailErrorText;
        [SerializeField] private Transform _groupConfirm;
        [SerializeField] private Transform _groupEmail;
        [SerializeField] private Text _headerText;
        [SerializeField] private Text _buttonText;

        public void SetHeaderText(string headerText)
        {
            _headerText.text = headerText;
        }

        public void SetButtonText(string buttonText)
        {
            _buttonText.text = buttonText;
        }

        public void SetErrorText(string error)
        {
            if (error == string.Empty)
            {
                _errorGameObject.SetActive(false);
                _groupConfirm.localPosition = Vector3.zero;
            }
            else
            {
                _errorGameObject.SetActive(true);
                _groupConfirm.localPosition = new Vector3(0, -100, 0);
            }
            _errorText.text = error;
        }

        public void SetEmailErrorText(string error)
        {
            if (error == string.Empty)
            {
                _emailErrorGameObject.SetActive(false);
                _groupEmail.localPosition = Vector3.zero;
            }
            else
            {
                _emailErrorGameObject.SetActive(true);
                _groupEmail.localPosition = new Vector3(0, -100, 0);
            }
            _emailErrorText.text = error;
        }

        public string GetID()
        {
            return _idInputField.text;
        }
        public string GetPass()
        {
            return _passwordInputField.text;
        }
        public string GetRepass()
        {
            return _confirmPasswordInputField.text;
        }
        public string GetEmail()
        {
            return _emailInputField.text;
        }

        public void GoBack()
        {
            OnGoBack?.Invoke();
        }

        public void Register()
        {
            OnRegister?.Invoke();
        }

        public void PasswordClick()
        {
            UnityEngine.Debug.Log("============");
            OnPasswordClick?.Invoke();
        }

        public void EmailClick()
        {
            UnityEngine.Debug.Log("============");
            OnEmailClick?.Invoke();
        }
    }
}
