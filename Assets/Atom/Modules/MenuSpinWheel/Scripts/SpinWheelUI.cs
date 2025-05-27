using System.Collections;
using System.Collections.Generic;
using Athena.Common.UI;
using TMPro;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.UI;

namespace Atom.Modules.SpinWheel
{
    public class SpinWheelUI : UIController
    {
        [SerializeField][Range(.2f, 2f)] private float wheelSize = 1f;
        [SerializeField] private Button _spinButton;

        [Header("References :")]
        [SerializeField] private Transform _linesParent;

        [Space]
        [SerializeField] private Transform _spinWheelTransform;
        [SerializeField] private Transform _wheelCircle;
        [SerializeField] private Transform _wheelPiecesParent;

        [Header("Result screen :")]
        [SerializeField] private GameObject _resultScreen;
        [SerializeField] private TextMeshProUGUI _resultLabel;
        [SerializeField] private Image _resultIcon;
        [SerializeField] private TextMeshProUGUI _resultAmout;

        public Transform LinesParent { get => _linesParent; set => _linesParent = value; }
        public Transform SpinWheelTransform { get => _spinWheelTransform; set => _spinWheelTransform = value; }
        public Transform WheelCircle { get => _wheelCircle; set => _wheelCircle = value; }
        public Transform WheelPiecesParent { get => _wheelPiecesParent; set => _wheelPiecesParent = value; }
        public Button SpinButton { get => _spinButton; set => _spinButton = value; }

        public void ShowResultScreen(WheelPiece piece)
        {
            _resultScreen.SetActive(true);
            _resultLabel.text = piece.Label;
            _resultIcon.sprite = Resources.Load<Sprite>(piece.Icon);
            _resultAmout.text = piece.Amount.ToString();
        }

        private void OnValidate()
        {
            if (SpinWheelTransform != null)
                SpinWheelTransform.localScale = new Vector3(wheelSize, wheelSize, 1f);
        }
    }
}
