using System.Collections.Generic;
using UnityEngine;

namespace Tank
{
    public class ProjectilePool : MonoBehaviour
    {
        public static ProjectilePool Instance;
        public GameObject projectilePrefab;
        public int poolSize = 20;

        private List<GameObject> pool;

        void Awake()
        {
            Instance = this;
            pool = new List<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(projectilePrefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                pool.Add(obj);
            }
        }

        public GameObject GetProjectile()
        {
            foreach (var obj in pool)
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
            // Optionally expand pool if needed
            GameObject objNew = Instantiate(projectilePrefab);
            objNew.SetActive(false);
            objNew.transform.SetParent(transform);
            pool.Add(objNew);
            return objNew;
        }
    }
} 