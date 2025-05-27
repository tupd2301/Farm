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
        #if USE_GOOGLE_SIGN_IN
        private string _webClientId = "572316904054-h8o2547chouk0g7efn90e60p38144gc2.apps.googleusercontent.com";
        private string _serverAuthCode = "";

        private GoogleSignInConfiguration configuration;
        #endif
        // Defer the configuration creation until Awake so the web Client ID
        // Can be set via the property inspector in the Editor.
        private void InitGoogle()
        {
            #if USE_GOOGLE_SIGN_IN
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = _webClientId,
                RequestIdToken = true
            };
            #endif
        }

        private bool _isLogin;
        public void LoginGoogleAndLinkNormal(bool isLogin)
        {
            #if USE_GOOGLE_SIGN_IN
            _isLogin = isLogin;
            if (configuration == null)
            {
                InitGoogle();
            }

            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestAuthCode = true;
            Debug.Log("Calling SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
            #endif
        }

        public void OnSignOut()
        {
            #if USE_GOOGLE_SIGN_IN
            Debug.Log("Calling SignOut");
            GoogleSignIn.DefaultInstance.SignOut();
            #endif
        }

        public void OnDisconnect()
        {
            #if USE_GOOGLE_SIGN_IN
            Debug.Log("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
            #endif
        }

        #if USE_GOOGLE_SIGN_IN
        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using (IEnumerator<System.Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error =
                                (GoogleSignIn.SignInException)enumerator.Current;
                        Debug.Log("Got Error: " + error.Status + " " + error.Message);
                    }
                    else
                    {
                        Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                    }
                }
            }
            else if (task.IsCanceled)
            {
                Debug.Log("Canceled");
            }
            else
            {
                Debug.Log("Welcome: " + task.Result.DisplayName + "!");
                if (_isLogin)
                {
                    LoginWithGoogle(task.Result.AuthCode);
                }
                else
                {
                    LinkPlayfabAndGoogle(task.Result.AuthCode);
                }
            }
        }

        public void OnSignInSilently()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            Debug.Log("Calling SignIn Silently");

            GoogleSignIn.DefaultInstance.SignInSilently()
                  .ContinueWith(OnAuthenticationFinished);
        }


        public void OnGamesSignIn()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = true;
            GoogleSignIn.Configuration.RequestIdToken = false;

            Debug.Log("Calling Games SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
        }


        public void LinkPlayfabAndGoogle(string serverAuthCode, bool forceLink = false)
        {
            _serverAuthCode = serverAuthCode;
            Debug.Log(serverAuthCode);
            PlayFabClientAPI.LinkGoogleAccount(new LinkGoogleAccountRequest { ServerAuthCode = serverAuthCode, ForceLink = forceLink },
                OnPlayfabLinkGoogleAuthComplete, OnPlayfabLinkGoogleAuthFailed);
        }

        private void OnPlayfabLinkGoogleAuthComplete(LinkGoogleAccountResult result)
        {
            Debug.Log("OnPlayfabLinkGoogleAuthComplete");
            LoginSucessful("OnPlayfabLinkGoogleAuthComplete");
            _linkAccountUI?.ShowUI(true, _hasAppleAccount, _hasFacebookAccount);
        }

        private void OnPlayfabLinkGoogleAuthFailed(PlayFabError error)
        {
            UnityEngine.Debug.Log("alo : " + error.Error.ToString());
            Debug.Log(error);
            switch (error.Error)
            {
                case PlayFabErrorCode.AccountAlreadyLinked:
                case PlayFabErrorCode.LinkedAccountAlreadyClaimed:
                    ShowNoticePopupUI("You has already linked to another account. Please check again");
                    break;
            }
        }

        public void LoginWithGoogle(string serverAuthCode)
        {
            //PlayfabControllerInstance?.EventShowUILoading(true);
            _serverAuthCode = serverAuthCode;
            UnityEngine.Debug.Log("server auth code : " + _serverAuthCode);
            PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest { CreateAccount = false, ServerAuthCode = _serverAuthCode },
             OnPlayfabGoogleAuthComplete, OnPlayfabGoogleAuthFailed);
        }

        private void OnPlayfabGoogleAuthComplete(LoginResult result)
        {
            _entityId = result.EntityToken.Entity.Id;
            _entityType = result.EntityToken.Entity.Type;
            _isNewAccountCreated = result.NewlyCreated;
            _playfabUserId = result.PlayFabId;

            UnityEngine.Debug.Log("login google sucess");
            PlayerPrefs.SetString(PlayerPrefKey.PlayfabID, result.PlayFabId);
            LoginSucessful("OnPlayfabLinkGoogleAuthComplete");
        }

        private void OnPlayfabGoogleAuthFailed(PlayFabError error)
        {
            UnityEngine.Debug.Log("login google fail : " + error.Error.ToString());
            if (IsLoginWithPlayfab())
            {
                LoginGoogleAndLinkNormal(false);
            }
            else
            {
                LoginWithDevices(() =>
                {
                    LoginGoogleAndLinkNormal(false);
                });
            }
        }
        #endif
        //--------------------------------Login Interface

        public void LoginGoogleAndLink()
        {
            LoginGoogleAndLinkNormal(true);
        }
        public void AccountLinkGoogle()
        {
            LoginGoogleAndLinkNormal(false);
        }
    }
}