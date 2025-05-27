using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class MainOneSDKUI : MonoBehaviour
    {

        public System.Action OnShowGuestAccountUI;
        public System.Action OnSwitchAccount;

        public void ShowGuestAccountUI()
        {
            OnShowGuestAccountUI?.Invoke();
        }

        public void SwitchAccount()
        {
            OnSwitchAccount?.Invoke();
        }
    }
}