using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Atom
{
    public static class UIAnimUtils
    {
        #region Easing functions
        //Copied from: https://gist.github.com/cjddmut/d789b9eb78216998e95c
        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }
        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value - 1)) + start;
        }

        //To create this ease: https://www.desmos.com/calculator/nze7zoan5w
        public static float EaseReverseQuad(float x)
        {
            return 4 * (-x * x + x);
        }
        #endregion


        #region Animation
        public static void PlayFadeAnim(Image image, float start, float finish, float duration = 0.2f, Ease ease = Ease.InOutSine, System.Action onFinish = null)
        {
            image.DOKill();
            image.color = new Color(image.color.r, image.color.g, image.color.b, start);
            image.DOFade(finish, duration).SetEase(ease).OnComplete(() => {
                onFinish?.Invoke();
            });
        }

        public static void SetAlpha(Image image, float alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        public static void PlaySizeDeltaXAnim(RectTransform rectTransform, float start, float finish, float duration = 0.2f, Ease ease = Ease.InOutSine, System.Action onFinish = null)
        {
            rectTransform.DOKill();
            rectTransform.sizeDelta = new Vector2(start, rectTransform.sizeDelta.y);
            rectTransform.DOSizeDelta(new Vector2(finish, rectTransform.sizeDelta.y), duration).SetEase(ease).OnComplete(() => {
                onFinish?.Invoke();
            }); ;
        }

        public static void PlayPopScaleAnim(Transform transform, Vector3 start, float intensity = 1.05f, float duration = 0.2f, System.Action onReachedHighest = null, System.Action onFinish = null)
        {
            transform.DOKill();
            transform.localScale = start;
            transform.DOScale(start * intensity, duration / 2).SetEase(Ease.InCubic).OnComplete(() => {
                onReachedHighest?.Invoke();
            });
            transform.DOScale(start, duration / 2).SetEase(Ease.OutCubic).SetDelay(duration/2).OnComplete(() =>
            {
                onFinish?.Invoke();
            });
        }

        public static void PlayShowPopUpAnim(Transform transform, System.Action onFinish)
        {
            CanvasGroup canvasGroup = transform.GetComponent<CanvasGroup>();
            canvasGroup.DOKill();
            canvasGroup.alpha = 1f;

            transform.DOKill();
            Vector3 originalScale = transform.localScale;
            transform.localScale = originalScale * 0.8f;
            transform.DOScale(originalScale * 1.02f, Global.UIConfig.POPUP_TIME / 3).SetEase(Ease.OutQuad);
            transform.DOScale(originalScale * 0.98f, Global.UIConfig.POPUP_TIME / 3).SetDelay(Global.UIConfig.POPUP_TIME / 3).SetEase(Ease.InQuad);
            transform.DOScale(originalScale, Global.UIConfig.POPUP_TIME / 3).SetDelay(Global.UIConfig.POPUP_TIME * 2 / 3).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    onFinish?.Invoke();
                });
        }

        public static void PlayHidePopUpAnimEase(Transform transform, System.Action onFinish)
        {
            CanvasGroup canvasGroup = transform.GetComponent<CanvasGroup>();
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.InQuad).OnComplete(() => {
                onFinish?.Invoke();
            });
        }

        public static void PlayHidePopUpAnim(Transform transform, System.Action onFinish)
        {
            CanvasGroup canvasGroup = transform.GetComponent<CanvasGroup>();
            canvasGroup.DOKill();
            canvasGroup.alpha = 0;
            onFinish?.Invoke();
        }
        #endregion



        #region Game Object Hierachy Utilities
        public static void AssignAnimForChildButtons(Transform rootTransform)
        {
            List<Button> childButtons = GetAllNestedChildComponents<Button>(rootTransform).ToList();

            foreach (Button childButton in childButtons)
            {
                if (childButton.GetComponent<ButtonAnimAssignee>() != null)
                    continue;

                ButtonAnimAssignee buttonAnimAssignee = childButton.gameObject.AddComponent<ButtonAnimAssignee>();
                buttonAnimAssignee.Setup();
            }
        }

        private static List<ComponentType> GetAllNestedChildComponents<ComponentType>(Transform rootTransform)
        {
            List<ComponentType> childComponents = new List<ComponentType>();

            Transform currentExaminedTransform = rootTransform;
            Stack<Transform> transformsToExamined = new Stack<Transform>();

            int iterationsLeft = 200;   //To avoid infinite loop

            while (iterationsLeft > 0)
            {
                if (transformsToExamined.Count > 0)
                {
                    currentExaminedTransform = transformsToExamined.Pop();
                }

                if (currentExaminedTransform == null)
                {
                    break;
                }

                foreach (Transform childTransform in currentExaminedTransform)
                {
                    if (childTransform.childCount > 0)
                    {
                        transformsToExamined.Push(childTransform);
                    }

                    ComponentType childComponent = childTransform.GetComponent<ComponentType>();

                    if (childComponent != null)
                        childComponents.Add(childComponent);
                }

                ComponentType component = currentExaminedTransform.GetComponent<ComponentType>();

                if (component != null)
                    childComponents.Add(component);

                currentExaminedTransform = null;

                iterationsLeft--;
            }

            return childComponents;
        }
        #endregion



        #region Others
        public static string TimeSpanToString(System.TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
            {
                return timeSpan.Days.ToString() + "d" + timeSpan.Hours.ToString() + "h";
            }
            else if (timeSpan.Hours > 0)
            {
                return timeSpan.Hours.ToString() + ":" + timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString();
            }
            else
            {
                return timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString();
            }
        }
        #endregion
    }
}