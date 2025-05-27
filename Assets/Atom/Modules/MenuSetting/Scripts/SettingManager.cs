using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtils;
using Athena.Common.UI;

namespace Atom
{
    public class SettingManager : SingletonMono<SettingManager>
    {
        protected SettingUI _settingUI;

        public SettingUI SettingUI
        {
            get
            {
                return _settingUI;
            }
        }

        public void ShowUI()
        {
            _settingUI = AppManager.Instance.ShowSafeTopUI<SettingUI>("Atom/SettingUI", false);
            _settingUI.RefreshSound();
            _settingUI.RefreshMusic();
            _settingUI.RefreshVibration();
            _settingUI.onClosePressed = () =>
            {
                UIManager.Instance.ReleaseUI(_settingUI, true);
            };
            _settingUI.onVibrationPressed = () => {
                //UIFeedbackManager.Instance.IsEnabled = !UIFeedbackManager.Instance.IsEnabled;
                //_settingUI.RefreshVibration(UIFeedbackManager.Instance.IsEnabled);
                AudioManager.Instance.ToggleVibrate();
                _settingUI.RefreshVibration();
            };
            _settingUI.onMusicPressed = () => {
                AudioManager.Instance.ToggleMusic();
                _settingUI.RefreshMusic();
            };
            _settingUI.onSoundPressed = () => {
                AudioManager.Instance.ToggleSfx();
                _settingUI.RefreshSound();
            };
            _settingUI.Show();
        }
    }
}