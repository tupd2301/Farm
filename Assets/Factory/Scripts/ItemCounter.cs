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
        public void CollectItem(ItemController item)
        {
            if (item.isCollected || BoxManager.Instance.isBoxClosing)
            {
                GameManager.Instance.CollectItem(item);
                return;
            }
            item.isCollected = true;
            BoxManager.Instance.GetItem(item);
            Debug.Log("Item collected");
            GameManager.Instance.CollectItem(item);
        }
    }
}
