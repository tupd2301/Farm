using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common.UI;
using UnityEngine.UI;

namespace Atom
{
    public class MainMenuUI : UIController
    {
        public System.Action OnOpenSetting;
        public System.Action OnOpenRating;
        public System.Action OnOpenMessages;
        public System.Action OnButtonPlayClick;

        public void OpenSetting()
        {
            OnOpenSetting?.Invoke();
        }

        public void OpenMessages()
        {
            OnOpenMessages?.Invoke();
        }

        public void OpenRating()
        {
            OnOpenRating?.Invoke();
        }

        public void ClickButtonPlay()
        {
            OnButtonPlayClick?.Invoke();
        }
    }
}