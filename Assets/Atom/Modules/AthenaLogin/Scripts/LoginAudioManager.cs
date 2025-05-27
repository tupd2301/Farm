using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

namespace OneID
{
    public class LoginAudioManager : SingletonMono<LoginAudioManager>
    {
        [Header("Sound Configs")]
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip _sfxTapButton;

        const string KEY_SFX = "SFX";
        const string KEY_VIBRATE = "VIBRATE";

        bool _isSfxOn;
        bool _isVibrate;

        public event System.Action<bool> OnSfxChanged;
        public event System.Action<bool> OnVibrateChanged;

        void LoadSoundSettings()
        {
            OnSfxChanged += (isSfxOn) =>
            {
                _audioSource.volume = _isSfxOn ? 1f : 0f;
            };

            OnVibrateChanged += (isVibrateOn) =>
            {

            };

            IsSfxOn = PlayerPrefs.GetInt(KEY_SFX, 1) > 0 ? true : false;
            IsVibrateOn = PlayerPrefs.GetInt(KEY_VIBRATE, 1) > 0 ? true : false;

            _audioSource.volume = _isSfxOn ? 1f : 0f;
        }

        public void ToggleSfx()
        {
            IsSfxOn = !_isSfxOn;
        }

        public void ToggleVibrate()
        {
            IsVibrateOn = !_isVibrate;
        }

        public bool IsSfxOn
        {
            get
            {
                return _isSfxOn;
            }

            set
            {
                if (value != _isSfxOn)
                {
                    _isSfxOn = value;
                    PlayerPrefs.SetInt(KEY_SFX, _isSfxOn ? 1 : 0);
                    PlayerPrefs.Save();

                    OnSfxChanged?.Invoke(value);
                }
            }
        }

        public bool IsVibrateOn
        {
            get
            {
                return _isVibrate;
            }

            set
            {
                if (value != _isVibrate)
                {
                    _isVibrate = value;
                    PlayerPrefs.SetInt(KEY_VIBRATE, _isVibrate ? 1 : 0);
                    PlayerPrefs.Save();

                    OnVibrateChanged?.Invoke(value);
                }
            }
        }

        public void PlaySfxTapButton()
        {
            _audioSource.PlayOneShot(_sfxTapButton);
        }
    }
}