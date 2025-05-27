using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomDecoration
{
    public class Furniture : MonoBehaviour
    {
        public Vector3 placePosition;
        public Quaternion placeRotation;

        private void Start() 
        {
            //SetPlacePositionAndRotation();
        }

        [ContextMenu("Set Place Position And Rotation")]
        public void SetPlacePositionAndRotation()
        {
            transform.SetPositionAndRotation(placePosition, placeRotation);
        }

        [ContextMenu("Get Place Position And Rotation")]
        public void GetPlacePositionAndRotation() 
        {
            placePosition = transform.position;
            placeRotation = transform.rotation;
        }
    }
}

