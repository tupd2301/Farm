using System;
using UnityEngine;

namespace Atom
{
    public static class SaveDataHelper
    {
        public static T GetJSONData<T>(string key, T defaultData)
        {
            string jsonString = Get(key, "");
            if (string.IsNullOrEmpty(jsonString))
            {
                return defaultData;
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static void SetJSONData<T>(string key, T data)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Set(key, json);
        }

        public static void Set(string key, string data)
        {
            PlayerPrefs.SetString(key, EncryptionUtil.Encrypt(data));
            PlayerPrefs.Save();
        }

        public static string Get(string key, string defaultValue)
        {
            var encryptText = PlayerPrefs.GetString(key, "");
            if (!string.IsNullOrEmpty(encryptText))
            {
                return EncryptionUtil.Decrypt(encryptText);
            }
            return "";
        }

        public static void Set(string key, bool value)
        {
            UnityEngine.PlayerPrefs.SetInt(key, value ? 1 : 0);
            UnityEngine.PlayerPrefs.Save();
        }

        public static bool Get(string key, bool defaultValue)
        {
            var value = defaultValue;
            if (UnityEngine.PlayerPrefs.HasKey(key))
            {
                value = UnityEngine.PlayerPrefs.GetInt(key) != 0;
            }
            return value;
        }

        public static void Set(string key, int value)
        {
            UnityEngine.PlayerPrefs.SetInt(key, value);
            UnityEngine.PlayerPrefs.Save();
        }

        public static int Get(string key, int defaultValue)
        {
            var value = defaultValue;
            if (UnityEngine.PlayerPrefs.HasKey(key))
            {
                value = UnityEngine.PlayerPrefs.GetInt(key);
            }
            return value;
        }

        public static void Set(string key, DateTime value)
        {
            UnityEngine.PlayerPrefs.SetString(key, Athena.Utils.DateToString(value));
            UnityEngine.PlayerPrefs.Save();
        }

        public static DateTime Get(string key, DateTime defaultValue)
        {
            var value = defaultValue;
            if (UnityEngine.PlayerPrefs.HasKey(key))
            {
                var str = UnityEngine.PlayerPrefs.GetString(key, "");
                if (!string.IsNullOrEmpty(str))
                {
                    value = Athena.Utils.StringToDate(str);
                }
            }
            return value;
        }
    }
}