using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Atom
{
    public class ButtonAnimAssignee : MonoBehaviour
    {
        [SerializeField]
        AnimType _animType = AnimType.ScaleObject;

        [SerializeField]
        Transform _childTransform = null;
        public Transform ChildTransform { get => _childTransform; set => _childTransform = value; }

        [SerializeField]
        bool _shouldSetupOnAwake = false;



        private void Awake()
        {
            if (_shouldSetupOnAwake)
            {
                Setup();
            }
        }

        public void Setup()
        {
            EventTrigger eventTrigger = this.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry onPointerDownEntry = new EventTrigger.Entry();
            onPointerDownEntry.eventID = EventTriggerType.PointerDown;

            EventTrigger.Entry onPointerUpEntry = new EventTrigger.Entry();
            onPointerUpEntry.eventID = EventTriggerType.PointerUp;

            switch (_animType)
            {
                case AnimType.ScaleObject:
                    Vector3 originalButtonScale = this.transform.localScale;
                    onPointerDownEntry.callback.AddListener((eventData) => PlayObjectShrinkAnim(eventData, originalButtonScale));
                    onPointerUpEntry.callback.AddListener((eventData) => PlayObjectRevertScaleAnim(eventData, originalButtonScale));
                    break;
                case AnimType.ScaleChild:
                    if (_childTransform == null)
                        return;
                    Vector3 originalTextScale = _childTransform.localScale;
                    onPointerDownEntry.callback.AddListener((eventData) => PlayShrinkAnim(_childTransform.transform, originalTextScale));
                    onPointerUpEntry.callback.AddListener((eventData) => PlayRevertScaleAnim(_childTransform.transform, originalTextScale));
                    break;
                default:
                    break;
            }

            eventTrigger.triggers.Add(onPointerDownEntry);
            eventTrigger.triggers.Add(onPointerUpEntry);
        }

        private static void PlayObjectShrinkAnim(BaseEventData eventData, Vector3 originalScale)
        {
            if (eventData.selectedObject != null)
            {
                eventData.selectedObject.transform.DOKill();
                eventData.selectedObject.transform.DOScale(originalScale * 0.9f, 0.05f);
            }
        }

        private static void PlayShrinkAnim(Transform transform, Vector3 originalScale)
        {
            if (transform != null)
            {
                transform.DOKill();
                transform.DOScale(originalScale * 0.9f, 0.05f);
            }
        }

        private static void PlayObjectRevertScaleAnim(BaseEventData eventData, Vector3 originalScale)
        {
            if (eventData.selectedObject != null)
            {
                eventData.selectedObject.transform.DOKill();
                eventData.selectedObject.transform.DOScale(originalScale, 0.05f);
            }
        }

        private static void PlayRevertScaleAnim(Transform transform, Vector3 originalScale)
        {
            if (transform != null)
            {
                transform.DOKill();
                transform.DOScale(originalScale, 0.05f);
            }
        }

        public enum AnimType
        {
            None,
            ScaleObject,
            ScaleChild,
        }
    }
}
