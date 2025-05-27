using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Factory
{
    public class BoxManager : MonoBehaviour
    {
        public static BoxManager Instance;
        public List<BoxController> _boxes;

        public GameObject _boxPrefab;

        public GameObject _boxParent;
        public GameObject _gate;

        public float _boxMoveDistance = 3;

        public int currentBoxIndex = 0;

        public bool isBoxClosing = false;

        public Slider _patienceSlider;

        void Awake()
        {
            Instance = this;
        }

        public void SpawnTextFloating(string text, Vector3 position)
        {
            var textObject = PoolSystem.Instance.GetObject("TextFloating");
            textObject.transform.SetParent(transform);
            text = float.Parse(text).ToString("0.0");
            var value = float.Parse(text);
            textObject.GetComponentInChildren<TMP_Text>().text = "+" + text + "$";
            textObject.SetActive(true);
            textObject.transform.localPosition = Vector3.zero;
            textObject.transform.DOMoveX(position.x, 0).SetEase(Ease.InSine);
            float moveY = 2;
            float scale = 1f;
            if (value > 0 && value < 10)
            {
                moveY = 2;
                scale = 1f;
            }
            else if (value >= 10 && value < 100)
            {
                moveY = 1;
                scale = 1.5f;
            }
            else if (value >= 100)
            {
                moveY = 0.5f;
                scale = 2f;
            }
            textObject.transform.DOScale(Vector3.one * 0.01f * scale, 0.3f).SetEase(Ease.InCirc);
            textObject
                .transform.DOLocalMoveY(moveY, 1f)
                .SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    textObject.transform.localScale = Vector3.one * 0.01f;
                    PoolSystem.Instance.ReturnObject(textObject, "TextFloating");
                });
        }

        void Update()
        {
            if (
                GameManager.Instance.homeUI != null
                && !isBoxClosing
                && !GameManager.Instance.isStop
            )
            {
                _patienceSlider.value = Mathf.Clamp(
                    _patienceSlider.value + Time.deltaTime,
                    0,
                    _patienceSlider.maxValue
                );
                float value = Mathf.Clamp01(
                    (float)_patienceSlider.value / _patienceSlider.maxValue
                );
                _patienceSlider.fillRect.GetComponent<Image>().color = new Color(
                    1,
                    1 - value,
                    1 - value,
                    1
                );
                if (_patienceSlider.value >= _patienceSlider.maxValue)
                {
                    _patienceSlider.value = 0;
                    isBoxClosing = true;
                    GameManager.Instance.ShowLosePanel();
                }
            }
        }

        public void UpdatePatienceSlider(int patience, int maxPatience = -1)
        {
            if (maxPatience != -1)
            {
                _patienceSlider.maxValue = maxPatience;
            }
            _patienceSlider.DOValue(patience, 0.3f).SetEase(Ease.InSine);
            float value = Mathf.Clamp01((float)patience / _patienceSlider.maxValue);
            _patienceSlider.fillRect.GetComponent<Image>().color = new Color(
                1,
                1 - value,
                1 - value,
                1
            );
            Debug.Log(1 - value);
        }

        [ContextMenu("MoveNextBox")]
        public async Task MoveNextBox()
        {
            isBoxClosing = true;
            var currentBox = _boxes[currentBoxIndex];
            await _gate
                .transform.DOLocalMoveX(-2, 0.3f)
                .SetEase(Ease.OutCirc)
                .AsyncWaitForCompletion();
            await currentBox.ShowTop();
            if (currentBoxIndex >= _boxes.Count - 1)
            {
                ClearBoxs();
                await Task.Delay(1000);
                await GameManager.Instance.NextDay();
                return;
            }
            currentBoxIndex++;
            UpdatePatienceSlider(0, _boxes[currentBoxIndex].boxConfig.patienceValue);
            float moveDistance = _boxMoveDistance * currentBoxIndex;
            await transform
                .DOLocalMoveX(-moveDistance, 1f)
                .SetEase(Ease.OutCirc)
                .AsyncWaitForCompletion();
            await _gate
                .transform.DOLocalMoveX(0, 0.3f)
                .SetEase(Ease.OutCirc)
                .AsyncWaitForCompletion();
            isBoxClosing = false;
        }

        public void GetItem(ItemController item)
        {
            if (currentBoxIndex >= _boxes.Count || item == null || isBoxClosing)
            {
                return;
            }
            SpawnTextFloating(item.itemData.cost.ToString(), item.transform.position);
            _boxes[currentBoxIndex].GetItem(item);
        }

        public void ClearBoxs()
        {
            DestroyBoxes();
            transform.DOKill();
            isBoxClosing = true;
            _gate.transform.DOLocalMoveX(-2, 1f).SetEase(Ease.InSine);
            transform.DOLocalMoveX(0, 1.0f).SetEase(Ease.InSine);
        }

        public void OpenGate()
        {
            _gate.transform.DOLocalMoveX(0, 1f).SetEase(Ease.InSine);
        }

        public void DestroyBoxes()
        {
            foreach (var box in _boxes)
            {
                Destroy(box.gameObject);
            }
            _boxes.Clear();
        }

        public void Init(List<BoxConfigDay> boxConfigs)
        {
            ClearBoxs();
            currentBoxIndex = 0;
            isBoxClosing = true;
            var index = 0;
            _boxes = new List<BoxController>();
            foreach (var boxConfig in boxConfigs)
            {
                for (int i = 0; i < boxConfig.amount; i++, index++)
                {
                    var box = Instantiate(_boxPrefab, _boxParent.transform);
                    box.GetComponent<BoxController>().boxConfig = boxConfig.boxConfig;
                    _boxes.Add(box.GetComponent<BoxController>());
                    box.transform.localPosition = new Vector3(_boxMoveDistance * index, 0, 0);
                    Debug.Log("InitBox: " + boxConfig.boxConfig.boxCurrencyValue);
                    box.GetComponent<BoxController>().Init(boxConfig.boxConfig);
                }
            }
            UpdatePatienceSlider(0, _boxes[currentBoxIndex].boxConfig.patienceValue);
        }
    }
}
