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
    public enum LoginType
    {
        Facebook,
        Apple
    }

    public static class PlayerPrefKey
    {
        //playfab
        public const string DeviceIDKey = "DeviceIDKey";
        public const string AppleIdToken = "AppleIdToken";
        public const string FBAccessToken = "FBAccessToken";
        public const string FacebookLinkedKey = "FacebookLinkedKey";
        public const string AppleLinkedKey = "AppleLinkedKey";
        public const string PlayfabID = "PlayfabID";

        public const string CustomIDSignIn = "CustomIDSignIn";

        public const string SecondTimeOpenApp = "SecondTimeOpenApp";
        public const string OneSDKButtonPositionX = "OneSDKButtonPositionX";
        public const string OneSDKButtonPositionY = "OneSDKButtonPositionY";
    }

    public partial class LoginManager : SingletonMono<LoginManager>
    {
        //--------------------------------Menu

        #region login popup
        private LoginUI _LoginUI;
        private RegisterUI _registerUI;
        private EmailLoginUI _loginPopupUI;
        private NoticeUI _noticePopupUI;
        private ConfirmUI _confirmPopupUI;

#if UNITY_IOS
        public void Update()
        {
#if USE_APPLE_AUTHENTICATION
            if (_appleLogin != null)
            {
                _appleLogin.Update();
            }
#endif
        }
#endif

        public void ShowNoticePopupUI(string message)
        {
            _noticePopupUI = LoginAppManager.Instance.ShowUI<NoticeUI>("NoticeUI", false, 3);
            _noticePopupUI.OnGoBack = () => { LoginAppManager.Instance.ReleaseUI(_noticePopupUI.gameObject); };
            _noticePopupUI.SetMessageText(message);
        }

        public void ShowNoticePopupUICallback(string message, System.Action callback)
        {
            _noticePopupUI = LoginAppManager.Instance.ShowUI<NoticeUI>("NoticeUI", false, 3);
            _noticePopupUI.OnGoBack = () => { LoginAppManager.Instance.ReleaseUI(_noticePopupUI.gameObject); callback?.Invoke(); };
            _noticePopupUI.SetMessageText(message);
        }

        public void ShowConfirmPopupUI(string message, System.Action okCallback, System.Action cancelCallback)
        {
            _confirmPopupUI = LoginAppManager.Instance.ShowUI<ConfirmUI>("ConfirmUI", false, 3);
            _confirmPopupUI.OnOK = () => { LoginAppManager.Instance.ReleaseUI(_confirmPopupUI.gameObject); okCallback?.Invoke(); };
            _confirmPopupUI.OnCancel = () => { LoginAppManager.Instance.ReleaseUI(_confirmPopupUI.gameObject); cancelCallback?.Invoke(); };
            _confirmPopupUI.SetMessageText(message);
        }

        public void ShowLoginNoticePopupUI(string message)
        {
            //_loginPopupUI?.SetErrorText(message);
            ShowNoticePopupUI(message);
        }
        public void ShowRegisterNoticePopupUI(string message, bool isShowTipError)
        {
            //if (isShowTipError)
            //{
            //    _registerUI.SetErrorText(message);
            //}
            ShowNoticePopupUI(message);
        }

        public void ShowLoginUI()
        {
            _LoginUI = LoginAppManager.Instance.ShowUI<LoginUI>("LoginUI", false, 0);
            _LoginUI.ShowUI();
            _LoginUI.OnShowLoginPopupUI = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { ShowLoginPopup(); } }; //ShowLoginPopup;

            _LoginUI.OnLoginGuest = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { LoginGuest(); } };// LoginGuest;
            _LoginUI.OnLoginFacebook = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { LoginFacebookAndLink(); } };//LoginFacebookAndLink;
            _LoginUI.OnLoginAppleId = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { LoginAppleAndLink(); } };//LoginAppleAndLink;
            _LoginUI.OnLoginGoogle = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { LoginGoogleAndLink(); } };//LoginGoogleAndLink;
        }

        public void ShowLoginPopup()
        {
            _loginPopupUI = LoginAppManager.Instance.ShowUI<EmailLoginUI>("EmailLoginUI", false, 1);
            _loginPopupUI.OnGoBack = () => { LoginAppManager.Instance.ReleaseUI(_loginPopupUI.gameObject); };
            _loginPopupUI.OnShowRegisterUI = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { ShowRegisterPopup(); } };//ShowRegisterPopup;
            _loginPopupUI.OnLogin = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { LoginPlayfab(); } };//LoginPlayfab;
            _loginPopupUI.OnForgotPassword = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { ShowForgotPasswordUI(); } };//ShowForgotPasswordUI;
            _loginPopupUI.SetErrorText(string.Empty);
        }

        public void ShowRegisterPopup()
        {
            LoginUIManager.Instance.ShowLoading(true);
            if (IsLoginWithPlayfab())
            {
                LoginUIManager.Instance.ShowLoading(false);
                DisplayRegisterPopup("REGISTER", "REGISTER");
            }
            else
            {
                LoginWithDevices(() =>
                {
                    LoginUIManager.Instance.ShowLoading(false);
                    DisplayRegisterPopup("REGISTER", "REGISTER");
                });
            }
        }

        public void ShowUpdateInfoPopup()
        {
            if (IsLoginWithPlayfab())
            {
                DisplayRegisterPopup("UPDATE INFO", "CONFIRM");
            }
            else
            {
                LoginWithDevices(() =>
                {
                    DisplayRegisterPopup("UPDATE INFO", "CONFIRM");
                });
            }
        }

        private void DisplayRegisterPopup(string headerText, string buttonText)
        {
            _registerUI = LoginAppManager.Instance.ShowUI<RegisterUI>("RegisterUI", false, 1);
            _registerUI.OnGoBack = () => { LoginAppManager.Instance.ReleaseUI(_registerUI.gameObject); };
            _registerUI.OnRegister = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { RegisterPlayFab(); } }; //RegisterPlayFab;
            _registerUI.OnPasswordClick = PasswordClick;
            _registerUI.OnEmailClick = EmailClick;
            _registerUI.SetErrorText(string.Empty);
            _registerUI.SetHeaderText(headerText);
            _registerUI.SetButtonText(buttonText);
        }
        #endregion

        #region one id
        private MainOneSDKUI _mainOneSDKUI;
        private GuestAccountUI _guestAccountUI;
        private ChangePasswordUI _changePasswordUI;

        private LinkAccountUI _linkAccountUI;
        public void ShowMainOneSDKUI()
        {
            if (_mainOneSDKUI == null)
            {
                _mainOneSDKUI = LoginAppManager.Instance.ShowUI<MainOneSDKUI>("MainOneSDKUI", false, 0);
                _mainOneSDKUI.OnShowGuestAccountUI = ShowGuestAccountUI;
            }
        }

        public void ShowGuestAccountUI()
        {
            _guestAccountUI = LoginAppManager.Instance.ShowUI<GuestAccountUI>("GuestAccountUI", false, 1);
            _guestAccountUI.ShowUI(_hasEmailAccount, _nameAccount, _emailAccount);
            _guestAccountUI.OnGoBack = () => { LoginAppManager.Instance.ReleaseUI(_guestAccountUI.gameObject); };
            _guestAccountUI.OnShowUpdateInfoUI = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { ShowUpdateInfoPopup(); } }; //ShowUpdateInfoPopup;
            _guestAccountUI.OnShowChangePasswordUI = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { ShowChangePasswordUI(); } }; //ShowChangePasswordUI;
            _guestAccountUI.OnShowLinkAccountUI = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { ShowLinkAccountUI(); } }; //ShowLinkAccountUI;
            _guestAccountUI.OnSwitchAccount = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { SignOut(); } }; //SignOut;
        }

        public void ShowChangePasswordUI()
        {
            _changePasswordUI = LoginAppManager.Instance.ShowUI<ChangePasswordUI>("ChangePasswordUI", false, 1);
            _changePasswordUI.OnGoBack = () => { LoginAppManager.Instance.ReleaseUI(_changePasswordUI.gameObject); };
            _changePasswordUI.OnConfirm = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { SendAccountRecoveryEmail(true); } }; //SendAccountRecoveryEmail;
            _changePasswordUI.ShowEmailAccount(_emailAccount, false);
        }

        public void ShowForgotPasswordUI()
        {
            _changePasswordUI = LoginAppManager.Instance.ShowUI<ChangePasswordUI>("ChangePasswordUI", false, 1);
            _changePasswordUI.OnGoBack = () => { LoginAppManager.Instance.ReleaseUI(_changePasswordUI.gameObject); };
            _changePasswordUI.OnConfirm = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { SendAccountRecoveryEmail(false); } }; //SendAccountRecoveryEmail;
            _changePasswordUI.ShowEmailAccount(string.Empty, true);
        }

        public void ShowLinkAccountUI()
        {
            _linkAccountUI = LoginAppManager.Instance.ShowUI<LinkAccountUI>("LinkAccountUI", false, 1);
            _linkAccountUI.ShowUI(_hasGoogleAccount, _hasAppleAccount, _hasFacebookAccount);
            _linkAccountUI.OnGoBack = () => { LoginAppManager.Instance.ReleaseUI(_linkAccountUI.gameObject); };
            _linkAccountUI.OnLinkFacebook = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { AccountLinkFacebook(); } }; //AccountLinkFacebook;
            _linkAccountUI.OnLinkAppleId = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { AccountLinkApple(); } }; //AccountLinkApple;
            _linkAccountUI.OnLinkGoogle = () => { if (Application.internetReachability == NetworkReachability.NotReachable) { ShowNoticePopupUI("Server connection failed. Please check your network"); } else { AccountLinkGoogle(); } }; //AccountLinkGoogle;
        }
        #endregion

        //--------------------------------Login

        #region user account info
        private bool _isNewAccountCreated;
        private string _playfabUserId;
        private string _entityId;
        private string _entityType;

        public bool _hasEmailAccount;
        public bool _hasFacebookAccount;
        public bool _hasGoogleAccount;
        public bool _hasAppleAccount;

        public string _nameAccount;
        public string _emailAccount;

        private bool _hidePassword;

        public bool HidePassword
        {
            get
            {
                return _hidePassword;
            }
            set
            {
                _hidePassword = value;
            }
        }

        public void SignOut()
        {
            if (_nameAccount == "Guest" && !_hasAppleAccount)
            {
                ShowConfirmPopupUI("Please link your account or update your information. If you don't take action, your account information won't be saved", () =>
                {
                    ActionSignOut();
                }, () =>
                {

                });
            }
            else
            {
                ActionSignOut();
            }
        }

        private void ActionSignOut()
        {
            PlayerPrefs.SetString(PlayerPrefKey.CustomIDSignIn, string.Empty);
            PlayFabClientAPI.ForgetAllCredentials();
            _emailAccount = string.Empty;
            _nameAccount = string.Empty;

            if (_mainOneSDKUI != null)
            {
                LoginAppManager.Instance.ReleaseUI(_mainOneSDKUI.gameObject);
            }
            if (_linkAccountUI != null)
            {
                LoginAppManager.Instance.ReleaseUI(_linkAccountUI.gameObject);
            }
            if (_guestAccountUI != null)
            {
                LoginAppManager.Instance.ReleaseUI(_guestAccountUI.gameObject);
            }

            LoginManager.Instance.ShowLoginUI();
        }

        public void LoginGuest()
        {
            if (IsLoginWithPlayfab())
            {
                _nameAccount = "Guest";
                LoginSucessful("Guest");
            }
            else
            {
                LoginUIManager.Instance.ShowLoading(true);
                LoginWithDevices(() =>
                {
                    LoginUIManager.Instance.ShowLoading(false);
                    _nameAccount = "Guest";
                    LoginSucessful("Guest");
                });
            }
        }

        public void LoginSucessful(string textLogin)
        {
            ShowMainOneSDKUI();
            if (_loginPopupUI != null)
            {
                LoginAppManager.Instance.ReleaseUI(_loginPopupUI.gameObject);
            }
            if (_LoginUI != null)
            {
                LoginAppManager.Instance.ReleaseUI(_LoginUI.gameObject);
            }
            if (_registerUI != null)
            {
                LoginAppManager.Instance.ReleaseUI(_registerUI.gameObject);
            }
            GetAccountInfo();
        }

        public void GetAccountInfo()
        {
            //LoginUIManager.Instance.ShowLoading(true);
            var requestAccountInfo = new GetAccountInfoRequest { PlayFabId = PlayerPrefs.GetString(PlayerPrefKey.PlayfabID) };
            PlayFabClientAPI.GetAccountInfo(requestAccountInfo, OnPlayfabRequestAccountComplete, OnPlayfabRequestAccountFailed);
        }

        private void OnPlayfabRequestAccountComplete(GetAccountInfoResult result)
        {
            //LoginUIManager.Instance.ShowLoading(false);
            if (result != null && result.AccountInfo != null)
            {
                UnityEngine.Debug.Log(result.AccountInfo.ToJson());
                _hasEmailAccount = (result.AccountInfo.PrivateInfo != null && !string.IsNullOrEmpty(result.AccountInfo.PrivateInfo.Email));
                _hasFacebookAccount = (result.AccountInfo.FacebookInfo != null);
                _hasGoogleAccount = (result.AccountInfo.GoogleInfo != null);
                _hasAppleAccount = (result.AccountInfo.AppleAccountInfo != null);
                _emailAccount = _hasEmailAccount ? result.AccountInfo.PrivateInfo.Email : string.Empty;
                if (result != null && result.AccountInfo != null)
                {
                    if (result.AccountInfo.Username != null)
                    {
                        _nameAccount = result.AccountInfo.Username;
                    }
                    else if (result.AccountInfo.FacebookInfo != null && result.AccountInfo.FacebookInfo.FullName != null)
                    {
                        _nameAccount = result.AccountInfo.FacebookInfo.FullName;
                    }
                    else if (result.AccountInfo.GoogleInfo != null && result.AccountInfo.GoogleInfo.GoogleName != null)
                    {
                        _nameAccount = result.AccountInfo.GoogleInfo.GoogleName;
                    }
                    else
                    {
                        _nameAccount = "Guest";
                    }
                }

                if (result.AccountInfo.CustomIdInfo != null && !string.IsNullOrEmpty(result.AccountInfo.CustomIdInfo.CustomId))
                {
                    PlayerPrefs.SetString(PlayerPrefKey.CustomIDSignIn, result.AccountInfo.CustomIdInfo.CustomId);
                }

                if (_guestAccountUI != null)
                {
                    _guestAccountUI.ShowUI(_hasEmailAccount, _nameAccount, _emailAccount);
                }
            }
        }

        private void OnPlayfabRequestAccountFailed(PlayFabError error)
        {
            //LoginUIManager.Instance.ShowLoading(false);
            switch (error.Error)
            {
                case PlayFabErrorCode.AccountAlreadyLinked:
                    //Check info User
                    break;
            }
        }

        public bool IsLoginWithPlayfab()
        {
            return PlayFabClientAPI.IsClientLoggedIn();
        }
        #endregion
    }
}