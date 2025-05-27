using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoomDecoration
{
    public enum FurnitureType
    {
        None,
        A,
        B,
        C,
        D,
        E,
        F
    }

    [Serializable]
    public class FurnitureCollection
    {
        public FurnitureType Type;
        [HideInInspector] public int CurrentIndex = -1;
        public List<FurnitureData> Prefabs;
    }

    [Serializable]
    public class FurnitureData
    {
        public List<string> PrefabNames;
        public int Price;
        [HideInInspector] public bool IsUnlocked;
    }

    [Serializable]
    public class FurnitureList
    {
        public List<FurnitureCollection> Furnitures;
    }

    [Serializable]
    public class PlaceFurnitureButtonData
    {
        public Vector3 Position;
        //public FurnitureType Type;
        public int UnlockLevel;
    }
}
