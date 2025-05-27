using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    public class Suction : MonoBehaviour
    {
        [SerializeField]
        private float _suctionForce = 10;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Item"))
            {
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            Debug.Log("OnTriggerExit2D");
        }
    }
}
