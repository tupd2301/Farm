using UnityEngine;
#if USE_APPLE_AUTHENTICATION
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
#endif
using UnityEngine.Events;
using System.Text;
namespace StarShipFramework.SaveProgress
{
    public class AppleLogin
    {
        private const string AppleUserIdKey = "AppleUserId";
#if USE_APPLE_AUTHENTICATION
        private IAppleAuthManager _appleAuthManager;

        // Start is called before the first frame update
        public void Init()
        {
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                this._appleAuthManager = new AppleAuthManager(deserializer);
            }

            // this.InitializeLoginMenu();// need to check later
        }
        private void InitializeLoginMenu()
        {
            // Check if the current platform supports Sign In With Apple
            if (this._appleAuthManager == null)
            {
                // Debug.LogError("Unsupported Platform!");
                return;
            }

            // If at any point we receive a credentials revoked notification, we delete the stored User ID, and go back to login
            this._appleAuthManager.SetCredentialsRevokedCallback(result =>
            {
                // Debug.Log("Received revoked callback " + result);
                // Debug.LogError("Ok, then setup button login with apple normal");
                PlayerPrefs.DeleteKey(AppleUserIdKey);
            });

            // If we have an Apple User Id available, get the credential status for it
            if (PlayerPrefs.HasKey(AppleUserIdKey))
            {
                var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);
                // Debug.LogError("Loading---Checking Apple Credentials");
                this.CheckCredentialStatusForUserId(storedAppleUserId);
            }
            // If we do not have an stored Apple User Id, attempt a quick login
            else
            {
                // Debug.LogError("Ok, then setup button login with apple normal - quick");
                this.AttemptQuickLogin();
            }
        }
        private void AttemptQuickLogin()
        {
            if (this._appleAuthManager == null)
            {
                // Debug.LogError("Unsupported Platform!");
                return;
            }
            
            var quickLoginArgs = new AppleAuthQuickLoginArgs();

            // Quick login should succeed if the credential was authorized before and not revoked
            this._appleAuthManager.QuickLogin(
                quickLoginArgs,
                credential =>
                {
                    // If it's an Apple credential, save the user ID, for later logins
                    var appleIdCredential = credential as IAppleIDCredential;
                    if (appleIdCredential != null)
                    {
                        PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                    }
                    // Debug.LogError("Success-Login playfab with apple id here");
                },
                error =>
                {
                    // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                    // var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    // Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    // Debug.LogError("Ok, then setup button login with apple normal");
                });
        }

        private void CheckCredentialStatusForUserId(string appleUserId)
        {
            if (this._appleAuthManager == null)
            {
                // Debug.LogError("Unsupported Platform!");
                return;
            }
            // If there is an apple ID available, we should check the credential state
            this._appleAuthManager.GetCredentialState(
                appleUserId,
                state =>
                {
                    switch (state)
                    {
                        // If it's authorized, login with that user id
                        case CredentialState.Authorized:
                            // Debug.LogError("first time then link to apple id");
                            // Debug.LogError("second time then show logout button");
                            return;
                        // If it was revoked, or not found, we need a new sign in with apple attempt
                        // Discard previous apple user id
                        case CredentialState.Revoked:
                        case CredentialState.NotFound:
                            // Debug.LogError("Ok, then setup button login with apple normal");
                            PlayerPrefs.DeleteKey(AppleUserIdKey);
                            return;
                    }
                },
                error =>
                {
                    // var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    // Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
                    // Debug.LogError("Ok, then setup button login with apple normal");
                });
        }
        public void SignInWithApple(UnityAction<string> callBack = null)
        {
            if (this._appleAuthManager == null)
            {
                // Debug.LogError("Unsupported Platform!");
                return;
            }
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            this._appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                    PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                    if (callBack != null)
                    {
                        callBack(GetAppleIdentityToken(credential.User, credential));
                    }
                    // Debug.LogError("Success-Login playfab with apple id here");
                },
                error =>
                {
                    // var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    // Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                    // Debug.LogError("Ok, then setup button login with apple normal");
                });
        }
        // Update is called once per frame
        public void Update()
        {
            if (this._appleAuthManager != null)
            {
                this._appleAuthManager.Update();
            }
        }

        //-------
        public string GetAppleIdentityToken(string appleUserId, ICredential receivedCredential)
        {
            
            if (receivedCredential == null)
            {
                return null;
            }

            var appleIdCredential = receivedCredential as IAppleIDCredential;
            var passwordCredential = receivedCredential as IPasswordCredential;
            if (appleIdCredential != null)
            {
                if (appleIdCredential.IdentityToken != null)
                {
                    var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);
                    return identityToken;
                }
            }
            else if (passwordCredential != null)
            {
            }
            else
            {
            }
            return null;
        }
#endif
    }
}
