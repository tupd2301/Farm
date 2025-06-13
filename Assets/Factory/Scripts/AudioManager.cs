using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Space]
    [SerializeField]
    AudioSource _soundAudioSource;

    [SerializeField]
    AudioSource _musicAudioSource;

    [Header("Sound")]
    [SerializeField]
    List<AudioConfig> _commonAudioClips;

    [SerializeField]
    AudioClip _combo1AudioClip;

    [SerializeField]
    AudioClip _combo2AudioClip;

    [SerializeField]
    AudioClip _combo3AudioClip;

    [Header("Music")]
    [SerializeField]
    List<AudioClip> _bgMusics;

    private const string KEY_SOUND = "KEY_SOUND";
    private const string KEY_MUSIC = "KEY_MUSIC";
    private bool _isEnableSound = true;
    private bool _isEnableMusic = true;
    private int _currentBackgroundMusicIndex;

    private Dictionary<string, AudioClip> _audioClipDict;
    private Coroutine _playBackgroundMusicCoroutine;

    public bool IsEnableSound
    {
        get { return _isEnableSound; }
        set
        {
            _isEnableSound = value;
            Save(KEY_SOUND, _isEnableSound);

            _soundAudioSource.DOKill();
            if (_isEnableSound)
            {
                _soundAudioSource.DOFade(1, 2f);
            }
            else
            {
                _soundAudioSource.DOFade(0, 2f);
            }
        }
    }

    public bool IsEnableMusic
    {
        get { return _isEnableMusic; }
        set
        {
            _isEnableMusic = value;
            Save(KEY_MUSIC, _isEnableMusic);

            _musicAudioSource.DOKill();
            if (_isEnableMusic)
            {
                _musicAudioSource.DOFade(1, 2f);
            }
            else
            {
                _musicAudioSource.DOFade(0, 2f);
            }
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Save(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private bool Load(string key, bool defaultValue)
    {
        var value = defaultValue;
        if (PlayerPrefs.HasKey(key))
        {
            value = PlayerPrefs.GetInt(key) != 0;
        }
        return value;
    }

    public void Start()
    {
        _isEnableSound = Load(KEY_SOUND, true);
        _isEnableMusic = Load(KEY_MUSIC, true);

        _currentBackgroundMusicIndex = 0;

        _audioClipDict = new();

        foreach (var audioClipConfig in _commonAudioClips)
        {
            _audioClipDict.Add(audioClipConfig.AudioId, audioClipConfig.AudioClip);
        }
        PlayBackgroundMusic();
    }

    public void PlaySound(AudioClip clip, float volumne = 1.0f)
    {
        if (!_isEnableSound)
        {
            return;
        }

        if (clip != null)
        {
            _soundAudioSource.PlayOneShot(clip, volumne);
        }
    }

    public void PlaySound(string id, float volumne = 1.0f)
    {
        var audioClip = _audioClipDict[id];

        if (audioClip != null)
        {
            PlaySound(audioClip, volumne);
        }
    }

    public void PlayCombo(int combo)
    {
        if (combo == 1)
        {
            PlaySound(_combo1AudioClip);
        }
        else if (combo == 2)
        {
            PlaySound(_combo2AudioClip);
        }
        else if (combo >= 3)
        {
            PlaySound(_combo3AudioClip);
        }
    }

    public void PlayBackgroundMusic()
    {
        // StopBackgroundMusic();
        // _playBackgroundMusicCoroutine = StartCoroutine(PlayBackgroundMusicWithFade());
        _musicAudioSource.clip = _bgMusics[0];
        _musicAudioSource.volume = 0.5f;
        _musicAudioSource.Play();
    }

    public void StopBackgroundMusic()
    {
        if (_playBackgroundMusicCoroutine == null)
        {
            return;
        }

        StopCoroutine(_playBackgroundMusicCoroutine);
        _playBackgroundMusicCoroutine = null;

        _musicAudioSource
            .DOFade(0, 2f)
            .OnComplete(() =>
            {
                _musicAudioSource.Stop();
            });
    }

    private IEnumerator PlayBackgroundMusicWithFade()
    {
        _musicAudioSource.DOKill();

        while (true)
        {
            _musicAudioSource.volume = 0;
            _musicAudioSource.clip = _bgMusics[_currentBackgroundMusicIndex];
            _musicAudioSource.Play();

            if (_isEnableMusic)
            {
                _musicAudioSource.DOFade(1, 2f);
            }

            yield return new WaitForSeconds(30f);

            _musicAudioSource
                .DOFade(0, 2f)
                .OnComplete(() =>
                {
                    _musicAudioSource.Stop();
                });

            yield return new WaitForSeconds(30f);

            _currentBackgroundMusicIndex = (_currentBackgroundMusicIndex + 1) % _bgMusics.Count;
        }
    }
}

[System.Serializable]
public struct AudioConfig
{
    public AudioClip AudioClip;
    public string AudioId;
}
