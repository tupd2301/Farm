using UnityEngine;

namespace Tank
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 10f;
        public float lifeTime = 2f;
        public int damage = 1;
        private float timer;

        private void OnEnable()
        {
            timer = 0f;
        }

        void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= lifeTime)
            {
                gameObject.SetActive(false);
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            // Check if hit a brick
            Brick brick = collision.gameObject.GetComponent<Brick>();
            if (brick != null)
            {
                Debug.Log("Projectile hit a brick at " + collision.contacts[0].point);
            }
            
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Debug.Log("Applying " + damage + " damage to " + collision.gameObject.name);
                    damageable.TakeDamage(damage);
                }
            
            // Deactivate projectile after any collision
            gameObject.SetActive(false);
        }
    }
} 