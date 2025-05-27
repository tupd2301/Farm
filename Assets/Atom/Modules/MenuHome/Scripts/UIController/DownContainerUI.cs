using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common.UI;
using UnityEngine.UI;

namespace Atom
{
    public class DownContainerUI : UIController
    {
        [SerializeField] private List<FloatingButton> _floatingButtons;
        [SerializeField] private Sprite _spriteInactive;
        [SerializeField] private Sprite _spriteActive;

        public void InitFloatingUI(List<UIController> listUI)
        {
            for (int i = 0; i < listUI.Count; i++)
            {
                if (i < _floatingButtons.Count)
                {
                    _floatingButtons[i].UI = listUI[i];
                }
            }
            ActiveButton(1);
        }

        public void PlayButton(int index)
        {
            ActiveButton(index);
        }

        private void ActiveButton(int index)
        {
            for (int i = 0; i < _floatingButtons.Count; i++)
            {
                if (i == index)
                {
                    _floatingButtons[i].IconAnimator.SetBool("Active", true);
                    _floatingButtons[i].IconSprite.sprite = _spriteActive;
                    _floatingButtons[i].TextName.SetActive(true);
                    _floatingButtons[i].UI.gameObject.SetActive(true);
                }
                else
                {
                    _floatingButtons[i].IconAnimator.SetBool("Active", false);
                    _floatingButtons[i].IconSprite.sprite = _spriteInactive;
                    _floatingButtons[i].TextName.SetActive(false);
                    _floatingButtons[i].UI.gameObject.SetActive(false);
                }
            }

        }
    }

    [System.Serializable]
    public class FloatingButton
    {
        public Animator IconAnimator;
        public Image IconSprite;
        public GameObject TextName;
        public UIController UI;
    }
}
