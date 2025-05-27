using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using Athena.Common.UI;
using UnityEngine.SceneManagement;

namespace Atom
{
    public class AppManager : SingletonMono<AppManager>
    {
        public GameObject loading;

        public void Start()
        {
            Application.targetFrameRate = 60;
            DontDestroyOnLoad(gameObject);

            GameModeContainer.Instance.InitGame();
            SceneManager.LoadScene("FishTank", LoadSceneMode.Single);
            /*
            GameObject audioManagerObject = GameObject.Instantiate(Resources.Load<GameObject>("AudioManager"));
            audioManagerObject.name = "AudioManager";

            ShowLoading(true);
            GameObject playfabManagerObject = GameObject.Instantiate(Resources.Load<GameObject>("SimpleTopDown/Manager/PlayfabManager"));
            playfabManagerObject.name = "PlayfabManager";
            */
        }

        public T ShowSafeTopUI<T>(string uiPath, bool overlay = false) where T : UIController
        {
            return ShowUI<T>(uiPath, overlay, 0);
        }

        public T ShowSafeOverlayUI<T>(string uiPath, bool overlay = false) where T : UIController
        {
            return ShowUI<T>(uiPath, overlay, 3);
        }

        public T ShowUI<T>(string uiPath, bool overlay = false, int layer = 1) where T : UIController
        {
            return Athena.Common.UI.UIManager.Instance.ShowUIOnTop<T>(uiPath, overlay, layer);
        }

        public T ShowUI<T>(string uiPath, int layer) where T : UIController
        {
            return Athena.Common.UI.UIManager.Instance.ShowUIOnTop<T>(uiPath, layer);
        }

        public void ShowLoading(bool show)
        {
            loading.SetActive(show);
        }

        public void PauseGame(bool pause)
        {
            if (pause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}