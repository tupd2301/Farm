using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

namespace Atom
{
    public class AudioManager : SingletonMono<AudioManager>
    {
        public void PlaySfxTapButton()
        {
            PlaySound("ClickButton", 1, 0);
        }

        public void ToggleSfx()
        {
            IsEnableSound = !_isEnableSound;
        }

        public void ToggleMusic()
        {
            IsEnableMusic = !_isEnableMusic;
        }

        public void ToggleVibrate()
        {
            IsEnableVibrate = !_isEnableVibrate;
        }

        #region Sound
        const string KEY_VIBRATE = "VIBRATE";
        const string KEY_SOUND = "SOUND";
        const string KEY_MUSIC = "MUSIC";
        [SerializeField]
        private AudioSource[] _regularSoundAudioSources = null;
        [SerializeField]
        private AudioSource[] _soundAudioSource = null;
        [SerializeField]
        private AudioSource _musicAudioSource = null;

        [SerializeField] ScriptableAudioConfig _audioConfig;

        private bool _isEnableSound = true;
        private bool _isEnableMusic = true;
        private bool _isEnableVibrate = true;

        private string _currentMusicId = null;

        private Dictionary<string, AudioClip> _audioClipCaches = new Dictionary<string, AudioClip>();
        private List<string> _regularAudioIds = new List<string>();
        private Dictionary<string, AudioSource> _regularAudioSources = new Dictionary<string, AudioSource>();
        private Dictionary<string, float> _regularAudioLastTimePlay = new Dictionary<string, float>();
        public bool IsEnableSound
        {
            get
            {
                return _isEnableSound;
            }
            set
            {
                _isEnableSound = value;
                Save(KEY_SOUND, _isEnableSound);
            }
        }

        public bool IsEnableVibrate
        {
            get
            {
                return _isEnableVibrate;
            }
            set
            {
                _isEnableVibrate = value;
                Save(KEY_VIBRATE, _isEnableVibrate);
            }
        }

        public bool IsEnableMusic
        {
            get
            {
                return _isEnableMusic;
            }
            set
            {
                _isEnableMusic = value;
                Save(KEY_MUSIC, _isEnableMusic);
                _musicAudioSource.mute = !value;
                if (_musicAudioSource.mute)
                {
                    _musicAudioSource.Pause();
                }
                else
                {
                    _musicAudioSource.Play();
                }
            }
        }

        public void Init(List<string> regularAudioIds)
        {
            _regularAudioIds = regularAudioIds;
            IsEnableSound = Load(KEY_SOUND, true);
            IsEnableMusic = Load(KEY_MUSIC, true);

            for (int i = 0, c = _regularAudioIds.Count; i < c; ++i)
            {
                _regularAudioSources.Add(_regularAudioIds[i], _regularSoundAudioSources[i % _regularSoundAudioSources.Length]);
                _regularAudioLastTimePlay.Add(_regularAudioIds[i], 0);
            }
        }

        private void Save(string key, bool value)
        {
            UnityEngine.PlayerPrefs.SetInt(key, value ? 1 : 0);
            UnityEngine.PlayerPrefs.Save();
        }

        private bool Load(string key, bool defaultValue)
        {
            var value = defaultValue;
            if (UnityEngine.PlayerPrefs.HasKey(key))
            {
                value = UnityEngine.PlayerPrefs.GetInt(key) != 0;
            }
            return value;
        }

        public void PlaySound(string audioId, float volume = 1.0f, int layer = 0)
        {
            if (!_isEnableSound) return;

            var audioClip = findAudioClipById(audioId);
            if (audioClip != null)
            {
                _soundAudioSource[layer].PlayOneShot(audioClip, volume);
            }
        }

        public void PlayRegularSound(string audioId, float volume = 1.0f)
        {
            if (!_isEnableSound) return;

            var audioClip = findAudioClipById(audioId);

            if (_regularAudioSources.ContainsKey(audioId))
            {
                var sources = _regularAudioSources[audioId];
                var lastTime = _regularAudioLastTimePlay[audioId];
                if (Time.timeSinceLevelLoad - lastTime < 0.05f)
                {
                    return;
                }
                _regularAudioLastTimePlay[audioId] = Time.timeSinceLevelLoad;
                if (sources.clip == audioClip)
                {
                    // sources.volume = volume;
                    sources.PlayOneShot(audioClip, volume);
                }
                else
                {
                    sources.clip = audioClip;
                    sources.volume = volume;
                    // sources.Play();
                    sources.PlayOneShot(audioClip, volume);
                }
            }
            else
            {
                PlaySound(audioId, volume);
            }

        }

        public void PlayBackgroundMusic(string audioId)
        {
            _currentMusicId = audioId;
            var audioClip = findAudioClipById(audioId);
            if (_musicAudioSource.clip != audioClip)
            {
                _musicAudioSource.clip = audioClip;
            }
            _musicAudioSource.loop = true;
            _musicAudioSource.Play();
        }

        public void StopBackgroundMusic()
        {
            _currentMusicId = null;
            _musicAudioSource.Stop();
        }

        public void PauseMusicBg()
        {
            _musicAudioSource.Pause();
        }

        public void ResumeMusicBg()
        {
            throw new System.NotImplementedException();
        }

        private AudioClip findAudioClipById(string audioId)
        {
            var audioClipData = _audioConfig.AudioData;
            AudioClip tmp = null;
            if (audioId != null && !_audioClipCaches.TryGetValue(audioId, out tmp))
            {
                for (int i = 0, c = audioClipData.Length; i < c; ++i)
                {
                    if (audioId == audioClipData[i].AudioId)
                    {
                        tmp = audioClipData[i].AudioClip;
                        break;
                    }
                }
                _audioClipCaches.Add(audioId, tmp);
            }

            return tmp;
        }

        private class AudioPlayedTime
        {
            public string AudioId;
            public int SimultaneousPlayCount;
        }
        #endregion
    }
}