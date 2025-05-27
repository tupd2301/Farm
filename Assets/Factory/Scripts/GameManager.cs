using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atom;
using UnityEngine;

namespace Factory
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public Camera mainCamera;
        public HomeUI homeUI;

        [SerializeField]
        private GameState _gameState;
        public GameState GameState => _gameState;

        [SerializeField]
        private Rotate _circle;

        [SerializeField]
        private float _circleSpeed;

        [SerializeField]
        private GameObject _itemContainer;

        [SerializeField]
        private PoolSystem _itemPool;

        [SerializeField]
        private GearDataSO _gearDataSO;

        [SerializeField]
        private ItemDataSO _itemDataSO;

        [SerializeField]
        private LevelConfigSO _levelConfigSO;

        private List<GameObject> _activeItems = new List<GameObject>();
        public List<GearController> _gearControllers = new List<GearController>();
        public GameObject ItemContainer => _itemContainer;
        public HomeUI HomeUI => homeUI;
        private const string ITEM_POOL_ID = "Item";

        private LevelConfiguration _currentLevelConfig;
        public int currentLevel = 0;
        public int currentDay = 0;
        private bool _isFirstOpenShop = false;
        private int _gold = 0;

        public bool isStop = false;

        void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            _gameState = GetComponent<GameState>();
        }

        public void Start()
        {
            currentLevel = 0;
            _currentLevelConfig = GetCurrentLevelConfig();

            homeUI = AppManager.Instance.ShowSafeTopUI<HomeUI>("Factory/HomeUI");
            if (homeUI == null)
            {
                return;
            }
            homeUI.StartButton.onClick.AddListener(StartGame);
            // _circle.rotation = new Vector3(0, 0, _circleSpeed);
            LoadLevel(0);
        }

        public async Task LoadLevel(int level)
        {
            Debug.Log($"LoadLevel {level}");
            _isFirstOpenShop = false;
            isStop = true;
            currentLevel = level;
            _currentLevelConfig = GetCurrentLevelConfig();
            currentDay = 0;
            _gold = 0;

            InitGears(_currentLevelConfig.gridSize);
            InitBoxes(GetCurrentDayConfig().boxConfigs);
            UpdateGold(_gold + _currentLevelConfig.initialLevelCurrency);
            ChangeGameState(GameStateType.Shop);
            await homeUI.ShowGameStartPanel();
        }

        public LevelConfiguration GetCurrentLevelConfig()
        {
            if (currentLevel < 0 || currentLevel >= _levelConfigSO.levelConfigs.Count)
            {
                return null;
            }
            return _levelConfigSO.levelConfigs[currentLevel];
        }

        public DayConfiguration GetCurrentDayConfig()
        {
            int dayNumber = currentDay;
            if (
                _currentLevelConfig == null
                || _currentLevelConfig.dayConfigurations == null
                || dayNumber < 0
                || dayNumber >= _currentLevelConfig.dayConfigurations.Count
            )
            {
                return null;
            }
            return _currentLevelConfig.dayConfigurations[dayNumber];
        }

        public void InitBoxes(List<BoxConfigDay> boxConfigs)
        {
            BoxManager.Instance.Init(boxConfigs);
        }

        public async Task NextDay()
        {
            currentDay++;
            ClearItems();
            Debug.Log($"NextDay {currentDay}");
            isStop = true;
            if (GetCurrentDayConfig() == null)
            {
                await ShowWinPanel();
                return;
            }
            _currentLevelConfig = GetCurrentLevelConfig();
            await homeUI.ShowGameStartPanel();
            ClearItems();
            InitBoxes(GetCurrentDayConfig().boxConfigs);
            UpdateGold(_gold + _currentLevelConfig.initialLevelCurrency);
            ChangeGameState(GameStateType.Shop);
        }

        public async Task NextLevel()
        {
            isStop = true;
            currentLevel++;
            Debug.Log($"NextLevel {currentLevel}");
            if (GetCurrentLevelConfig() == null)
            {
                currentLevel = 1;
            }
            currentDay = 0;
            ClearAllGears();
            ClearItems();
            await LoadLevel(currentLevel);
        }

        public async Task ShowWinPanel()
        {
            isStop = true;
            Debug.Log($"ShowWinPanel");
            await homeUI.ShowWinPanel();
            await NextLevel();
            await Task.Delay(2000);
        }

        public async Task ShowLosePanel()
        {
            return;
            isStop = true;
            Debug.Log($"ShowLosePanel");
            await homeUI.ShowLosePanel();
            await Task.Delay(2000);
            currentDay = 0;
            ClearAllGears();
            ClearItems();
            LoadLevel(currentLevel);
        }

        public void StartGame()
        {
            ChangeGameState(GameStateType.Main);
            // ActivateAllHeadGears();
        }

        public void ChangeGameState(GameStateType state)
        {
            _gameState.SetState(state);
            ClearItems();
        }

        public void ActivateAllHeadGears()
        {
            foreach (var gear in _gearControllers)
            {
                if (gear.isHead)
                {
                    gear.Rotate();
                }
            }
        }

        public GameObject SpawnItem(
            Vector2 position,
            GearController gearController,
            float Amplifier
        )
        {
            var checkItemData = GetItemDataByItemID(gearController.itemData.itemId);
            if (checkItemData == null)
            {
                return null;
            }
            var item = _itemPool.GetObject(ITEM_POOL_ID);
            item.GetComponent<Collider2D>().enabled = true;
            item.GetComponent<SpriteRenderer>().material = new Material(
                item.GetComponent<SpriteRenderer>().material
            );
            item.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", 1f);
            item.transform.SetParent(_itemContainer.transform);
            position = position * mainCamera.orthographicSize / 6.4f;
            item.transform.position = new Vector3(position.x, position.y, 0);
            float cost = (float)(checkItemData.cost * gearController.gearData.level + Amplifier);
            item.GetComponent<ItemController>().SetItemData(checkItemData, cost);
            System.Random random = new System.Random();
            float randomX = random.Next(-3, 3);
            item.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX, 0), ForceMode2D.Impulse);
            _activeItems.Add(item);
            return item;
        }

        public bool CheckActiveItemCount()
        {
            return _activeItems.Count > 50;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearItems();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ChangeGameState(GameStateType.Shop);
            }
        }

        [ContextMenu("Clear Items")]
        public void ClearItems()
        {
            foreach (var item in _activeItems)
            {
                _itemPool.ReturnObject(item, ITEM_POOL_ID);
            }
            _activeItems.Clear();
        }

        public void ClearAllGears()
        {
            foreach (var gearController in _gearControllers)
            {
                Destroy(gearController.gameObject);
            }
            _gearControllers.Clear();
        }

        public void UpdateGold(int gold)
        {
            _gold = gold;
            homeUI.UpdateGoldText(gold);
        }

        public void AddGold(int gold)
        {
            _gold += gold;
            homeUI.UpdateGoldText(_gold);
        }

        public void InitGears(Vector2 gridSize)
        {
            ClearAllGears();
            float size = 110;
            homeUI.GearItemContainer.cellSize = new Vector2(size, size);
            homeUI.GearItemContainer.spacing = new Vector2(1, 5);
            homeUI.GearItemContainer.constraintCount = (int)gridSize.x;
            for (int i = 0; i < gridSize.y; i++)
            {
                for (int j = 0; j < gridSize.x; j++)
                {
                    var gearController = Instantiate(
                            Resources.Load<GameObject>("Prefabs/Gear"),
                            homeUI.GearItemContainer.transform
                        )
                        .GetComponent<GearController>();
                    _gearControllers.Add(gearController);
                    if ((i % 2 == 0 && j % 2 == 1) || (i % 2 == 1 && j % 2 == 0))
                    {
                        gearController.startAngle = (45 / 2f);
                    }
                    gearController.gridCoordinate = new Vector2(i, j);
                    gearController.SetGear(6);
                    gearController.Hide();
                    gearController.OnRotate += (float Amplifier) =>
                    {
                        Debug.Log(
                            $"Rotate {gearController.itemData.itemName}, {Amplifier},{isStop}, {CheckActiveItemCount()}, {gearController.isHead}"
                        );
                        if (
                            !gearController.isHead
                            && gearController.itemData != null
                            && !isStop
                            && !CheckActiveItemCount()
                        )
                        {
                            Debug.Log("Spawn Item");    
                            var screenPosition = gearController.transform.position;
                            SpawnItem(screenPosition, gearController, Amplifier);
                        }
                    };
                }
            }
            for (int i = 0; i < GetCurrentLevelConfig().maxHeadGearSlots; i++)
            {
                SetRandomHeadGear();
            }
            CalculateGearNeighbors();
        }

        public void SetRandomHeadGear()
        {
            System.Random random = new System.Random();
            int gearindex = random.Next(0, _gearControllers.Count);
            do
            {
                gearindex = random.Next(0, _gearControllers.Count);
            } while (
                _gearControllers[gearindex].isHead
                || (
                    (
                        _gearControllers[gearindex].gridCoordinate.x % 2 == 0
                        && _gearControllers[gearindex].gridCoordinate.y % 2 == 1
                    )
                    || (
                        _gearControllers[gearindex].gridCoordinate.x % 2 == 1
                        && _gearControllers[gearindex].gridCoordinate.y % 2 == 0
                    )
                )
            );
            _gearControllers[gearindex].SetGear(1);
            _gearControllers[gearindex].Rotate();
            _gearControllers[gearindex].Show();
            _gearControllers[gearindex].isHead = true;
        }

        public void CalculateGearNeighbors()
        {
            foreach (var gear in _gearControllers)
            {
                Vector2 coord = gear.gridCoordinate;
                int direction = 0;
                // Check all adjacent positions (up, down, left, right)
                Vector2[] adjacentPositions = new Vector2[]
                {
                    new Vector2(coord.x + 1, coord.y), // right - direction 3
                    new Vector2(coord.x - 1, coord.y), // left - direction 1
                    new Vector2(coord.x, coord.y + 1), // up - direction 0
                    new Vector2(coord.x, coord.y - 1), // down - direction 2
                };

                foreach (var pos in adjacentPositions)
                {
                    var neighbor = _gearControllers.Find(g => g.gridCoordinate == pos);
                    if (neighbor != null)
                    {
                        switch (pos)
                        {
                            case Vector2 v when v.x == coord.x && v.y == coord.y + 1:
                                SetGearConnection(gear, neighbor, 3);
                                break;
                            case Vector2 v when v.x == coord.x - 1 && v.y == coord.y:
                                SetGearConnection(gear, neighbor, 0);
                                break;
                            case Vector2 v when v.x == coord.x && v.y == coord.y - 1:
                                SetGearConnection(gear, neighbor, 1);
                                break;
                            case Vector2 v when v.x == coord.x + 1 && v.y == coord.y:
                                SetGearConnection(gear, neighbor, 2);
                                break;
                        }
                        direction++;
                    }
                }
            }
        }

        public void SetGearConnection(
            GearController gearController,
            GearController connectedGearController,
            int direction
        )
        {
            if (gearController.connectedGears.Find(g => g.gear == connectedGearController) != null)
            {
                return;
            }
            gearController.Connect(connectedGearController, direction);
        }

        public void CollectItem(ItemController item)
        {
            _itemPool.ReturnObject(item.gameObject, ITEM_POOL_ID);
            if (_activeItems.Contains(item.gameObject))
            {
                _activeItems.Remove(item.gameObject);
            }
        }

        public void DisconnectGear(
            GearController gearController,
            GearController connectedGearController,
            int direction
        )
        {
            gearController.Disconnect(connectedGearController, direction);
        }

        public void ForceGearsInShop()
        {
            var totalWeight = 0f;

            foreach (var gearData in _gearDataSO.gearDataList.FindAll(g => g.id != 0))
            {
                totalWeight += gearData.weight;
            }

            homeUI.ShopItems[1].gear.SetGearData(_gearDataSO.gearDataList[0]);
            homeUI.ShopItems[1].gear.SetItemData(_gearDataSO.gearDataList[0]);
            homeUI.ShopItems[1].gear.isInShop = true;
            homeUI.ShopItems[1].gear.Show();
            homeUI.ShopItems[1].gear.FillItemIcon(1);
            homeUI.UpdateCostText(homeUI.ShopItems[1], (int)_gearDataSO.gearDataList[0].cost);
            homeUI.ShopItems[1].purchased = false;
            homeUI.ShopItems[1].gear.OnDropShop = null;
            homeUI.ShopItems[1].gear.OnDropShop += (gearData) =>
            {
                if (_gold >= (int)gearData.cost)
                {
                    _gold -= (int)gearData.cost;
                    UpdateGold(_gold);
                    homeUI.ShopItems[1].purchased = true;
                }
                CheckGoldAllGearsInShop();
            };

            foreach (var shopItem in homeUI.ShopItems)
            {
                if (shopItem.gear == homeUI.ShopItems[1].gear)
                {
                    continue;
                }
                System.Random random = new System.Random();
                float randomValue = random.Next(0, (int)totalWeight);
                float currentWeight = 0f;

                foreach (var gearData in _gearDataSO.gearDataList.FindAll(g => g.id != 0))
                {
                    currentWeight += gearData.weight;
                    if (randomValue <= currentWeight)
                    {
                        shopItem.gear.SetGearData(gearData);
                        shopItem.gear.SetItemData(gearData);
                        shopItem.gear.isInShop = true;
                        homeUI.UpdateCostText(shopItem, (int)gearData.cost);
                        shopItem.gear.Show();
                        shopItem.gear.FillItemIcon(1);
                        shopItem.gear.OnDropShop = null;
                        shopItem.gear.OnDropShop += (gearData) =>
                        {
                            if (_gold >= (int)gearData.cost)
                            {
                                _gold -= (int)gearData.cost;
                                UpdateGold(_gold);
                                shopItem.purchased = true;
                            }
                            CheckGoldAllGearsInShop();
                        };
                        shopItem.purchased = false;
                        break;
                    }
                }
            }
            CheckGoldAllGearsInShop();
        }

        public bool CheckGold(int cost)
        {
            return _gold >= cost;
        }

        [ContextMenu("CheckGoldAllGearsInShop")]
        public void CheckGoldAllGearsInShop()
        {
            foreach (var shopItem in homeUI.ShopItems)
            {
                if (shopItem.gear.isInShop)
                {
                    if (
                        shopItem.gear.gearData == null
                        || !CheckGold(shopItem.gear.gearData.cost)
                        || String.IsNullOrEmpty(shopItem.gear.gearData.itemName)
                        || shopItem.purchased
                    )
                    {
                        homeUI.StrikethroughCostText(shopItem, true);
                        homeUI.UpdatePanelImage(shopItem, true);
                    }
                    else
                    {
                        homeUI.StrikethroughCostText(shopItem, false);
                        homeUI.UpdatePanelImage(shopItem, false);
                    }
                }
            }
            homeUI.SetLockRerollButton(CheckGold(5));
        }

        public bool CheckItemInShop(GearController gear)
        {
            return !CheckGold(gear.gearData.cost) || String.IsNullOrEmpty(gear.gearData.itemName);
        }

        public void CheckFirstOpenShop()
        {
            if (_isFirstOpenShop && GameState.Instance.CurrentState == GameStateType.Shop)
            {
                var shopItems = homeUI.ShopItems.FindAll(g =>
                    g.gear.gearData != null && !g.gear.isInShop
                );
                if (shopItems.Count < 2)
                {
                    homeUI.UnlockStartButton();
                }
            }
        }

        public void RandomGearsInShop()
        {
            if (!_isFirstOpenShop)
            {
                ForceGearsInShop();
                homeUI.LockStartButton();
                _isFirstOpenShop = true;
                return;
            }
            _gold -= 5;
            UpdateGold(_gold);
            var totalWeight = 0f;
            foreach (var gearData in _gearDataSO.gearDataList)
            {
                totalWeight += gearData.weight;
            }

            foreach (var shopItem in homeUI.ShopItems)
            {
                System.Random random = new System.Random();
                float randomValue = random.Next(0, (int)totalWeight);
                float currentWeight = 0f;

                foreach (var gearData in _gearDataSO.gearDataList)
                {
                    currentWeight += gearData.weight;
                    if (randomValue <= currentWeight)
                    {
                        shopItem.gear.SetGearData(gearData);
                        shopItem.gear.SetItemData(gearData);
                        shopItem.gear.isInShop = true;
                        shopItem.gear.Show();
                        homeUI.UpdateCostText(shopItem, (int)shopItem.gear.gearData.cost);
                        shopItem.gear.FillItemIcon(1);
                        shopItem.purchased = false;
                        break;
                    }
                }
            }
            CheckGoldAllGearsInShop();
        }

        public ItemData GetItemDataByGearID(int id)
        {
            return _itemDataSO.itemDataList.Find(item => item.gearId == id);
        }

        public ItemData GetItemDataByItemID(int id)
        {
            return _itemDataSO.itemDataList.Find(item => item.itemId == id);
        }
    }
}
