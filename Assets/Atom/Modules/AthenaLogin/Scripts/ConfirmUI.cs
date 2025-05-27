using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class ConfirmUI : MonoBehaviour
    {
        public System.Action OnOK;
        public System.Action OnCancel;
        [SerializeField]
        public Text _messageText;

        public void SetMessageText(string message)
        {
            _messageText.text = message;
        }

        public void OnOKClick()
        {
            OnOK?.Invoke();
        }

        public void OnCancelClick()
        {
            OnCancel?.Invoke();
        }
    }
}