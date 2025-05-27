using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoomDecoration
{
    public class Room : MonoBehaviour
    {
        public List<Furniture> furnitures;
        public ParticleSystem vfx;
        public Transform buttonContainer;
        public List<PlaceFurnitureButton> buttons;

        private void Awake()
        {
            furnitures = GetComponentsInChildren<Furniture>(includeInactive: true).ToList();
        }

        public void Init()
        {
            foreach (var furniture in furnitures)
            {
                furniture.gameObject.SetActive(false);
            }
            buttons = buttonContainer.GetComponentsInChildren<PlaceFurnitureButton>().ToList();
        }

        public Furniture GetFurnitureByName(string name)
        {
            //Debug.Log("Getting furniture by name: " + name);
            return furnitures.FirstOrDefault(f => f.name == name);
        }
    }
}
