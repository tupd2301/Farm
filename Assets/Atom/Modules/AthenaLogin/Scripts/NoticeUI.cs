using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class NoticeUI : MonoBehaviour
    {
        public System.Action OnGoBack;
        [SerializeField]
        public Text _messageText;

        public void SetMessageText(string message)
        {
            _messageText.text = message;
        }


        public void GoBack()
        {
            OnGoBack?.Invoke();
        }
    }
}