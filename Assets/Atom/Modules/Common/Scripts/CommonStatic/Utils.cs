using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Athena
{
    public static class Utils
    {
        public static void RemoveAllChildren(Transform parent)
        {
            int childs = parent.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.Destroy(parent.GetChild(i).gameObject);
            }
        }

        public static Sprite GetSpriteAt(string path)
        {
            return Resources.Load<Sprite>(System.IO.Path.ChangeExtension(path, null));
        }

        public static List<T> ArrayFromJson<T>(List<object> jsonList) where T : IFromJson, new()
        {
            if (jsonList == null || jsonList.Count == 0)
                return null;

            List<T> list = new List<T>();

            for (int i = 0; i < jsonList.Count; i++)
            {
                T data = new T();
                data.FromJson(jsonList[i]);
                list.Add(data);
            }

            return list;
        }

        public static int ConvertInt(object raw)
        {
            return Convert.ToInt32(raw);
        }

        public static List<object> ToListObject<T>(List<T> list)
        {
            List<object> objList = new List<object>();
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                    objList.Add(list[i]);
            }
            return objList;
        }

        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static int GetWeekOfYear(DateTime date)
        {
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            int currentWeek = myCal.GetWeekOfYear(date, myCWR, myFirstDOW);

            return currentWeek;
        }

        public static DateTime StringToDate(string str)
        {
            var longValue = long.Parse(str);
            return LongToDate(longValue);
        }

        public static DateTime LongToDate(long longValue)
        {
            return ParseUnixTimestamp(longValue);
        }

        public static string DateToString(DateTime date)
        {
            return ToUnixTimestamp(date).ToString();
        }

        public static long ToUnixTimestamp(DateTime value)
        {
            return (long)(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static DateTime ParseUnixTimestamp(long timestamp)
        {
            return (new DateTime(1970, 1, 1)).AddSeconds(timestamp);
        }

        public static DateTime MillisecondsToDate(string str)
        {
            var longValue = long.Parse(str);
            const long TicksPerSecond = 10000;
            return new DateTime(longValue * TicksPerSecond);
        }

        public static DateTime MillisecondsToDate(long tick)
        {
            const long TicksPerSecond = 10000;
            return new DateTime(tick * TicksPerSecond);
        }

        public static string GenerateUID()
        {
            string platform = "ios";
            if (Application.platform == RuntimePlatform.Android)
            {
                platform = "android";
            }
            string date1 = DateToString(DateTime.UtcNow);
            var joindate = PlayerPrefs.GetString("USER_PROPERTY_JOIN_DATE", "28071993");
            string s2 = UnityEngine.Random.Range(1, 999999).ToString();
            return Base64Encode(string.Format("{0}_{1}_{2}_{3}", platform, date1, joindate, s2));
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static int VersionCompare(string a, string b)
        {
            return VersionToInt(a) - VersionToInt(b);
        }

        public static int VersionToInt(string version)
        {
            var idx = new int[] { 10000, 100, 1 };
            var arr = version.Split('.');
            int sum = 0;
            for (int i = 0, c = arr.Length; i < c; ++i)
            {
                sum += (int.Parse(arr[i]) * idx[i]);
            }
            return sum;
        }
    }

    public interface IFromJson
    {
        void FromJson(object jsonDict);
    }

    public interface IToJson
    {
        string ToJson();
    }

}
