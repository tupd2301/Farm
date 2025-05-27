using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common.UI;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class DailyLevelCompleteUI : UIController
    {
        public System.Action onCollectPressed;

        [SerializeField]
        private TextMeshProUGUI _completeText;
        [SerializeField]
        private GameObject _star;

        public void Setup(System.DateTime date)
        {
            _completeText.text = string.Format("You have completed the Daily Challenge for {0}!", 1);
        }

        public void OnCollectPressed()
        {
            //AudioManager.Instance.PlaySFX(AudioId.ButtonTap, usePlayIndex: true);
            onCollectPressed?.Invoke();
        }
    }
}