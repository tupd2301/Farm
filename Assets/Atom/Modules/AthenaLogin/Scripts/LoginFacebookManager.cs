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
        
        public string _fbToken { set; get; }
        private FacebookLogin _facebookLogin;

        public void LoginWithFacebook(string accessToken)
        {
            //don't create account because just wanna get data
            //PlayfabControllerInstance?.EventShowUILoading(true);
            _fbToken = accessToken;
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = false, AccessToken = _fbToken },
             OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
        }

        private void OnPlayfabFacebookAuthComplete(LoginResult result)
        {
            _entityId = result.EntityToken.Entity.Id;
            _entityType = result.EntityToken.Entity.Type;
            _isNewAccountCreated = result.NewlyCreated;
            _playfabUserId = result.PlayFabId;

            PlayerPrefs.SetInt(PlayerPrefKey.FacebookLinkedKey, 1);
            PlayerPrefs.SetString(PlayerPrefKey.PlayfabID, _playfabUserId);

            LoginSucessful("OnPlayfabLinkFacebookAuthComplete");
        }

        private void OnPlayfabFacebookAuthFailed(PlayFabError error)
        {
            //PlayfabControllerInstance?.EventShowUILoading(false);
            if (IsLoginWithPlayfab())
            {
                LinkPlayfabAndFacebook(_fbToken, false);
            }
            else
            {
                LoginWithDevices(() =>
                {
                    LinkPlayfabAndFacebook(_fbToken, false);
                });
            }
        }

        public void LinkPlayfabAndFacebook(string accessToken, bool forceLink = false)
        {
            _fbToken = accessToken;
            /*
            * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
            * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
            */
            //error 
            PlayFabClientAPI.LinkFacebookAccount(new LinkFacebookAccountRequest { AccessToken = accessToken, ForceLink = forceLink },
                OnPlayfabLinkFacebookAuthComplete, OnPlayfabLinkFacebookAuthFailed);

        }

        private void OnPlayfabLinkFacebookAuthComplete(LinkFacebookAccountResult result)
        {
            Debug.Log("OnPlayfabLinkFacebookAuthComplete");
            LoginSucessful("OnPlayfabLinkFacebookAuthComplete");
            _linkAccountUI?.ShowUI(_hasGoogleAccount, _hasAppleAccount, true);
            if (PlayerPrefs.GetInt(PlayerPrefKey.FacebookLinkedKey) <= 0)
            {
                PlayerPrefs.SetInt(PlayerPrefKey.FacebookLinkedKey, 1);
            }
            //PlayfabControllerInstance?.EventShowUILoading(false);
        }

        private void OnPlayfabLinkFacebookAuthFailed(PlayFabError error)
        {

            Debug.Log("OnPlayfabLinkFacebookAuthFailed");
            switch (error.Error)
            {
                case PlayFabErrorCode.LinkedAccountAlreadyClaimed:
                    ShowNoticePopupUI("You has already linked to another account. Please check again");
                    break;
            }
            //PlayfabControllerInstance?.EventShowUILoading(false);
        }

        //--------------------------------Login Interface

        public void LoginFacebookAndLink()
        {
            bool isForceLink = false;
            _facebookLogin = new FacebookLogin();
            _facebookLogin.Init(() =>
            {
                LoginFacebokNormal(true, isForceLink);
            });

        }

        public void AccountLinkFacebook()
        {
            bool isForceLink = false;
            _facebookLogin = new FacebookLogin();
            _facebookLogin.Init(() =>
            {
                LoginFacebokNormal(false, isForceLink);
            });

        }

        public void LoginFacebokNormal(bool loginAndLink, bool isForceLink)
        {
            Debug.Log("LoginFacebokNormal isForceLink = " + isForceLink);
            //EventShowUILoading(true);
            _facebookLogin.Login((result, accessToken) =>
            {
                if (result)
                {
                    PlayerPrefs.SetString(PlayerPrefKey.FBAccessToken, accessToken);
                    if (loginAndLink)
                    {
                        LoginWithFacebook(accessToken);
                    }
                    else
                    {
                        LinkPlayfabAndFacebook(accessToken, isForceLink);
                    }
                }
                else
                {
                    PlayerPrefs.SetInt(PlayerPrefKey.FacebookLinkedKey, 0);
                    //EventShowUILoading(false);
                }
            });
        }
    }
}