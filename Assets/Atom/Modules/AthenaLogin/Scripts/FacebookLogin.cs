using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_FACEBOOK
using Facebook.Unity;
#endif
using UnityEngine.Networking;
using UnityEngine.Events;
namespace StarShipFramework.SaveProgress
{
    public class FacebookLogin
    {
        private UnityAction<bool, string> callBackUnityLogin = null;
        private UnityAction callBackFbInitDone = null;
        // Awake function from Unity's MonoBehavior
        public void Init(UnityAction callBackInitDone = null)
        {
            callBackFbInitDone = callBackInitDone;
#if USE_FACEBOOK
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
                if (callBackFbInitDone != null)
                {
                    callBackFbInitDone();
                }
            }
#endif
        }

        private void InitCallback()
        {
#if USE_FACEBOOK
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Debug.Log("Success to Initialize the Facebook SDK");
                if (callBackFbInitDone != null)
                {
                    callBackFbInitDone();
                }
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                // Debug.Log("Failed to Initialize the Facebook SDK");
            }
#endif
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
        public void Login(UnityAction<bool, string> callBack = null)
        {
#if USE_FACEBOOK
            if (FB.IsInitialized)
            {
                if (callBack != null)
                {
                    callBackUnityLogin = callBack;
                }
                var perms = new List<string>() { "public_profile", "email" };
                FB.LogInWithReadPermissions(perms, AuthCallback);
            }
#endif
        }
#if USE_FACEBOOK
        private void AuthCallback(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log(aToken.UserId);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions)
                {
                    Debug.Log(perm);
                }
                if (result.ResultDictionary != null)
                {
                    foreach (string key in result.ResultDictionary.Keys)
                    {
                        Debug.Log(key + " : " + result.ResultDictionary[key].ToString());
                    }
                }
                if (callBackUnityLogin != null)
                {
                    callBackUnityLogin(FB.IsLoggedIn, result.ResultDictionary["access_token"].ToString());
                }
                // GetProfileInfo();
            }
            else
            {
                Debug.Log("User cancelled login");
                if (callBackUnityLogin != null)
                {
                    callBackUnityLogin(FB.IsLoggedIn, "");
                }
            }
        }
#endif

        private void GetProfileInfo()
        {
#if USE_FACEBOOK
            var profile = FB.Mobile.CurrentProfile();
            if (profile != null)
            {
            }
            else
            {
                Debug.LogError("Null");
            }
#endif
        }
        public void Logout()
        {
#if USE_FACEBOOK
            if (FB.IsInitialized && FB.IsLoggedIn)
                FB.LogOut();
#endif
        }
        public bool IsLogin()
        {
#if USE_FACEBOOK
            return FB.IsInitialized && FB.IsLoggedIn;
#endif
            return false;
        }
        public bool IsInitialized()
        {
#if USE_FACEBOOK
            return FB.IsInitialized;
#endif
            return false;
        }
        public string GetAccessToken()
        {
#if USE_FACEBOOK
            if (FB.IsInitialized && FB.IsLoggedIn)
            {
                string aToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
                return aToken;
            }
#endif
            return "";

        }
    }
}
