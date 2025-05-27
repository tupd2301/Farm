using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Rotate : MonoBehaviour
    {
        public Vector3 rotation;

        [SerializeField]
        private Rigidbody2D _rb;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            _rb.angularVelocity = rotation.z;
        }
    }
}
