using System.Collections.Generic;
using UnityEngine;

public static class Yielders
{
    static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);

    static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    public static WaitForEndOfFrame EndOfFrame
    {
        get { return _endOfFrame; }
    }

    static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
    public static WaitForFixedUpdate FixedUpdate
    {
        get { return _fixedUpdate; }
    }

    public static WaitForSeconds Get(float seconds)
    {
        if (!_timeInterval.ContainsKey(seconds))
            _timeInterval.Add(seconds, new WaitForSeconds(seconds));
        return _timeInterval[seconds];
    }

    public static T GetComponentEnsure<T>(this GameObject obj) where T : MonoBehaviour
    {
        var x = obj.GetComponent<T>();
        if (x == null)
        {
            x = obj.AddComponent<T>();
        }
        return x;
    }
}