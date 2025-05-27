using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Factory
{
    public class BoxController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _body;

        [SerializeField]
        private GameObject _top;

        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private Slider _fillSlider;

        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private TMP_Text _fillMaxText;

        [SerializeField]
        private TMP_Text _boxTypeText;

        public float fillMax = 100;
        public float fillCurrent = 0;

        public int boxMoney = 5;

        public bool isFull = false;

        public BoxConfig boxConfig;

        void Start()
        {
            _canvas.worldCamera = GameManager.Instance.mainCamera;
            _fillSlider.value = 0;
        }

        public void Init(BoxConfig boxConfig)
        {
            this.boxConfig = boxConfig;
            this.fillMax = boxConfig.boxCurrencyValue;
            fillCurrent = 0;
            _fillSlider.value = 0;
            _top.transform.DOComplete();
            _top.SetActive(false);
            _top.transform.localScale = Vector3.one;
            _top.transform.localPosition = new Vector3(-2, 1.5f, 0);
            _top.transform.localRotation = Quaternion.Euler(0, 0, 90);
            _fillMaxText.text = fillMax.ToString();
            _boxTypeText.text = boxConfig.boxType.ToString();
        }

        public async Task GetItem(ItemController item)
        {
            fillCurrent += item.itemData.cost;
            _fillSlider.value = Mathf.Clamp01(fillCurrent / fillMax);
            _transform.DOComplete();
            await _transform
                .DOScale(1.05f, 0.03f)
                .SetEase(Ease.InSine)
                .SetLoops(2, LoopType.Yoyo)
                .AsyncWaitForCompletion();
            _fillMaxText.text = fillMax.ToString();
            if (fillCurrent >= fillMax && !isFull)
            {
                isFull = true;
                fillCurrent = 0;
                await BoxManager.Instance.MoveNextBox();
            }
        }

        [ContextMenu("ShowTop")]
        public async Task ShowTop()
        {
            _top.transform.DOComplete();
            _top.SetActive(true);
            _top.transform.localScale = Vector3.one;
            _top.transform.localPosition = new Vector3(-2, 1.5f, 0);
            _top.transform.localRotation = Quaternion.Euler(0, 0, 90);
            var tween = _top
                .transform.DOLocalRotate(new Vector3(0, 0, 0), 0.7f)
                .SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    _transform
                        .DOScale(new Vector3(1.1f, 1.1f, 1f), 0.2f)
                        .SetEase(Ease.InSine)
                        .SetLoops(2, LoopType.Yoyo);
                });
            await tween.AsyncWaitForCompletion();
            GameManager.Instance.AddGold(boxConfig.boxCurrencyValue);
        }
    }
}
