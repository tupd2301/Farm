using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Athena.Common;
using Athena.Common.UI;

namespace Atom
{
    public class RatingUI : FlexibleUIController
    {
        public System.Action onCancelPressed, onOKPressed;
        public System.Action<int> onStarPressed;

        [SerializeField]
        private Button[] _stars;
        [SerializeField]
        private Button _sendButton;
        [SerializeField]
        private Sprite _spriteButtonEnabled, _spriteButtonDisabled;

        public void Setup()
        {
            SetActiveSendButton(false);
            GetComponent<GraphicRaycaster>().enabled = false;
            foreach (Button star in _stars)
            {
                star.transform.Find("Icon").gameObject.SetActive(false);
            }

            UIAnimUtils.AssignAnimForChildButtons(this.transform);
        }

        public override void Show()
        {
            base.Show();

            onShowStarted?.Invoke();
            UIAnimUtils.PlayShowPopUpAnim(this.transform, () => StartCoroutine(AppearProcess()));
        }

        private IEnumerator AppearProcess()
        {
            yield return Yielders.Get(.25f);

            for (int i = 0; i < 5; i++)
            {
                _stars[i].transform.Find("Icon").gameObject.SetActive(true);
                yield return Yielders.Get(.1f);
            }

            for (int i = 4; i >= 0; i--)
            {
                _stars[i].transform.Find("Icon").gameObject.SetActive(false);
                yield return Yielders.Get(.1f);
            }

            GetComponent<GraphicRaycaster>().enabled = true;
            onShowFinished?.Invoke();
        }

        public void Rate(int index)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i <= index)
                {
                    _stars[i].transform.Find("Icon").gameObject.SetActive(true);
                }
                else
                {
                    _stars[i].transform.Find("Icon").gameObject.SetActive(false);
                }
            }
        }

        public override void Hide()
        {
            base.Hide();

            onHideStarted?.Invoke();
            UIAnimUtils.PlayHidePopUpAnim(this.transform, onHideFinished);
        }

        public void OnCancelPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onCancelPressed?.Invoke();
        }

        public void OnOKPressed()
        {
            AudioManager.Instance.PlaySfxTapButton();
            onOKPressed?.Invoke();
        }

        public void OnStarPressed(int index)
        {
            AudioManager.Instance.PlaySfxTapButton();
            onStarPressed?.Invoke(index);
        }

        public void SetActiveSendButton(bool isActive)
        {
            _sendButton.enabled = isActive;
            _sendButton.image.overrideSprite = isActive ? _spriteButtonEnabled : _spriteButtonDisabled;
        }
    }
}