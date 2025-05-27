using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using RoomDecoration.CameraUtilities;
using Atom;

namespace RoomDecoration
{
    public class FurnitureManager : MonoBehaviour
    {
        public List<FurnitureConfig> rooms;

        public List<PlaceFurnitureButton> buttons;
        [HideInInspector] public Room currentRoom;
        public bool enableCameraZoom = true;

        [Header("Effects")]
        public bool useEffect;
        public ParticleSystem placeEffect;
        public AudioSource placeAudio;

        protected RoomDecorationUI ui;
        protected RoomDecorationSaveData saveData;
        protected int currentRoomIndex;
        protected string currentRoomName;
        protected int currentRoomLevel;
        protected int currentRoomMaxLevel;

        protected List<FurnitureCollection> furnitureList;
        protected List<string> spawnFurnitureNames;
        
        private void Start()
        {
            LoadData();
            SetupUI();
            PrepareScene();
        }

        private void PrepareScene()
        {
            if (currentRoom != null)
            {
                Destroy(currentRoom.gameObject);
            }
            // Instantiate Room
            var roomGO = Instantiate(LoadPrefabFromResources(rooms[currentRoomIndex].RoomName, RoomDecorationConstants.PATH_ROOM));
            currentRoom = roomGO.GetComponent<Room>();
            currentRoom.Init();
            buttons = currentRoom.buttons;
            foreach (var button in buttons)
            {
                button.OnClicked += OnPlaceFurnitureButtonClicked;
            }
            // Instantiate unlocked furniture
            foreach (var collection in furnitureList)
            {
                var index = collection.CurrentIndex;
                if (index == -1 || collection.Prefabs[index].IsUnlocked == false)
                {
                    continue;
                }

                for (int i = 0; i <= index; i++)
                {
                    var prefabNames = collection.Prefabs[i].PrefabNames;
                    for (int j = 0; j < prefabNames.Count; j++)
                    {
                        currentRoom.GetFurnitureByName(prefabNames[j])?.gameObject.SetActive(true);
                    }
                }
            }

            ShowButtons();
        }

        private void SetupUI()
        {
            if (ui != null)
                return;

            ui = AppManager.Instance.ShowUI<RoomDecorationUI>("RoomDecorationUI");
            ui.HideCompletePanel();
            ui.SetupProgressBar(currentRoomMaxLevel, currentRoomLevel);

            if (currentRoomLevel >= currentRoomMaxLevel && currentRoomIndex < rooms.Count - 1)
            {
                ui.ShowCompletePanel();
            }

            ui.SetNextButtonCallback(ChangeRoom);
        }

        private void ChangeRoom()
        {
            if (currentRoomIndex + 1 > rooms.Count)
            {
                return;
            }
            currentRoomIndex += 1;
            SetVariables();
            PrepareScene();
            ui.HideCompletePanel();
        }

        private void OnPlaceFurnitureButtonClicked(FurnitureType type)
        {
            //Debug.Log("<color=green>RoomDecoration</color>: OnPlaceFurnitureButtonClicked");

            FurnitureCollection furnitureCollection = furnitureList.FirstOrDefault(f => f.Type == type);
            if (furnitureCollection == null) { return; }
            if (furnitureCollection.CurrentIndex + 1 >= furnitureCollection.Prefabs.Count) { return; }

            HideButtons();

            furnitureCollection.CurrentIndex += 1;
            furnitureCollection.Prefabs[furnitureCollection.CurrentIndex].IsUnlocked = true;

            spawnFurnitureNames = furnitureCollection.Prefabs[furnitureCollection.CurrentIndex].PrefabNames;

            if (enableCameraZoom)
            {
                CameraZoomEvent.Trigger(currentRoom.GetFurnitureByName(spawnFurnitureNames[0]).transform.position);
            }
            else
            {
                SpawnFurnitures();
            }

            currentRoomLevel += 1;
            ui.UpdateProgressBar(currentRoomLevel);

            if (currentRoomLevel >= currentRoomMaxLevel && currentRoomIndex < rooms.Count - 1)
            {
                ui.ShowCompletePanel();
            }

            // Save
            SaveData();
        }

        private void SpawnFurnitures()
        {
            if (useEffect)
            {
                placeEffect.transform.position = currentRoom.GetFurnitureByName(spawnFurnitureNames[0]).transform.position;
                placeEffect.Play();
                placeAudio.Play();
            }
            
            for (int i = 0; i < spawnFurnitureNames.Count; i++)
            {
                currentRoom.GetFurnitureByName(spawnFurnitureNames[i])?.gameObject.SetActive(true);

            }
        }

        private void HideButtons()
        {
            foreach (var button in buttons)
            {
                button.gameObject.SetActive(false);
            }
        }

        private void ShowButtons()
        {
            foreach (var collection in furnitureList)
            {
                var index = collection.CurrentIndex;
                var button = buttons.FirstOrDefault(b => b.furnitureType == collection.Type);
                if (index < collection.Prefabs.Count - 1 && currentRoomLevel >= button.unlockLevel)
                {
                    button.SetPrice(collection.Prefabs[index + 1].Price);
                    button.gameObject.SetActive(true);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        #region SaveLoad

        private void LoadData()
        {
            string path = Path.Combine(Application.persistentDataPath, RoomDecorationConstants.SAVE_NAME);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                saveData = JsonConvert.DeserializeObject<RoomDecorationSaveData>(json);
                Debug.Log($"RoomDecoration: Save file loaded from: {path}");
            }
            else
            {
                Debug.Log("RoomDecoration: Save file not found, creating new one");
                saveData = new RoomDecorationSaveData();
                saveData.rooms = rooms;
            }
            currentRoomIndex = saveData.currentRoomIndex;
            SetVariables();
        }

        private void SetVariables()
        {
            currentRoomName = saveData.rooms[currentRoomIndex].RoomName;
            currentRoomLevel = saveData.rooms[currentRoomIndex].Level;
            furnitureList = saveData.rooms[currentRoomIndex].Furnitures;
            currentRoomMaxLevel = furnitureList.Sum(f => f.Prefabs.Count);
        }

        private void SaveData()
        {
            saveData.currentRoomIndex = currentRoomIndex;
            saveData.currentRoomName = currentRoomName;
            saveData.rooms[currentRoomIndex].Level = currentRoomLevel;
            string path = Path.Combine(Application.persistentDataPath, RoomDecorationConstants.SAVE_NAME);
            string json = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(path, json);
            Debug.Log($"RoomDecorationData saved to: {path}");
        }

        private GameObject LoadPrefabFromResources(string prefabName, string prefix = "")
        {
            return Resources.Load<GameObject>(prefix + "/" + prefabName);
        }

        #endregion
        private void OnEnable()
        {
            CameraZoomOutStopEvent.Register(ShowButtons);
            CameraZoomInStopEvent.Register(SpawnFurnitures);
        }

        private void OnDisable()
        {
            CameraZoomOutStopEvent.Unregister(ShowButtons);
            CameraZoomInStopEvent.Unregister(SpawnFurnitures);
            foreach (var button in buttons)
            {
                button.OnClicked -= OnPlaceFurnitureButtonClicked;
            }
        }

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            foreach (var room in rooms)
            {
                room.ResetData();
            }
        }
#endif
    }
}

