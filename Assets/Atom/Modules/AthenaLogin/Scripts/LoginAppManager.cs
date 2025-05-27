using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

namespace OneID
{
    public class LoginAppManager : SingletonMono<LoginAppManager>
    {
        [SerializeField]
        private bool _landscapeMode;
        public bool LanscapeMode
        {
            get
            {
                return _landscapeMode;
            }
        }

        public void Start()
        {
            Application.targetFrameRate = 60;
            SetLandscape(_landscapeMode);
            if (PlayerPrefs.GetInt(PlayerPrefKey.SecondTimeOpenApp) > 0)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefKey.CustomIDSignIn)))
                {
                    LoginManager.Instance.LoginWithDevices(() =>
                    {
                        LoginManager.Instance.ShowMainOneSDKUI();
                        LoginManager.Instance.GetAccountInfo();
                    });
                }
                else
                {
                    LoginManager.Instance.ShowLoginUI();
                }
            }
            else
            {
                LoginManager.Instance.LoginWithDevices(() =>
                    {
                        PlayerPrefs.SetInt(PlayerPrefKey.SecondTimeOpenApp, 1);
                        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefKey.CustomIDSignIn)))
                        {
                            LoginManager.Instance.ShowMainOneSDKUI();
                            LoginManager.Instance.GetAccountInfo();
                        }
                        else
                        {
                            LoginManager.Instance.ShowLoginUI();
                        }
                    }
                );
            }
            
        }

        public void SetLandscape(bool isLandscape)
        {
            LoginUIManager.Instance.SetCanvas(isLandscape);
        }

        public T ShowSafeTopUI<T>(string uiPath, bool overlay = false) where T : MonoBehaviour
        {
            return ShowUI<T>(uiPath, overlay, 0);
        }

        public T ShowSafeOverlayUI<T>(string uiPath, bool overlay = false) where T : MonoBehaviour
        {
            return ShowUI<T>(uiPath, overlay, 3);
        }

        public T ShowUI<T>(string uiPath, bool overlay = false, int layer = 1) where T : MonoBehaviour
        {
            string folder = _landscapeMode ? "UILandscapePrefabs" : "UIPrefabs";
            GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(string.Format(folder + "/{0}", uiPath)));
            LoginUIManager.Instance.ShowUI(obj, layer);
            return obj.GetComponent<T>();
        }

        public void ReleaseUI(GameObject uiGameObject)
        {
            GameObject.Destroy(uiGameObject);
        }
    }
}