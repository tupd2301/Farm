using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common;
using Athena.Common.UI;
using TMPro;

namespace Atom
{
    public class SettingUI : FlexibleUIController
    {
        public System.Action onMusicPressed, onSoundPressed, onQuiPressed, onTutorialPresesd, onRetryPressed,
                             onClosePressed, onHomePressed, onLeaderboardPressed, onMorePressed, onVibrationPressed;

        [SerializeField]
        private SwitchUI _musicSwitchUI, _soundSwitchUI, _vibrationSwitchUI;

        public void Setup()
        {
            //UIAnimUtils.AssignAnimForChildButtons(this.transform);
        }

        protected override void OnBack()
        {
            if (UIManager.Instance.IsActiveOnTop(this))
            {
                OnClosePressed();
            }
        }

        public override void Show()
        {
            base.Show();

            //onShowStarted?.Invoke();
            UIAnimUtils.PlayShowPopUpAnim(this.transform, onShowFinished);
        }

        public override void Hide()
        {
            base.Hide();

            onHideStarted?.Invoke();
            UIAnimUtils.PlayHidePopUpAnim(this.transform, onHideFinished);
        }

        public void OnVibrationPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onVibrationPressed?.Invoke();
        }

        public void OnMusicPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onMusicPressed?.Invoke();
        }

        public void OnSoundPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onSoundPressed?.Invoke();
        }

        public void OnClosePressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onClosePressed?.Invoke();
        }

        public void OnQuitPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onQuiPressed?.Invoke();
        }

        public void OnHomePressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onHomePressed?.Invoke();
        }

        public void OnTutorialPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onTutorialPresesd?.Invoke();
        }

        public void OnLeaderboardPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onLeaderboardPressed?.Invoke();
        }

        public void OnMorePressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onMorePressed?.Invoke();
        }

        public void OnRetryPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onRetryPressed?.Invoke();
        }

        public void RefreshVibration()
        {
            if (AudioManager.Instance.IsEnableVibrate)
            {
                _vibrationSwitchUI.SetOn();
            }
            else
            {
                _vibrationSwitchUI.SetOff();
            }
        }

        public void RefreshMusic()
        {
            if (AudioManager.Instance.IsEnableMusic)
            {
                _musicSwitchUI.SetOn();
            }
            else
            {
                _musicSwitchUI.SetOff();
            }
        }

        public void RefreshSound()
        {
            if (AudioManager.Instance.IsEnableSound)
            {
                _soundSwitchUI.SetOn();
            }
            else
            {
                _soundSwitchUI.SetOff();
            }
        }
    }
}