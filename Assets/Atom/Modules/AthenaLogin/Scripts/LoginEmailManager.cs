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
        #region send email
        public void SendAccountRecoveryEmail(bool isChangePassword)
        {
            UnityEngine.Debug.Log(_changePasswordUI.TextEmail);
            if (string.IsNullOrEmpty(_changePasswordUI.TextEmail))
            {
                ShowNoticePopupUI("Please fill your email to reset password");
            }
            else
            {
                LoginUIManager.Instance.ShowLoading(true);
                PlayFabClientAPI.SendAccountRecoveryEmail(new SendAccountRecoveryEmailRequest { TitleId = "F7E64", Email = _changePasswordUI.TextEmail, EmailTemplateId = isChangePassword ? "6D014909A8E7162E" : "738BA230435131F1" }, OnSendAccountRecoveryEmailSuccess, OnSendAccountRecoveryEmailFailure);
            }
        }

        private void OnSendAccountRecoveryEmailSuccess(SendAccountRecoveryEmailResult result)
        {
            LoginUIManager.Instance.ShowLoading(false);
            if (_guestAccountUI != null)
            {
                ShowNoticePopupUICallback("Please check your email to change password", () => { if (_changePasswordUI != null) LoginAppManager.Instance.ReleaseUI(_changePasswordUI.gameObject); });
            }
            else
            {
                ShowNoticePopupUICallback("New password has been sent to your email. Please check it", () => { if (_changePasswordUI != null) LoginAppManager.Instance.ReleaseUI(_changePasswordUI.gameObject); });
            }
        }

        private void OnSendAccountRecoveryEmailFailure(PlayFabError error)
        {
            LoginUIManager.Instance.ShowLoading(false);
            string errorReport = error.GenerateErrorReport();
            UnityEngine.Debug.Log(errorReport);
            string displayError = string.Empty;
            if (errorReport.Contains("Email address is not valid."))
            {
                displayError = "Email is wrong format. Please check again";
            }
            else if (errorReport.Contains("user not found") || errorReport.Contains("Invalid email address"))
            {
                displayError = "This email hasn't been linked. Please contact with us to support";
            }
            else if (errorReport.Contains("EmailDenyList was denied by PlayFab for previously bouncing"))
            {
                displayError = "Your email has been banned. Please fill a other email";
            }
            else
            {
                displayError = errorReport;
            }
            ShowLoginNoticePopupUI(displayError);
        }
        #endregion

        #region login email

        private string _username = "element";
        private string _userEmail = "element@gmail.com";
        private string _userPassword = "@Admin123";
        private string _playFabId;

        public void LoginPlayfab()
        {
            var request = new LoginWithPlayFabRequest
            { Username = _loginPopupUI.GetEmail(), Password = _loginPopupUI.GetPassword() };
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginCustomSuccess, OnLoginCustomFailure);
        }

        public void LoginPlayfabWithEmail()
        {
            var request = new LoginWithEmailAddressRequest
            { Email = _loginPopupUI.GetEmail(), Password = _loginPopupUI.GetPassword() };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginCustomSuccess, OnLoginCustomFailure);
        }

        public void LoginWithCustomID()
        {
            var request = new LoginWithCustomIDRequest
            { };
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginCustomSuccess, OnLoginCustomFailure);
        }

        private void OnLoginCustomSuccess(LoginResult result)
        {
            PlayerPrefs.SetString(PlayerPrefKey.PlayfabID, result.PlayFabId);
            Debug.Log("Congratulations, On Login Email Success");
            LoginSucessful("Congratulations, On Login Email Success");
            _playFabId = result.PlayFabId;
        }

        private void OnLoginCustomFailure(PlayFabError error)
        {
            _loginPopupUI?.SetErrorText("ID or Password incorrect");
        }

        private void GetRegisterInfo()
        {
            if (_registerUI != null)
            {
                _username = _registerUI.GetID();
                _userPassword = _registerUI.GetPass();
                _userEmail = _registerUI.GetEmail();
            }

            Debug.Log("id:" + _username);
            Debug.Log("pass:" + _userPassword);
            Debug.Log("email:" + _userEmail);
        }

        private bool HasSpecialChars(string yourString)
        {
            for (int i = 0; i < yourString.Length; i++)
            {
                char ch = yourString[i];
                if (!char.IsLetterOrDigit(ch))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasUpperChars(string yourString)
        {
            for (int i = 0; i < yourString.Length; i++)
            {
                char ch = yourString[i];
                if (char.IsUpper(ch))
                {
                    return true;
                }
            }
            return false;
        }

        public void RegisterPlayFab()
        {
            GetRegisterInfo();
            if (string.IsNullOrEmpty(_username))
            {
                ShowRegisterNoticePopupUI("Please fill in your information", false);
            }
            else if (_username.Length < 6)
            {
                ShowRegisterNoticePopupUI("ID too short", false);
            }
            else if (_username.Length > 15 || _username.Length < 6)
            {
                ShowRegisterNoticePopupUI("ID only from 6 - 15 letters", false);
            }
            else if (string.IsNullOrEmpty(_userPassword) || string.IsNullOrEmpty(_registerUI.GetRepass()))
            {
                ShowRegisterNoticePopupUI("Please fill in your information", false);
            }
            else if (_registerUI.GetPass() != _registerUI.GetRepass())
            {
                ShowRegisterNoticePopupUI("Passwords do not match", false);
            }
            else if (_userPassword.Length < 6)
            {
                ShowRegisterNoticePopupUI("Password is too short", false);
            }
            else if (_userPassword.Length > 15)
            {
                ShowRegisterNoticePopupUI("Password only from 6 - 15 letters", false);
            }
            else if (!HasSpecialChars(_userPassword) || !HasUpperChars(_userPassword))
            {
                ShowRegisterNoticePopupUI("Password contain at least 1 upper case letter and 1 special letter", false);
            }
            else if (string.IsNullOrEmpty(_userEmail))
            {
                ShowRegisterNoticePopupUI("Please fill in your information", false);
            }
            else
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    ShowNoticePopupUI("Server connection failed. Please check your network");
                }
                else
                {
                    LoginUIManager.Instance.ShowLoading(true);
                    PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest
                    { Email = _userEmail, Password = _userPassword, Username = _username },
                    OnRegisterSuccess,
                    OnRegisterFailure);
                }
            }

        }

        private void OnRegisterSuccess(AddUsernamePasswordResult result)
        {
            LoginUIManager.Instance.ShowLoading(false);
            if (_guestAccountUI != null)
            {
                ShowNoticePopupUICallback("Congratulation, your account has been updated sucessfully", () => { LoginSucessful("OnRegisterSuccessComplete"); });
            }
            else
            {
                LoginSucessful("OnRegisterSuccessComplete");
            }
            Debug.Log("Register success");
        }

        private void OnRegisterFailure(PlayFabError error)
        {
            LoginUIManager.Instance.ShowLoading(false);
            string reportError = error.GenerateErrorReport();
            string displayError = string.Empty;
            if (reportError.Contains("Email address is not valid"))
            {
                displayError = "Email is wrong format. Please check again";
            }
            else if (reportError.Contains("User name already exists."))
            {
                displayError = "ID already exists";
            }
            else if (reportError.Contains("Email address already exists."))
            {
                displayError = "Email already exists. Please fill in a other email";
            }
            else if (reportError.Contains("Username contains invalid characters"))
            {
                displayError = "ID is not consist of special character";
            }
            ShowRegisterNoticePopupUI(displayError, false);
            Debug.Log("Register failure");
            Debug.LogError(error.GenerateErrorReport());
        }

        public void UpdateUserData(string keyData, string valueData)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                    {
                        { keyData, valueData },
                    }
            },
                result => Debug.Log("Successfully updated user data"),
                error =>
                {
                    Debug.Log("Got error setting user data Ancestor to Arthur");
                    Debug.Log(error.GenerateErrorReport());
                });
        }

        public void PasswordClick()
        {
            _registerUI.SetErrorText("Password must contain at least 1 upper case letter and 1 special letter");
        }

        public void EmailClick()
        {
            _registerUI.SetEmailErrorText("Please fill an email that you're using. It will assist you in resetting or changing your password");
        }

        #endregion
    }
}