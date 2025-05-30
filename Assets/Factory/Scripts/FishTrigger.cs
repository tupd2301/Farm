using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Factory
{
    public class FishTrigger : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private FishController fishController;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick");
            fishController.OnClick();
        }
    }
}
