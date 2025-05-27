using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoomDecoration
{
    [CreateAssetMenu(fileName = "FurnitureList", menuName = "Atom/RoomDecoration/FurnitureList", order = 0)]
    public class FurnitureConfig : ScriptableObject
    {
        public string RoomName;
        [HideInInspector] public bool IsCompleted;
        public int Level;
        public List<FurnitureCollection> Furnitures;

        [ContextMenu("ResetData")]
        public void ResetData()
        {
            foreach (var furnitureCollection in Furnitures)
            {
                furnitureCollection.CurrentIndex = -1;
                foreach (var prefab in furnitureCollection.Prefabs)
                {
                    prefab.IsUnlocked = false;
                }
            }
            Level = 0;
            IsCompleted = false;
        }

        public int GetTotalFurnitures()
        {
            int total = 0;
            foreach (var furnitureCollection in Furnitures)
            {
                total += furnitureCollection.Prefabs.Count;
            }
            return total; 
        }
    }

    
}
