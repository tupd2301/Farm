using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomDecoration
{
    public class PlaceFurnitureButton : MonoBehaviour
    {
        public FurnitureType furnitureType;
        public int unlockLevel;
        public Action<FurnitureType> OnClicked;

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;


        private void OnEnable() 
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            OnClicked?.Invoke(furnitureType);
        }

        public void SetPrice(int price)
        {
            text.text = price.ToString();
        }

        private void OnValidate()
        {
            name = "Button_" + furnitureType;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }

#if UNITY_EDITOR
        [ContextMenu("Copy position")]
        public void CopyPosition()
        {
            GUIUtility.systemCopyBuffer = transform.position.ToString();
        }
#endif
    }

}
