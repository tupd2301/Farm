using System;
using UnityEngine;
using UnityEngine.UI;

namespace Atom.Modules.SpinWheel
{
    public class SpinWheelPiece : MonoBehaviour
    {
        public RectTransform holder;
        public Image bgColor;
        public Image bgImage;
        public Image icon;
        public Text label;
        public Text amount;

        public void Setup(WheelPiece pieceData, float pieceAngle, float fillAmount)
        {
            if (!String.IsNullOrEmpty(pieceData.Icon))
                icon.sprite = Resources.Load<Sprite>(pieceData.Icon);
            if (!String.IsNullOrEmpty(pieceData.BGSprite))
            {
                bgImage.sprite = Resources.Load<Sprite>(pieceData.BGSprite);
                bgImage.enabled = true;
            }

            bgColor.color = pieceData.Color;
            bgColor.fillAmount = fillAmount;
            bgColor.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, pieceAngle);
            label.text = pieceData.Label;
            amount.text = pieceData.Amount.ToString();
        }
    }
}