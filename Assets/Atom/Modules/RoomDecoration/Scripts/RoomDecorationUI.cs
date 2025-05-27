using System;
using System.Collections;
using System.Collections.Generic;
using Athena.Common.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RoomDecoration
{
    public class RoomDecorationUI : UIController
    {
        [SerializeField] private List<PlaceFurnitureButton> _placeButtons;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private float _progressBarMoveDuration = 0.5f;
        [SerializeField] private GameObject _completePanel;
        [SerializeField] private Button _nextButton;
        public List<PlaceFurnitureButton> PlaceButtons { get => _placeButtons; }
        public Slider ProgressBar { get => _progressBar; }
        public Button NextButton { get => _nextButton; set => _nextButton = value; }

        public Action NextButtonClicked;

        public void SetupProgressBar(int maxValue, int currentValue)
        {
            _progressBar.maxValue = maxValue;
            _progressBar.value = currentValue;
        }

        public void UpdateProgressBar(int currentValue)
        {
            _progressBar.DOValue(currentValue, _progressBarMoveDuration);
        }

        public void ShowCompletePanel()
        {
            _completePanel.SetActive(true);
        }

        public void HideCompletePanel()
        {
            _completePanel.SetActive(false);
        }

        public void SetNextButtonCallback(UnityEngine.Events.UnityAction callback)
        {
            _nextButton.onClick.AddListener(callback);
        }

        private void OnDisable()
        {
            _nextButton.onClick.RemoveAllListeners();
        }
    }
}
