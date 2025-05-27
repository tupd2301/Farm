using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common.UI;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class TopUI : UIController
    {
        public TextMeshProUGUI TextCoin; 
        public System.Action OnButtonSettingClick;

        public void ClickButtonSetting()
        {
            OnButtonSettingClick?.Invoke();
        }
    }
}
