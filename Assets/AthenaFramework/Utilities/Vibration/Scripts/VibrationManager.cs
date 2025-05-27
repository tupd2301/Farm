using MoreMountains.NiceVibrations;
using UnityEngine;

namespace AthenaFramework.Utilities
{
    public class VibrationManager : MonoBehaviour
    {
        public static VibrationManager Instance { get; private set; }

        private const string KEY_VIBRATION = "KEY_VIBRATION";
        private bool _isOpen = true;

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                Save(KEY_VIBRATION, _isOpen);
            }
        }

        protected void Awake()
        {
            _isOpen = Load(KEY_VIBRATION, true);

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

        public void LightImpact()
        {
            if (!IsOpen)
                return;
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }

        public void MediumImpact()
        {
            if (!IsOpen)
                return;
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }

        public void HeavyImpact()
        {
            if (!IsOpen)
                return;
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        }
    }
}
