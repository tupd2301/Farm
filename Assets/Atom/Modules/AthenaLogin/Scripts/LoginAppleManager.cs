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
        private string _appleIdToken;
        public void LinkPlayfabAndAppleId(string userId, bool forceLink = false)
        {

            Debug.Log("LinkPlayfabAndAppleId");
            _appleIdToken = userId;
            PlayFabClientAPI.LinkApple(new LinkAppleRequest { IdentityToken = userId, ForceLink = forceLink },
                OnPlayfabLinkAppleAuthComplete, OnPlayfabLinkAppleAuthFailed);
        }
        private void OnPlayfabLinkAppleAuthComplete(EmptyResult result)
        {
            Debug.Log("OnPlayfabLinkAppleAuthComplete");
            LoginSucessful("OnPlayfabLinkAppleAuthComplete");
            _linkAccountUI?.ShowUI(_hasGoogleAccount, true, _hasFacebookAccount);
            if (PlayerPrefs.GetInt(PlayerPrefKey.AppleLinkedKey) <= 0)
            {
                PlayerPrefs.SetInt(PlayerPrefKey.AppleLinkedKey, 1);
            }
            //PlayfabControllerInstance?.EventShowUILoading(false);
        }

        private void OnPlayfabLinkAppleAuthFailed(PlayFabError error)
        {

            Debug.Log("OnPlayfabLinkAppleAuthFailed");
            switch (error.Error)
            {
                case PlayFabErrorCode.LinkedAccountAlreadyClaimed:
                    //LoginWithAppleId(AppleIdToken);
                    ShowNoticePopupUI("You has already linked to another account. Please check again");
                    break;
                case PlayFabErrorCode.LinkedIdentifierAlreadyClaimed:
                    //LoginWithAppleId(AppleIdToken);
                    ShowNoticePopupUI("You has already linked to another account. Please check again");
                    break;
                default:
                    //LoginWithAppleId(AppleIdToken);
                    break;
            }
            //PlayfabControllerInstance?.EventShowUILoading(false);
        }

        public void LoginWithAppleId(string userId)
        {
            //don't create account because just wanna get data
            //PlayfabControllerInstance?.EventShowUILoading(true);
            _appleIdToken = userId;
            PlayFabClientAPI.LoginWithApple(new LoginWithAppleRequest { CreateAccount = false, IdentityToken = userId },
             OnPlayfabAppleAuthComplete, OnPlayfabLoginAppleAuthFailed);
        }

        private void OnPlayfabAppleAuthComplete(LoginResult result)
        {
            _entityId = result.EntityToken.Entity.Id;
            _entityType = result.EntityToken.Entity.Type;
            _isNewAccountCreated = result.NewlyCreated;
            _playfabUserId = result.PlayFabId;

            PlayerPrefs.SetInt(PlayerPrefKey.AppleLinkedKey, 1);
            PlayerPrefs.SetString(PlayerPrefKey.PlayfabID, _playfabUserId);
            LoginSucessful("OnPlayfabLinkAppleAuthComplete");
        }

        private void OnPlayfabLoginAppleAuthFailed(PlayFabError error)
        {
            //PlayfabControllerInstance?.EventShowUILoading(false);
            if (IsLoginWithPlayfab())
            {
                LinkPlayfabAndAppleId(_appleIdToken);
            }
            else
            {
                LoginWithDevices(() =>
                {
                    LinkPlayfabAndAppleId(_appleIdToken);
                });
            }
        }

        //--------------------------------Login Interface

        private AppleLogin _appleLogin;
        public void LoginAppleAndLink()
        {
            LoginAppleNormal(true);
        }

        public void AccountLinkApple()
        {
            LoginAppleNormal(false);
        }

        public void LoginAppleNormal(bool loginAndLink)
        {
#if UNITY_IOS
#if USE_APPLE_AUTHENTICATION
            //EventShowUILoading(true);
            _appleLogin = new AppleLogin();
            _appleLogin.Init();
            _appleLogin.SignInWithApple((idToken) =>
            {
                Debug.Log("SignInWithApple callback");
                Debug.Log("idToken " + idToken);
                if (idToken != null)
                {
                    PlayerPrefs.SetString(PlayerPrefKey.AppleIdToken, idToken);
                    if (loginAndLink)
                    {
                        LoginWithAppleId(idToken);
                    }
                    else
                    {
                        LinkPlayfabAndAppleId(idToken);
                    }
                }
            });
#endif
#endif
        }
    }
}