using UnityEngine;

namespace Atom
{
    [CreateAssetMenu(menuName = "Atom/Audio Config")]
    public class ScriptableAudioConfig : ScriptableObject
    {
        public AudioClipData[] AudioData => _audioClipData;

        [SerializeField] AudioClipData[] _audioClipData;

        [System.Serializable]
        public class AudioClipData
        {
            public AudioClip AudioClip;
            public string AudioId;
        }
    }
}