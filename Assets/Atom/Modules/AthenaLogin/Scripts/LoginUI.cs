using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class LoginUI : MonoBehaviour
    {

        public System.Action OnShowLoginPopupUI;
        public System.Action OnLoginGuest;
        public System.Action OnLoginFacebook;
        public System.Action OnLoginAppleId;
        public System.Action OnLoginGoogle;

        [SerializeField]
        public GameObject _loginAppleButton;
        [SerializeField]
        public GameObject _loginGoogleButton;

        public void ShowUI()
        {
            //_loginAppleButton.SetActive(false);
            //_loginGoogleButton.SetActive(true);
#if UNITY_IOS
            //_loginAppleButton.SetActive(true);
            //_loginGoogleButton.SetActive(false);
#endif
        }

        public void ShowLoginPopupUI()
        {
            OnShowLoginPopupUI?.Invoke();
        }

        public void LoginGuest()
        {
            OnLoginGuest?.Invoke();
        }

        public void LoginFacebook()
        {
            OnLoginFacebook?.Invoke();
        }

        public void LoginAppleId()
        {
            OnLoginAppleId?.Invoke();
        }

        public void LoginGoogle()
        {
            OnLoginGoogle?.Invoke();
        }
    }
}