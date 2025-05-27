using UnityEngine;

namespace Athena.Common
{
    public static class LocalDatabase
    {
        /// <summary>
        /// Get PlayerPrefs value by key
        /// </summary>
        /// <returns>
        /// Returns value that is saved in PlayerPrefs by given key
        /// </returns>
        public static T GetValue<T>(string key, T defaultValue)
        {
            if (typeof(T).Equals(typeof(int)))
            {
                return (T)System.Convert.ChangeType(PlayerPrefs.GetInt(key, int.Parse(defaultValue.ToString())), typeof(T));
            }

            if (typeof(T).Equals(typeof(float)))
            {
                return (T)System.Convert.ChangeType(PlayerPrefs.GetFloat(key, float.Parse(defaultValue.ToString())), typeof(T));
            }

            return (T)System.Convert.ChangeType(PlayerPrefs.GetString(key, defaultValue.ToString()), typeof(T));
        }

        /// <summary>
        /// Set PlayerPrefs value by given key
        /// </summary>
        public static void SetValue<T>(string key, T value)
        {
            if (typeof(T).Equals(typeof(int)))
            {
                PlayerPrefs.SetInt(key, int.Parse(value.ToString()));
                PlayerPrefs.Save();
                return;
            }

            if (typeof(T).Equals(typeof(float)))
            {
                PlayerPrefs.SetFloat(key, float.Parse(value.ToString()));
                PlayerPrefs.Save();
                return;
            }
            PlayerPrefs.SetString(key, value.ToString());
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Get JSON Data that is saved in PlayerPrefs by given key
        /// </summary>
        public static T GetJSONData<T>(string key, T defaultData)
        {
            string jsonString = PlayerPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(jsonString))
            {
                return defaultData;
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Set JSON Data by given key
        /// </summary>
        public static void SetJSONData<T>(string key, T data)
        {
            PlayerPrefs.SetString(key, Newtonsoft.Json.JsonConvert.SerializeObject(data));
            PlayerPrefs.Save();
        }

        public static bool ContainKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}