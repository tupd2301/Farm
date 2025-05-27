using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class GuestAccountUI : MonoBehaviour
    {
        public System.Action OnGoBack;
        public System.Action OnShowUpdateInfoUI;
        public System.Action OnShowChangePasswordUI;
        public System.Action OnShowLinkAccountUI;
        public System.Action OnSwitchAccount;

        [SerializeField] private GameObject _updateInfoButton;
        [SerializeField] private GameObject _changePasswordButton;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _coinText;

        public void ShowUI(bool hasEmailAccount, string nameAccount, string emailAccount)
        {
            _nameText.text = nameAccount;
            _updateInfoButton.SetActive(!hasEmailAccount);
            _changePasswordButton.SetActive(hasEmailAccount);
        }

        public void GoBack()
        {
            OnGoBack?.Invoke();
        }

        public void ShowUpdateInfoUI()
        {
            OnShowUpdateInfoUI?.Invoke();
        }

        public void ShowChangePasswordUI()
        {
            OnShowChangePasswordUI?.Invoke();
        }

        public void ShowLinkAccountUI()
        {
            OnShowLinkAccountUI?.Invoke();
        }

        public void SwitchAccount()
        {
            OnSwitchAccount?.Invoke();
        }
    }
}
