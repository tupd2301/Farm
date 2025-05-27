using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomDecoration
{
    public class FurnitureSpawner : MonoBehaviour
    {
        public static FurnitureSpawner Instance;
        public string furnitureName;
        public Transform furnitureHolder;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start() 
        {
            furnitureHolder = new GameObject().transform;
            furnitureHolder.name = "FurnitureHolder";
        }

        public void SpawnFurniture()
        {
            Instantiate(Resources.Load<GameObject>(furnitureName), furnitureHolder);
        }
    }
}