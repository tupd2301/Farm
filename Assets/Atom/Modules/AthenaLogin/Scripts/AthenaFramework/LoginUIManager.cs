using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtils;

namespace OneID
{
    public class LoginUIManager : SingletonMono<LoginUIManager>
    {
        [SerializeField] List<Transform> _UILayers;
        [SerializeField] Camera _cameraUI;
        [SerializeField] GameObject _loading;

        public List<Transform> UILayers
        {
            get
            {
                return _UILayers;
            }
#if UNITY_EDITOR
            set
            {
                _UILayers = value;
            }
#endif
        }

        [SerializeField] Canvas _mainCanvas;
        public Canvas MainCanvas
        {
            get { return _mainCanvas; }
        }

        [SerializeField] RectTransform _gameRect;
        [SerializeField] RectTransform _leftSafeRect;
        [SerializeField] RectTransform _rightSafeRect;
        [SerializeField] RectTransform _topSafeRect;
        [SerializeField] RectTransform _bottomSafeRect;

        public RectTransform GameRect
        {
            get
            {
                return _gameRect;
            }
#if UNITY_EDITOR
            set
            {
                _gameRect = value;
            }
#endif
        }

        public RectTransform LeftSafeRect
        {
            get
            {
                return _leftSafeRect;
            }
#if UNITY_EDITOR
            set
            {
                _leftSafeRect = value;
            }
#endif
        }
        public RectTransform RightSafeRect
        {
            get
            {
                return _rightSafeRect;
            }
#if UNITY_EDITOR
            set
            {
                _rightSafeRect = value;
            }
#endif
        }
        public RectTransform TopSafeRect
        {
            get
            {
                return _topSafeRect;
            }
#if UNITY_EDITOR
            set
            {
                _topSafeRect = value;
            }
#endif
        }
        public RectTransform BottomSafeRect
        {
            get
            {
                return _bottomSafeRect;
            }
#if UNITY_EDITOR
            set
            {
                _bottomSafeRect = value;
            }
#endif
        }


        public void ShowUI(GameObject instanceUI, int layer)
        {
            var RootUI = UILayers[layer];
            if (instanceUI.transform.parent != RootUI)
            {
                instanceUI.transform.SetParent(RootUI, false);
                RectTransform rectTrans = instanceUI.GetComponent<RectTransform>();
                rectTrans.anchorMin = Vector2.zero;
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMin = rectTrans.offsetMax = Vector2.zero;

                Canvas canvas = instanceUI.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = _cameraUI;
                canvas.planeDistance = 0f;
            }
            instanceUI.transform.SetAsLastSibling();
        }

        public void ShowLoading(bool show)
        {
            _loading.SetActive(show);
        }

        public void SetCanvas(bool isLandscape)
        {

        }
    }
}
