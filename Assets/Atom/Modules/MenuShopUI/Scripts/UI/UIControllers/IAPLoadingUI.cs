using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Athena.Common;
using Athena.Common.UI;
using DG.Tweening;
using TMPro;
using System.Linq;

namespace Atom
{
    public class IAPLoadingUI : FlexibleUIController
    {
        public System.Action onTimeOut, onLoadingShowed;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        private bool _autoHide;
        private float _timeOut;
        public void Setup(bool autoHide, float timeOut)
        {
            _autoHide = autoHide;
            _timeOut = timeOut;
        }

        public void SetLayer(string layer)
        {
            Canvas canvas = GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = layer;
        }

        public override void Show()
        {
            StartCoroutine(LoadingProcess());
            onShowFinished?.Invoke();
        }

        public void DisableAutoHide()
        {
            _autoHide = false;
        }

        private IEnumerator LoadingProcess()
        {
            yield return Yielders.Get(1.5f);
            onLoadingShowed?.Invoke();

            while (_timeOut > 0.0f)
            {
                float deltaTime = 0.5f;
                _timeOut -= deltaTime;
                yield return Yielders.Get(deltaTime);
            }

            if (_autoHide)
            {
                onTimeOut?.Invoke();
            }
            yield break;
        }

        public override void Hide()
        {
            StopAllCoroutines();
            onHideStarted?.Invoke();
            onFadeOutFinished = () => {
                onHideFinished?.Invoke();
            };
            FadeOut();
        }

        public override void FadeIn()
        {
            onFadeInStarted?.Invoke();
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(1f, Global.UIConfig.FADING_TIME).SetEase(Ease.OutSine).OnComplete(() => {
                onFadeInFinished?.Invoke();
            });
        }

        public override void FadeOut()
        {
            onFadeOutStarted?.Invoke();
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(0f, Global.UIConfig.FADING_TIME).SetEase(Ease.OutSine).OnComplete(() => {
                onFadeOutFinished?.Invoke();
            });
        }
    }
}