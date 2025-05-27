using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace OneID
{
    public class UIElementDragger : EventTrigger
    {
        private bool dragging;
        private Vector2 _startPosition;
        private Vector2 _dragOffset;

        public void Start()
        {
            if (PlayerPrefs.HasKey(PlayerPrefKey.OneSDKButtonPositionX))
            {
                //transform.position = new Vector3(PlayerPrefs.GetFloat(PlayerPrefKey.OneSDKButtonPositionX), PlayerPrefs.GetFloat(PlayerPrefKey.OneSDKButtonPositionY), 0);
            }
        }

        public void Update()
        {
            if (dragging)
            {
                transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + _dragOffset;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            dragging = true;
            _startPosition = (Vector2)transform.position;
            _dragOffset = (Vector2)transform.position - (Vector2)Input.mousePosition;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            dragging = false;
            Vector2 offset = (Vector2)transform.position - _startPosition;
            if (offset.sqrMagnitude < 400)
            {
                transform.parent.parent.GetComponent<MainOneSDKUI>().ShowGuestAccountUI();
            }
            else
            {
                PlayerPrefs.SetFloat(PlayerPrefKey.OneSDKButtonPositionX, transform.position.x);
                PlayerPrefs.SetFloat(PlayerPrefKey.OneSDKButtonPositionY, transform.position.y);
            }
        }
    }
}