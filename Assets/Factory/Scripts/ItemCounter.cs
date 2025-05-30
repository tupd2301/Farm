using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    public class ItemCounter : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Item"))
            {
                var item = other.gameObject.GetComponent<ItemController>();
                CollectItem(item);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Item"))
            {
                var item = other.gameObject.GetComponent<ItemController>();
                CollectItem(item);
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            return;
            if (other.gameObject.CompareTag("Item"))
            {
                var item = other.gameObject.GetComponent<ItemController>();
                CollectItem(item);
            }
        }

        public void CollectItem(ItemController item) { }
    }
}
