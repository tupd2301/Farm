using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tank
{
    public class Brick : MonoBehaviour, IDamageable
    {
        private int hitPoints = 1;
        public int maxHitPoints = 100;

        public ParticleSystem vfx;

        void Start()
        {
            Material material = GetComponent<Renderer>().material;
            material = new Material(material);
            material.SetFloat("_RandomSeed", Random.Range(0f, 1000f));
            GetComponent<Renderer>().material = material;
            hitPoints = maxHitPoints;
        }

        public void TakeDamage(int amount)
        {
            hitPoints -= amount;
            GetComponent<Renderer>()
                .material.SetFloat("_Threshold", 1 - hitPoints / (float)maxHitPoints);
            vfx.Play();
            if (hitPoints <= 0)
            {
                Destroy(gameObject, 1f);
            }
        }
    }
}
