using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Factory
{
    public class CoinTrigger : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        public CoinController coinController;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick");
            coinController.OnClick();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnPointerEnter");
            // coinController.OnClick();
        }
    }
}
