using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Factory
{
    public class GearController
        : MonoBehaviour,
            IDropHandler,
            IBeginDragHandler,
            IEndDragHandler,
            IDragHandler
    {
        [SerializeField]
        private Image _gearIcon;

        [SerializeField]
        private Image _gearItemIcon;

        [SerializeField]
        private Image _gearItemIconBG;

        [SerializeField]
        private RectTransform _gui;

        [SerializeField]
        private TMP_Text _levelText;

        [SerializeField]
        private Sprite _gear1;

        [SerializeField]
        private Sprite _gear6ForItemIcon;

        [SerializeField]
        private Sprite _gear6ForTextIcon;

        public Vector2 gridCoordinate;

        public List<GearNeighbor> connectedGears = new List<GearNeighbor>();

        public int direction = 0;
        public float angle = 90;

        public float startAngle = 0;
        public bool isActive = false;
        public bool isHead = false;
        public bool isReverse = false;

        public bool isInShop = false;

        public System.Action<float> OnRotate;
        public System.Action<GearData> OnDropShop;

        private GearController _tempGear;
        private Canvas _canvas;
        private RectTransform _rectTransform;

        public GearData gearData;
        public ItemData itemData;
        public float currentTotalTickValue = 0;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _gearItemIcon.material = new Material(_gearItemIcon.material);
            _gearItemIcon.material.SetFloat("_Fill", 1f);
            _gearItemIconBG.material = new Material(_gearItemIconBG.material);
            _gearItemIconBG.material.SetFloat("_Fill", 1f);
            OnDropShop += (gearData) =>
            {
                Debug.Log("Default OnDropShop: " + (int)gearData.cost);
            };
        }

        public void FillItemIcon(float fillAmount)
        {
            _gearItemIcon?.material?.SetFloat("_Fill", fillAmount);
            // _gearItemIconBG?.material?.SetFloat("_Fill", fillAmount);
        }

        public void Show()
        {
            isActive = true;
            _gui.gameObject.SetActive(true);
            _gearIcon.transform.localEulerAngles = new Vector3(0, 0, startAngle);
        }

        public void Hide()
        {
            isActive = false;
            _gui.gameObject.SetActive(false);
            gearData = null;
            itemData = null;
        }

        public void SetGearData(GearData data)
        {
            currentTotalTickValue = 0;
            FillItemIcon(1);
            if (gearData == null)
            {
                gearData = new GearData();
            }
            if (data == null)
            {
                return;
            }
            gearData.Copy(data);
            if (gearData.id == 0)
            {
                _levelText.gameObject.SetActive(true);
                _gearItemIcon.gameObject.SetActive(false);
                _levelText.text = gearData.level.ToString();
                SetGear(gearData.id);
            }
            else
            {
                _levelText.gameObject.SetActive(false);
                _gearItemIcon.gameObject.SetActive(true);
                var sprite = Resources.Load<Sprite>("Sprites/" + data.iconName);
                if (sprite != null)
                {
                    _gearItemIcon.sprite = sprite;
                    SetGear(gearData.id);
                }
                else
                {
                    _gearItemIcon.sprite = null;
                    SetGear(gearData.id);
                }
            }
        }

        public void SetItemData(GearData data)
        {
            ItemData item = GameManager.Instance.GetItemDataByGearID(data.id);
            if (item != null)
            {
                itemData = new ItemData();
                itemData.Copy(item);
            }
        }

        public void SetGear(int gear)
        {
            if (isHead)
            {
                _gearIcon.sprite = _gear1;
                return;
            }
            _gearIcon.sprite = gear == 0 ? _gear6ForTextIcon : _gear6ForItemIcon;
        }

        public void Connect(GearController gear, int direction)
        {
            connectedGears.Add(new GearNeighbor { gear = gear, direction = direction });
        }

        public void Disconnect(GearController gear, int direction)
        {
            connectedGears.Remove(
                connectedGears.Find(g => g.gear == gear && g.direction == direction)
            );
        }

        public void Rotate()
        {
            if (isHead)
            {
                _gearIcon.transform.DOKill();
            }

            Ease ease = Ease.Linear;
            if (connectedGears.Exists(x => x.direction == direction && x.gear.isActive))
            {
                ease = Ease.InSine;
            }
            _gearIcon
                .transform.DORotate(new Vector3(0, 0, angle * ((direction + 1) % 4)), 0.5f)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    Rotate();
                    NeighborRotate();
                });
        }

        public void Rotate(float angle, float Amplifier)
        {
            _gearIcon.transform.DOComplete();
            _gearIcon
                .transform.DORotate(new Vector3(0, 0, angle + startAngle), 0.1f)
                .OnComplete(() =>
                {
                    UpdateRotationProgress(Amplifier);
                });
        }

        public void UpdateRotationProgress(float Amplifier)
        {
            _gearIcon.transform.localEulerAngles = new Vector3(0, 0, startAngle);
            if (gearData == null || gearData.id == 0 || isHead)
            {
                return;
            }
            if (GameManager.Instance.GameState.CurrentState != GameStateType.Main)
                return;
            float AmplifierToTick = Amplifier >= gearData.maxValue ? 0 : Amplifier;
            float AmplifierToCost =
                Amplifier >= gearData.maxValue ? Amplifier - gearData.maxValue : 0;
            int bonus = (int)(AmplifierToTick / gearData.maxValue);
            currentTotalTickValue += gearData.tickValue + (bonus >= 1 ? 0 : AmplifierToTick);
            if (currentTotalTickValue >= gearData.maxValue)
            {
                int rotationCount = (int)(currentTotalTickValue / gearData.maxValue) + bonus;
                currentTotalTickValue = currentTotalTickValue % gearData.maxValue;
                StartCoroutine(InvokeRotateWithDelay(rotationCount, AmplifierToCost));
            }
            FillItemIcon(currentTotalTickValue / gearData.maxValue);
        }

        private IEnumerator InvokeRotateWithDelay(int rotationCount, float Amplifier)
        {
            for (int i = 0; i < rotationCount; i++)
            {
                yield return new WaitForSeconds(0.1f);
                OnRotate?.Invoke(Amplifier);
            }
        }

        public void NeighborRotate()
        {
            List<GearController> allConnectedGears = FindAllConnectedGearsOnBoard();
            direction = (direction + 1) % 4;
            float Amplifier = 0;
            foreach (var gear in allConnectedGears)
            {
                if (gear.gearData.id == 0)
                {
                    if (Amplifier == 0)
                    {
                        Amplifier = 1;
                    }
                    Amplifier += ((0.2f * Mathf.Pow(2, gear.gearData.level - 1)));
                }
            }
            Amplifier *= GameManager.Instance.GetGearDataByID(0).baseValue;
            Debug.Log("Amplifier: " + Amplifier);
            foreach (var gear in allConnectedGears)
            {
                gear.Rotate(45 * (gear.isReverse ? -1 : 1), Amplifier);
            }
        }

        public List<GearController> FindAllConnectedGearsOnBoard(List<GearController> allGears)
        {
            List<GearController> gears = new List<GearController>();
            foreach (var gear in allGears)
            {
                foreach (var connectedGear in gear.connectedGears)
                {
                    if (
                        connectedGear.gear.isActive
                        && !connectedGear.gear.isHead
                        && !allGears.Contains(connectedGear.gear)
                    )
                    {
                        gears.Add(connectedGear.gear);
                        connectedGear.gear.isReverse = !gear.isReverse;
                    }
                }
            }
            if (gears.Count != 0)
            {
                allGears.AddRange(gears);
                allGears = FindAllConnectedGearsOnBoard(allGears);
            }
            return allGears;
        }

        public List<GearController> FindAllConnectedGearsOnBoard()
        {
            List<GearController> gears = new List<GearController>();
            foreach (var gear in connectedGears)
            {
                if (gear.gear.isActive && !gear.gear.isHead && gear.direction == direction)
                {
                    gear.gear.isReverse = !isReverse;
                    gears.Add(gear.gear);
                }
            }
            if (gears.Count != 0)
            {
                gears = FindAllConnectedGearsOnBoard(gears);
            }
            return gears;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (GameManager.Instance.GameState.CurrentState == GameStateType.Main)
                return;

            var droppedGear = eventData.pointerDrag.GetComponent<GearController>();
            if (!droppedGear)
                return;

            if (isHead || isInShop || droppedGear == this || droppedGear.gearData == null)
                return;

            if (CanMergeAmplifierGears(droppedGear))
            {
                if (droppedGear.isInShop)
                {
                    droppedGear.OnDropShop?.Invoke(droppedGear.gearData);
                }
                MergeAmplifierGears(droppedGear);
                return;
            }
            else if (droppedGear.isInShop && gearData != null)
            {
                return;
            }

            if (
                gearData != null
                && !string.IsNullOrEmpty(gearData.itemName)
                && !string.IsNullOrEmpty(droppedGear.gearData.itemName)
            )
            {
                SwapGears(droppedGear);
            }
            else
            {
                if (droppedGear.isInShop && !isInShop)
                {
                    if (!GameManager.Instance.CheckGold((int)droppedGear.gearData.cost))
                    {
                        Debug.Log("Not enough gold");
                        GameManager.Instance.HomeUI.WarningGoldPanel();
                        return;
                    }
                    droppedGear.OnDropShop?.Invoke(droppedGear.gearData);
                }
                TransferGear(droppedGear);
            }
            GameManager.Instance.CheckFirstOpenShop();
        }

        private bool CanMergeAmplifierGears(GearController otherGear)
        {
            if (
                GameManager.Instance.GameState.CurrentState == GameStateType.Shop
                && otherGear.isInShop
                && !isInShop
                && !GameManager.Instance.CheckGold((int)otherGear.gearData.cost)
            )
            {
                Debug.Log("Not enough gold");
                GameManager.Instance.HomeUI.WarningGoldPanel();
                return false;
            }
            return gearData != null
                && otherGear.gearData.id == gearData.id
                && gearData.id == 0
                && otherGear.gearData.level == gearData.level;
        }

        private void MergeAmplifierGears(GearController otherGear)
        {
            gearData.level++;
            _levelText.text = gearData.level.ToString();
            otherGear.Hide();
        }

        private void SwapGears(GearController otherGear)
        {
            GearData tempGearData = new GearData();
            tempGearData.Copy(gearData);

            SetGearData(otherGear.gearData);
            SetItemData(otherGear.gearData);
            Show();
            Debug.Log("SwapGears");
            otherGear.SetGearData(tempGearData);
            otherGear.SetItemData(tempGearData);
            otherGear.Show();
        }

        private void TransferGear(GearController otherGear)
        {
            SetGearData(otherGear.gearData);
            SetItemData(otherGear.gearData);
            Show();
            otherGear.Hide();
            GetComponent<CanvasGroup>().alpha = 1f;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (
                isHead
                || !isActive
                || GameManager.Instance.GameState.CurrentState == GameStateType.Main
            )
                return;

            // Create a temporary gear
            _tempGear = Instantiate(gameObject, GameManager.Instance.TempContainer.transform)
                .GetComponent<GearController>();
            _tempGear.transform.SetParent(GameManager.Instance.TempContainer.transform);
            _tempGear.transform.localScale = Vector3.zero;
            _tempGear.GetComponent<RectTransform>().sizeDelta = Vector2.one * 190;
            _tempGear.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f).SetEase(Ease.InBack);

            // Set the temporary gear's properties
            _tempGear._gearIcon.raycastTarget = false;
            _tempGear.GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().alpha = 0.5f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (
                isHead
                || !isActive
                || GameManager.Instance.GameState.CurrentState == GameStateType.Main
            )
                return;
            if (_tempGear == null)
                return;

            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _tempGear.transform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out position
            );
            _tempGear.transform.localPosition = position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (GameManager.Instance.GameState.CurrentState == GameStateType.Main)
                return;

            if (_tempGear != null)
            {
                Destroy(_tempGear.gameObject);
                _tempGear = null;
            }
            GetComponent<CanvasGroup>().alpha = 1f;
        }
    }

    [System.Serializable]
    public class GearNeighbor
    {
        public GearController gear;
        public int direction;
    }
}
