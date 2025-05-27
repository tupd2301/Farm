using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.UI;
using StarShipFramework.SaveProgress;
using Google;
using System.Threading.Tasks;
using CustomUtils;

namespace OneID
{
    public partial class LoginManager : SingletonMono<LoginManager>
    {
        System.Action _onLoginSuccess;

        // Start is called before the first frame update
        public void LoginWithDevices(System.Action callback = null)
        {
            _onLoginSuccess = callback;

            var requestCustomID = new LoginWithCustomIDRequest { CustomId = GetDeviceID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithCustomID(requestCustomID, OnMobileDeviceLoginSuccess, OnLoginFailure);
        }

        private void OnMobileDeviceLoginSuccess(LoginResult result)
        {
            Debug.Log("OnMobileDeviceLoginSuccess" + result);
            _entityId = result.EntityToken.Entity.Id;
            _entityType = result.EntityToken.Entity.Type;
            _isNewAccountCreated = result.NewlyCreated;
            _playfabUserId = result.PlayFabId;
            PlayerPrefs.SetString(PlayerPrefKey.PlayfabID, result.PlayFabId);

            _onLoginSuccess?.Invoke();
        }

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.Log("OnLoginFailure" + error);
        }

        public string GetDeviceID()
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefKey.CustomIDSignIn)))
            {
                return PlayerPrefs.GetString(PlayerPrefKey.CustomIDSignIn);
            }

            string deviceID = PlayerPrefs.GetString(PlayerPrefKey.DeviceIDKey, "");
            if (deviceID.Length <= 0)
            {
#if UNITY_IOS && !UNITY_EDITOR
                deviceID = UnityEngine.iOS.Device.vendorIdentifier;
#else
                deviceID = SystemInfo.deviceUniqueIdentifier;
#endif

                PlayerPrefs.SetString(PlayerPrefKey.DeviceIDKey, deviceID);
            }
            deviceID += Random.Range(100000, 900000).ToString();
            Debug.Log("GetDeviceID " + deviceID);
            return deviceID;
        }
    }
}