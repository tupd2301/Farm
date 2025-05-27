using UnityEngine;

namespace Factory
{
    public enum GameStateType
    {
        None,
        Main,
        Shop
    }

    public class GameState : MonoBehaviour
    {
        public static GameState Instance;
        private GameStateType _currentState = GameStateType.None;

        public GameStateType CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (_currentState != value)
                {
                    OnStateExit(_currentState);
                    _currentState = value;
                    OnStateEnter(_currentState);
                }
            }
        }

        void Awake()
        {
            Instance = this;
        }

        public void SetState(GameStateType newState)
        {
            CurrentState = newState;
        }

        private void OnStateEnter(GameStateType state)
        {
            switch (state)
            {
                case GameStateType.None:
                    // Hide all UI elements
                    if (GameManager.Instance.homeUI != null)
                    {
                        GameManager.Instance.homeUI.gameObject.SetActive(false);
                    }
                    break;

                case GameStateType.Main:
                    if (GameManager.Instance.homeUI != null)
                    {
                        GameManager.Instance.homeUI.gameObject.SetActive(true);
                        GameManager.Instance.homeUI.HideShopPopup();
                        BoxManager.Instance.isBoxClosing = false;
                        BoxManager.Instance.OpenGate();
                        GameManager.Instance.isStop = false;
                    }
                    break;

                case GameStateType.Shop:
                    if (GameManager.Instance.homeUI != null)
                    {
                        GameManager.Instance.homeUI.gameObject.SetActive(true);
                        GameManager.Instance.homeUI.ShowShopPopup();
                        GameManager.Instance.RandomGearsInShop();
                        BoxManager.Instance.isBoxClosing = true;
                    }
                    break;
            }
        }

        private void OnStateExit(GameStateType state)
        {
            switch (state)
            {
                case GameStateType.None:
                    break;

                case GameStateType.Main:
                    break;

                case GameStateType.Shop:
                    if (GameManager.Instance.homeUI != null)
                    {
                        GameManager.Instance.homeUI.HideShopPopup();
                    }
                    break;
            }
        }
    }
} 