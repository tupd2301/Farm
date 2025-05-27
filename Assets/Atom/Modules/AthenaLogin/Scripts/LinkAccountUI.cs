using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class LinkAccountUI : MonoBehaviour
    {
        public System.Action OnGoBack;

        public System.Action OnLinkFacebook;
        public System.Action OnLinkGoogle;
        public System.Action OnLinkAppleId;
        public System.Action OnLinkUserEmail;

        [SerializeField] private GameObject _facebookButton;
        [SerializeField] private GameObject _googleButton;
        [SerializeField] private GameObject _appleButton;

        [SerializeField] private Text _facebookText;
        [SerializeField] private Text _googleText;
        [SerializeField] private Text _appleText;

        public void ShowUI(bool hasGoogleAccount, bool hasAppleAccount, bool hasFacebookAccount)
        {
            _facebookButton.gameObject.SetActive(true);
            _googleButton.gameObject.SetActive(false);
            _appleButton.gameObject.SetActive(false);
#if UNITY_ANDROID
            _googleButton.gameObject.SetActive(true);
#elif UNITY_IOS
            _appleButton.gameObject.SetActive(true);
#endif

            _facebookText.text = hasFacebookAccount ? "CONNECTED TO FACEBOOK" : "SIGN IN WITH FACEBOOK";
            _googleText.text = hasGoogleAccount ? "CONNECTED TO GOOGLE" : "SIGN IN WITH GOOGLE";
            _appleText.text = hasAppleAccount ? "CONNECTED TO APPLE" : "SIGN IN WITH APPLE";

            _facebookButton.GetComponent<Button>().interactable = !hasFacebookAccount;
            _googleButton.GetComponent<Button>().interactable = !hasGoogleAccount;
            _appleButton.GetComponent<Button>().interactable = !hasAppleAccount;
        }

        public void GoBack()
        {
            OnGoBack?.Invoke();
        }

        public void LinkFacebook()
        {
            OnLinkFacebook?.Invoke();
        }

        public void LinkGoogle()
        {
            OnLinkGoogle?.Invoke();
        }

        public void LinkAppleId()
        {
            OnLinkAppleId?.Invoke();
        }

        public void LinkUserEmail()
        {
            OnLinkUserEmail?.Invoke();
        }
    }
}